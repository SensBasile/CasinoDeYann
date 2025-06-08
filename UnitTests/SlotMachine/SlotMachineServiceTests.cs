using System.Reflection;
using CasinoDeYann.Services.SlotMachine;
using CasinoDeYann.Services.SlotMachine.Models;
using CasinoDeYann.Services.Stats;
using CasinoDeYann.Services.Stats.Models;
using CasinoDeYann.Services.User;
using Moq;

namespace UnitTests.SlotMachine
{
    public class SlotMachineServiceTests
    {
        private readonly Mock<IUserService> _userService;
        private readonly Mock<IStatsService> _statsService;
        private readonly SlotMachineService _service;
        private readonly BindingFlags _flags = BindingFlags.NonPublic | BindingFlags.Instance;

        public SlotMachineServiceTests()
        {
            _userService = new Mock<IUserService>();
            _statsService = new Mock<IStatsService>();

            // By default, Pay returns a user with 100 money
            _userService
                .Setup(u => u.Pay(It.IsAny<string>(), It.IsAny<long>()))
                .ReturnsAsync((string name, long bet) =>
                    new CasinoDeYann.Services.User.Models.User(1, name, 100, 1000, "User"));

            // AddExp always succeeds
            _userService
                .Setup(u => u.AddExp(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Task.FromResult<CasinoDeYann.Services.User.Models.User>(null));

            // AddMoney returns a fixed new balance
            _userService
                .Setup(u => u.AddMoney(It.IsAny<string>(), It.IsAny<long>()))
                .ReturnsAsync(200L);

            // Stats Create always succeeds
            _statsService
                .Setup(s => s.Create(It.IsAny<GameHistoryEntryModel>()))
                .Returns(Task.FromResult<CasinoDeYann.Services.Stats.Models.GameHistoryEntryModel>(null));

            _service = new SlotMachineService(_userService.Object, _statsService.Object);

            // Seed the private Random to make grid deterministic (optional)
            typeof(SlotMachineService)
                .GetField("_random", _flags)
                .SetValue(_service, new Random(0));
        }

        [Theory]
        [InlineData(new[] { 0, 0, 0, 1, 2 }, 0)]    // exactly 3 in a row → alignCoeff[0] * symbolCoeff[0] = 1.3 * 1.3 = 1.69
        [InlineData(new[] { 5, 5, 5, 5, 1 }, 1)]    // exactly 4 in a row → alignCoeff[1] * symbolCoeff[5] = 1.5 * 2.5 = 3.75
        [InlineData(new[] { 2, 2, 2, 2, 2 }, 2)]    // exactly 5 in a row → alignCoeff[2] * symbolCoeff[2] = 2.0 * 2.0 = 4.0
        public void CheckAlignRow_CorrectCoeffAndPattern(int[] row, int expectedCoeffIndex)
        {
            // arrange
            var patterns = new bool[5];
            var mi = typeof(SlotMachineService)
                        .GetMethod("CheckAlignRow", _flags);

            // act
            var rawScore = (float)mi.Invoke(_service, new object[] { row, patterns });
            
            // get private fields
            var alignCoeff = (float[])typeof(SlotMachineService)
                .GetField("_alignCoeff", _flags)
                ?.GetValue(_service)!;
            var symbolsCoeff = (float[])typeof(SlotMachineService)
                .GetField("_symbolsCoeff", _flags)
                ?.GetValue(_service)!;

            var expected = alignCoeff[expectedCoeffIndex] * symbolsCoeff[row[0]];

            // assert score within epsilon
            Assert.InRange(rawScore, expected - 0.0001f, expected + 0.0001f);

            // only the first exactly (MinAlign + expectedCoeffIndex) positions are marked
            var expectedMarks = MinAlign + expectedCoeffIndex; // MinAlign is 3
            Assert.True(patterns.Take(expectedMarks).All(x => x),
                        "The first aligned symbols should be marked true");
            Assert.True(patterns.Skip(expectedMarks).All(x => !x),
                        "Positions beyond the alignment should remain false");
        }

        [Fact]
        public void ComputeGain_SimpleThreeAlignInFirstRow_ReturnsExpectedGain()
        {
            // arrange
            var grid = new int[5][]
            {
                new[]{ 0, 0, 0, 1, 2 },   // only this row has 3 aligned 0's
                new[]{ 1, 2, 3, 4, 5 },
                new[]{ 2, 3, 4, 5, 6 },
                new[]{ 3, 4, 5, 6, 7 },
                new[]{ 4, 5, 6, 7, 8 }
            };
            var patterns = Enumerable.Range(0,5).Select(_=>new bool[5]).ToArray();

            var mi = typeof(SlotMachineService)
                        .GetMethod("ComputeGain", _flags);

            long bet = 10;
            
            // act
            var gain = (long)mi.Invoke(_service, new object[] { grid, bet, patterns });
            
            Assert.Equal(30L, gain);
        }

        [Fact]
        public async Task Play_InvokesUserAndStatsServicesAndReturnsModel()
        {
            // arrange
            const string userName = "alice";
            const int bet = 20;

            // act
            var model = await _service.Play(userName, bet);

            // assert interactions
            _userService.Verify(u => u.Pay(userName, bet), Times.Once);
            _userService.Verify(u => u.AddExp(userName, It.IsAny<long>()), Times.AtLeastOnce);
            _userService.Verify(u => u.AddMoney(userName, It.IsAny<long>()), Times.Once);
            _statsService.Verify(s => s.Create(It.Is<GameHistoryEntryModel>(
                gh => gh.Username == userName &&
                      gh.Game == "Slot Machine" &&
                      gh.Bet == bet
            )), Times.Once);

            // assert returned model shape
            Assert.NotNull(model);
            Assert.Equal(5, model.Grid.Length);
            Assert.All(model.Grid, row => Assert.Equal(5, row.Length));
            Assert.Equal(5, model.Patterns.Length);
            Assert.All(model.Patterns, row => Assert.Equal(5, row.Length));

            // user money comes from AddMoney stub
            Assert.Equal(100, model.UserMoney);

            // message is one of the two expected
            Assert.Contains(model.Message, new[]
            {
                "Bravo vous avez gagné !!!",
                "Retentez votre chance"
            });
        }

        private const int MinAlign = 3;
    }
}
