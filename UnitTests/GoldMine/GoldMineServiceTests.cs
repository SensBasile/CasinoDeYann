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
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IStatsService> _statsServiceMock;
        private readonly GoldMineService _goldMineService;

        public GoldMineServiceTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _statsServiceMock = new Mock<IStatsService>();
            _goldMineService = new GoldMineService(_userServiceMock.Object, _statsServiceMock.Object);
        }

        [Fact]
        public async Task Mine_ShouldReturnFalse_WhenUserHasEnoughMoney()
        {
            var username = "testuser";
            var user = new CasinoDeYann.Services.User.Models.User(1, username, 100, 100, "User");

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
            var user = new CasinoDeYann.Services.User.Models.User(1, username, 50,  100, "User");
            var updatedUser = new CasinoDeYann.Services.User.Models.User(1,  username, 51, 100, "User");

            _userServiceMock.Setup(s => s.GetUser(username)).ReturnsAsync(user);
            _userServiceMock.Setup(s => s.AddMoney(username, 1)).ReturnsAsync(updatedUser.Money);
            _userServiceMock.Setup(s => s.AddExp(username, 1)).ReturnsAsync(updatedUser);
            _statsServiceMock.Setup(s => s.Create(It.IsAny<GameHistoryEntryModel>())).Returns(Task.FromResult<GameHistoryEntryModel>(null));

            var result = await _goldMineService.Mine(username);

            Assert.True(result.ToResponse().Credited);

            _userServiceMock.Verify(s => s.AddMoney(username, 5), Times.Once);
            _userServiceMock.Verify(s => s.AddExp(username, 5), Times.Once);
            _statsServiceMock.Verify(s => s.Create(It.Is<GameHistoryEntryModel>(entry =>
                entry.Username == username &&
                entry.Game == "Gold Mine" &&
                entry.Bet == 0 &&
                entry.Gain == 5
            )), Times.Once);
        }
    }
}
