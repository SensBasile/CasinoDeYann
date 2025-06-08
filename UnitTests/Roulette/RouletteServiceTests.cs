using System.Reflection;
using CasinoDeYann.Controllers.Roulette.DTOs;
using CasinoDeYann.Services.Roulette;
using CasinoDeYann.Services.Stats;
using CasinoDeYann.Services.Stats.Models;
using CasinoDeYann.Services.User;
using FluentAssertions;
using Moq;

namespace UnitTests.Roulette
{
    public class RouletteServiceTests
    {
        private static MethodInfo GetPrivateMethod(string name) =>
            typeof(RouletteService)
              .GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)
              ?? throw new InvalidOperationException($"Method {name} not found");

        private static T InvokePrivate<T>(RouletteService svc, string methodName, params object[] args) =>
            (T)GetPrivateMethod(methodName).Invoke(svc, args);

        private class DeterministicRandom : Random
        {
            private readonly int _fixed;
            public DeterministicRandom(int fixedNext) => _fixed = fixedNext;
            public override int Next(int minValue, int maxValue) => _fixed;
        }

        [Fact]
        public void GetTotalBetValue_SumsAllBetsCorrectly()
        {
            // Arrange
            var svc = CreateService();
            var req = new RouletteRequest
            {
                Singles    = new [] { new SingleBetRequest { Number = 5, Amount = 10 } },
                Even       = 7,
                Odd        = null,
                Red        = 3,
                Black      = 2,
                FirstTier  = 1,
                SecondTier = null,
                ThirdTier  = 4,
                FirstRow   = 5,
                SecondRow  = null,
                ThirdRow   = 6,
                FirstHalf  = 8,
                SecondHalf = 9
            };

            // Act
            long total = InvokePrivate<long>(svc, "getTotalBetValue", req);

            // Assert: 10 + 7 + 3 + 2 + 1 + 4 + 5 + 6 + 8 + 9 == 55
            total.Should().Be(55);
        }

        [Theory]
        // Single wins when number matches: Amount * 36
        [InlineData("Singles",    10, 5, 360, 4)]  // also uses Next=4 so no match
        // Even
        [InlineData("Even",       10, 4, 20, 3)]
        [InlineData("Even",       10, 5, 0,  69)]
        // Odd
        [InlineData("Odd",        7,  3, 14, 6)]
        [InlineData("Odd",        7,  4, 0,  2)]
        // Red (3 is red)
        [InlineData("Red",        5,  3, 10, 2)]
        [InlineData("Red",        5,  4, 0,  2)]
        // Black (4 is black)
        [InlineData("Black",      8,  4, 16, 0)]
        [InlineData("Black",      8,  3, 0,  3)]
        // FirstTier 1–12
        [InlineData("FirstTier",  2,  7, 6, 13)]
        [InlineData("FirstTier",  2, 13, 0,  20)]
        // SecondTier 13–24
        [InlineData("SecondTier", 3, 15, 9,  2)]
        [InlineData("SecondTier", 3, 12, 0,  5)]
        // ThirdTier 25–36
        [InlineData("ThirdTier",  4, 30,12, 17)]
        [InlineData("ThirdTier",  4, 24, 0,  6)]
        // FirstRow: 1,4,7,... (n−1)%3==0
        [InlineData("FirstRow",   5,  7, 15, 8)]
        [InlineData("FirstRow",   5,  4, 15, 5)]
        [InlineData("FirstRow",   5,  2,  0, 2)]
        // SecondRow: (n−2)%3==0
        [InlineData("SecondRow", 6, 5, 18, 4)]
        [InlineData("SecondRow", 6, 2, 18, 1)]
        [InlineData("SecondRow",  6,  4,  0, 4)]
        // ThirdRow: n%3==0
        [InlineData("ThirdRow", 2,  3, 6, 4)]
        [InlineData("ThirdRow",   2,  4, 0,  4)]
        // FirstHalf 1–18
        [InlineData("FirstHalf", 10, 18,20, 19)]
        [InlineData("FirstHalf", 10, 19, 0, 19)]
        // SecondHalf 19–36
        [InlineData("SecondHalf",3, 20,6,  8)]
        [InlineData("SecondHalf",3, 18,0, 18)]
        public void ComputeGains_BetTypeBehavesAsExpected(
            string propName,
            long   amount,
            int    winNumber,
            long   expectedGain,
            int    dummyNumber
        )
        {
            // Arrange: build a request that has only that one bet set
            var req = new RouletteRequest();
            typeof(RouletteRequest)
                .GetProperty(propName)
                !.SetValue(req, propName == "Singles"
                    ? new[] { new SingleBetRequest { Number = winNumber, Amount = amount } }
                    : (object?)amount);

            // non-Single bets
            var svc = CreateService();
            
            // also put a losing entry for Singles if testing Singles, else dummy
            if (propName == "Singles")
            {
                // test both match and non-match: first call win, second call lose
                // win
                long gainWin = InvokePrivate<long>(CreateService(), "ComputeGains", req, winNumber);
                gainWin.Should().Be(expectedGain);

                // lose
                ((SingleBetRequest[])req.Singles!)[0].Number = dummyNumber;
                long gainLose = InvokePrivate<long>(svc, "ComputeGains", req, winNumber);
                gainLose.Should().Be(0);
                return;
            }
            long gain1 = InvokePrivate<long>(svc, "ComputeGains", req, winNumber);
            gain1.Should().Be(expectedGain);

            long gain2 = InvokePrivate<long>(svc, "ComputeGains", req, dummyNumber);
            gain2.Should().Be(0);
        }

        [Theory]
        [InlineData( 1,   100, 7,   true)]  // single matches → win
        [InlineData( 2,    50, 8,   false)] // even bet on 8 → win? 8 is even, but singles only→ lose
        public async Task Play_UpdatesUserAndStats_AndReturnsCorrectModel(
            int    betNumber,
            long   betAmount,
            int    forcedWinNumber,
            bool   expectWin
        )
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var statsServiceMock= new Mock<IStatsService>();

            // totalBet will equal betAmount
            var initialUser = new CasinoDeYann.Services.User.Models.User(1, "alice", 1000, 1000, "User");
            userServiceMock
                .Setup(u => u.Pay("alice", betAmount))
                .ReturnsAsync(initialUser);

            // capture exp additions
            userServiceMock.Setup(u => u.AddExp("alice", It.IsAny<long>())).ReturnsAsync(initialUser);

            // capture money addition
            var afterMoney = expectWin 
                ? initialUser.Money - betAmount + (expectWin ? betAmount * 36 : 0)
                : initialUser.Money - betAmount;
            userServiceMock
                .Setup(u => u.AddMoney("alice", It.IsAny<long>()))
                .ReturnsAsync(afterMoney);

            statsServiceMock
                .Setup(s => s.Create(It.IsAny<GameHistoryEntryModel>()))
                .Returns(Task.FromResult<GameHistoryEntryModel>(null));

            var svc = new RouletteService(userServiceMock.Object, statsServiceMock.Object);
            // inject deterministic random
            var randField = typeof(RouletteService)
                .GetField("_random", BindingFlags.Instance | BindingFlags.NonPublic)!;
            randField.SetValue(svc, new DeterministicRandom(forcedWinNumber));

            var req = new RouletteRequest
            {
                Singles = new[] { new SingleBetRequest { Number = betNumber, Amount = betAmount } }
            };

            // Act
            var result = await svc.play("alice", req);

            // Assert model
            result.WinningNumber.Should().Be(forcedWinNumber);
            var expectedGain = expectWin && betNumber == forcedWinNumber
                ? betAmount * 36
                : 0;
            result.Gain.Should().Be(expectedGain);
            if (expectWin && betNumber == forcedWinNumber)
                result.Message.Should().Contain($"remporté {expectedGain - betAmount}");
            else
                result.Message.Should().Contain($"perdu {betAmount - expectedGain}");
            result.Money.Should().Be(afterMoney);

            // verify service calls
            userServiceMock.Verify(u => u.Pay("alice", betAmount), Times.Once);
            userServiceMock.Verify(u => u.AddExp("alice", betAmount / 100 + 10), Times.Once);
            userServiceMock.Verify(u => u.AddMoney("alice", expectedGain), Times.Once);
            userServiceMock.Verify(u => u.AddExp("alice", expectedGain / 700 + 35), Times.Once);

            statsServiceMock.Verify(s => s.Create(
                It.Is<GameHistoryEntryModel>(gh =>
                    gh.Username    == "alice" &&
                    gh.Game    == "Roux'Lette" &&
                    gh.Bet    == betAmount &&
                    gh.Gain        == expectedGain
                )), Times.Once);
        }

        private static RouletteService CreateService()
            => new RouletteService(
                   Mock.Of<IUserService>(),
                   Mock.Of<IStatsService>()
               );
    }
}
