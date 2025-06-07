using CasinoDeYann.Services;
using CasinoDeYann.Services.GoldMineService;
using CasinoDeYann.Services.Stats;
using CasinoDeYann.Services.Stats.Models;
using CasinoDeYann.Services.User;
using Moq;

namespace UnitTests.GoldMine
{
    public class GoldMineServiceTests
    {
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<StatsService> _statsServiceMock;
        private readonly GoldMineService _goldMineService;

        public GoldMineServiceTests()
        {
            _userServiceMock = new Mock<UserService>(null, null); // On injecte null car on ne teste pas UserService ici
            _statsServiceMock = new Mock<StatsService>();
            _goldMineService = new GoldMineService(_userServiceMock.Object, _statsServiceMock.Object);
        }

        [Fact]
        public async Task Mine_ShouldReturnFalse_WhenUserHasEnoughMoney()
        {
            var username = "testuser";
            var user = new CasinoDeYann.DataAccess.Dbo.User { Username = username, Money = 100 };

            _userServiceMock.Setup(s => s.GetUser(username)).ReturnsAsync(user);

            var result = await _goldMineService.Mine(username);

            Assert.False(result.ToResponse().Credited);

            _userServiceMock.Verify(s => s.AddMoney(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
            _userServiceMock.Verify(s => s.AddExp(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
            _statsServiceMock.Verify(s => s.Create(It.IsAny<GameHistoryEntryModel>()), Times.Never);
        }

        [Fact]
        public async Task Mine_ShouldCreditUser_WhenUserHasLessThan100Money()
        {
            var username = "testuser";
            var user = new CasinoDeYann.DataAccess.Dbo.User { Username = username, Money = 50 };
            var updatedUser = new CasinoDeYann.DataAccess.Dbo.User { Username = username, Money = 51 };

            _userServiceMock.Setup(s => s.GetUser(username)).ReturnsAsync(user);
            _userServiceMock.Setup(s => s.AddMoney(username, 1)).ReturnsAsync(updatedUser);
            _userServiceMock.Setup(s => s.AddExp(username, 1)).ReturnsAsync(updatedUser);
            _statsServiceMock.Setup(s => s.Create(It.IsAny<GameHistoryEntryModel>())).Returns((Task<GameHistoryEntryModel>)Task.CompletedTask);

            var result = await _goldMineService.Mine(username);

            Assert.True(result.ToResponse().Credited);

            _userServiceMock.Verify(s => s.AddMoney(username, 1), Times.Once);
            _userServiceMock.Verify(s => s.AddExp(username, 1), Times.Once);
            _statsServiceMock.Verify(s => s.Create(It.Is<GameHistoryEntryModel>(entry =>
                entry.Username == username &&
                entry.Game == "Gold Mine" &&
                entry.Bet == 0 &&
                entry.Gain == 1
            )), Times.Once);
        }
    }
}
