using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CasinoDeYann.DataAccess.Interfaces;
using CasinoDeYann.DataAccess.Dbo;
using CasinoDeYann.Services.Stats;
using CasinoDeYann.Services.Stats.Models;
using CasinoDeYann.Services.User;
using CasinoDeYann.Services.User.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace CasinoDeYann.Services.Stats.Tests
{
    public class StatsServiceTests
    {
        private readonly Mock<IStatsRepository> _statsRepoMock;
        private readonly Mock<UserService> _userServiceMock;
        private readonly StatsService _statsService;

        public StatsServiceTests()
        {
            _statsRepoMock   = new Mock<IStatsRepository>();
            _userServiceMock = new Mock<UserService>();
            _statsService    = new StatsService(_statsRepoMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task Create_ShouldInsertStatsAndReturnGameHistoryEntryModel()
        {
            // Arrange
            var request = new GameHistoryEntryModel(
                Id: 0,
                Username: "johndoe",
                Date:     new DateTime(2025, 6, 1),
                Game:     "Blackjack",
                Bet:      50,
                Gain:     100,
                IsCanceled: false
            );

            var user = new User.Models.User(10, "johndoe", 2000, 500 , "User");
            _userServiceMock
                .Setup(s => s.GetUser("johndoe"))
                .ReturnsAsync(user);

            var inserted = new DataAccess.Dbo.Stats
            {
                Id        = 42,
                UserId    = 10,
                Date      = request.Date,
                Game      = request.Game,
                Bet       = request.Bet,
                Gain      = request.Gain,
                IsCanceled = false
            };
            _statsRepoMock
                .Setup(r => r.Insert(It.IsAny<DataAccess.Dbo.Stats>()))
                .ReturnsAsync(inserted);

            // Act
            var result = await _statsService.Create(request);

            // Assert
            result.Should().BeEquivalentTo(new GameHistoryEntryModel(
                Id: 42,
                Username: "johndoe",
                Date: request.Date,
                Game: "Blackjack",
                Bet: 50,
                Gain: 100,
                IsCanceled: false
            ));
            _userServiceMock.Verify(s => s.GetUser("johndoe"), Times.Once);
            _statsRepoMock.Verify(r => r.Insert(It.Is<DataAccess.Dbo.Stats>(st =>
                st.UserId == 10 &&
                st.Bet    == 50m &&
                st.Gain   == 100m &&
                st.Game   == "Blackjack"
            )), Times.Once);
        }

        [Fact]
        public async Task GetPlayerStats_ShouldMapDataAndPagingCorrectly()
        {
            // Arrange
            var user = new User.Models.User( 5, "alice", 1500, 300, "User");

            var summary = new UserStatsSummary
            {
                HighestGain        = 200,
                NumberOfGames      = 4,
                TotalWon           = 500,
                TotalLost          = 250,
                GamesPlayedPerGame = new Dictionary<string, int> { { "Roulette", 2 }, { "Poker", 2 } },
                GamesPlayedPerDay  = new Dictionary<DateTime, int> { { new DateTime(2025,5,30), 3 }, { new DateTime(2025,5,31), 1 } }
            };
            _statsRepoMock
                .Setup(r => r.GetStats(5))
                .ReturnsAsync(summary);

            var statsList = new[]
            {
                new DataAccess.Dbo.Stats { Id = 1, Date = new DateTime(2025,5,30), Game = "Roulette", Bet = 100, Gain = 150, IsCanceled = false },
                new DataAccess.Dbo.Stats { Id = 2, Date = new DateTime(2025,5,31), Game = "Poker",   Bet = 200, Gain =  50, IsCanceled = true  }
            };
            var paged = new PaginatedStats(statsList, totalPages: 2);
            _statsRepoMock
                .Setup(r => r.Get("date_desc", "alice", 2, true))
                .ReturnsAsync(paged);

            // Act
            var result = await _statsService.GetPlayerStats("date_desc", user, 2);

            // Assert
            result.HighestGain.Should().Be(200);
            result.NumberOfGames.Should().Be(4);
            result.TotalWon.Should().Be(500);
            result.TotalLost.Should().Be(250);
            result.GamesPlayedPerGame["Roulette"].Should().Be(2);
            result.GamesPlayedPerDay[new DateTime(2025,5,30)].Should().Be(3);

            result.HasPrevious.Should().BeTrue();  // pageIndex > 1
            result.HasNext.Should().BeTrue();      // TotalPages == pageIndex

            result.History.Should().HaveCount(2);
            result.History.ElementAt(1).IsCanceled.Should().BeTrue();
        }

        [Fact]
        public async Task GetUserProfileAsync_ShouldReturnProfileWithStats()
        {
            // Arrange
            var user = new User.Models.User(7, "bob", 2500, 1000, "User");
            _userServiceMock
                .Setup(s => s.GetUser("bob"))
                .ReturnsAsync(user);

            var statsModel = new UserStatsModel(
                History:             new List<GameHistoryEntryModel>(),
                HasPrevious:         false,
                HasNext:             false,
                HighestGain:         0,
                NumberOfGames:       0,
                TotalWon:            0,
                TotalLost:           0,
                GamesPlayedPerGame:  new Dictionary<string,int>(),
                GamesPlayedPerDay:   new Dictionary<DateTime,int>()
            );

            // On GetPlayerStats, on cette instance appelera la méthode réelle,
            // donc on mocke elle-même pour rediriger vers notre statsModel.
            var serviceMock = new Mock<StatsService>(_statsRepoMock.Object, _userServiceMock.Object) { CallBase = true };
            serviceMock
                .Setup(s => s.GetPlayerStats("game_asc", user, 1))
                .ReturnsAsync(statsModel);

            // Act
            var profile = await serviceMock.Object.GetUserProfileAsync("game_asc", "bob", 1);

            // Assert
            profile.Level.Should().Be(2);   // 2500 / 1000 == 2
            profile.Balance.Should().Be(1000);
            profile.Stats.Should().Be(statsModel);
        }

        [Fact]
        public async Task GetBackOffice_ShouldReturnBackOfficeModel()
        {
            // Arrange
            var statsList = new[]
            {
                new DataAccess.Dbo.Stats { Id = 11, User = new DataAccess.Dbo.User { Username = "charlie" }, Date = DateTime.Today, Game = "Slots", Bet = 10, Gain = 0, IsCanceled = false },
                new DataAccess.Dbo.Stats { Id = 12, User = new DataAccess.Dbo.User { Username = "dave"    }, Date = DateTime.Today, Game = "Poker", Bet = 20, Gain = 5, IsCanceled = false }
            };
            var paged = new PaginatedStats(statsList, totalPages: 3);
            _statsRepoMock
                .Setup(r => r.Get("game_asc", "", 1, false))
                .ReturnsAsync(paged);

            // Act
            var backOffice = await _statsService.GetBackOffice("game_asc", "", 1);

            // Assert
            backOffice.GameHistory.Should().HaveCount(2);
            backOffice.HasPrevious.Should().BeFalse(); // pageIndex = 1
            backOffice.HasNext.Should().BeTrue();      // TotalPages > pageIndex
            backOffice.GameHistory.Should().Contain(m =>
                m.Username == "charlie" && m.Game == "Slots"
            );
        }

        [Fact]
        public async Task Cancel_ShouldMarkStatCanceledAndCallAddMoney()
        {
            // Arrange
            var stat = new DataAccess.Dbo.Stats { Id = 5, UserId = 9, Bet = 100, Gain = 30, IsCanceled = false };
            _statsRepoMock
                .Setup(r => r.GetOneById(5, ""))
                .ReturnsAsync(stat);

            var user = new User.Models.User(9, "eve", 0, 0, "User");
            _userServiceMock
                .Setup(s => s.GetUser(9))
                .ReturnsAsync(user);

            _userServiceMock
                .Setup(s => s.AddMoney("eve", 70))
                .Returns(Task.FromResult(70L));

            var updated = new DataAccess.Dbo.Stats { Id = 5, UserId = 9, Bet = 100, Gain = 30, IsCanceled = true };
            _statsRepoMock
                .Setup(r => r.Update(It.Is<DataAccess.Dbo.Stats>(st => st.IsCanceled)))
                .ReturnsAsync(updated);

            // Act
            var result = await _statsService.Cancel(5);

            // Assert
            result.IsCanceled.Should().BeTrue();
            _userServiceMock.Verify(s => s.AddMoney("eve", 70), Times.Once);
            _statsRepoMock.Verify(r => r.Update(It.Is<DataAccess.Dbo.Stats>(st => st.IsCanceled)), Times.Once);
        }

        [Fact]
        public async Task GetUserStats_ShouldReturnSummaryDirectly()
        {
            // Arrange
            var summary = new UserStatsSummary
            {
                HighestGain        = 300,
                NumberOfGames      = 10,
                TotalWon           = 500,
                TotalLost          = 200,
                GamesPlayedPerGame = new Dictionary<string,int>(),
                GamesPlayedPerDay  = new Dictionary<DateTime,int>()
            };
            _statsRepoMock
                .Setup(r => r.GetStats(15))
                .ReturnsAsync(summary);

            // Act
            var result = await _statsService.GetUserStats(15);

            // Assert
            result.Should().BeEquivalentTo(summary);
        }
    }
}
