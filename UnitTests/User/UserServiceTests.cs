using CasinoDeYann.DataAccess.Dbo;
using CasinoDeYann.DataAccess.Interfaces;
using CasinoDeYann.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace UnitTests.User;

public class UserServiceTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IStatsRepository> _statsRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _statsRepositoryMock = new Mock<IStatsRepository>();
        _userService = new UserService(_usersRepositoryMock.Object, _statsRepositoryMock.Object);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUser()
    {
        var username = "testuser";
        var user = new CasinoDeYann.DataAccess.Dbo.User { Username = username };
        _usersRepositoryMock.Setup(repo => repo.GetOneByName(username)).ReturnsAsync(user);

        var result = await _userService.GetUser(username);

        Assert.Equal(username, result.Username);
    }

    [Fact]
    public async Task Pay_ShouldDeductMoneyAndUpdateUser_WhenEnoughMoney()
    {
        var username = "testuser";
        var user = new CasinoDeYann.DataAccess.Dbo.User { Username = username, Money = 1000 };
        _usersRepositoryMock.Setup(repo => repo.GetOneByName(username)).ReturnsAsync(user);
        _usersRepositoryMock.Setup(repo => repo.Update(It.IsAny<CasinoDeYann.DataAccess.Dbo.User>())).ReturnsAsync((CasinoDeYann.DataAccess.Dbo.User u) => u);

        var result = await _userService.Pay(username, 500);

        Assert.Equal(500, result.Money);
        _usersRepositoryMock.Verify(repo => repo.Update(It.Is<CasinoDeYann.DataAccess.Dbo.User>(u => u.Money == 500)), Times.Once);
    }

    [Fact]
    public async Task Pay_ShouldThrowBadHttpRequestException_WhenNotEnoughMoney()
    {
        var username = "testuser";
        var user = new CasinoDeYann.DataAccess.Dbo.User { Username = username, Money = 100 };
        _usersRepositoryMock.Setup(repo => repo.GetOneByName(username)).ReturnsAsync(user);

        await Assert.ThrowsAsync<BadHttpRequestException>(() => _userService.Pay(username, 200));
    }

    [Fact]
    public async Task GetLeaderboard_ShouldReturnTop10UsersByMoney()
    {
        var users = Enumerable.Range(1, 15)
            .Select(i => new CasinoDeYann.DataAccess.Dbo.User { Username = $"user{i}", Money = i * 100 })
            .ToList();

        _usersRepositoryMock.Setup(repo => repo.Get("")).ReturnsAsync(users);

        var result = (await _userService.GetLeaderboard()).ToList();

        Assert.Equal(10, result.Count);
        Assert.True(result[0].Money >= result[1].Money);
    }

    [Fact]
    public async Task GetLeaderboard_ShouldReturnLessThan10_WhenLessUsersExist()
    {
        var users = Enumerable.Range(1, 5)
            .Select(i => new CasinoDeYann.DataAccess.Dbo.User { Username = $"user{i}", Money = i * 100 })
            .ToList();

        _usersRepositoryMock.Setup(repo => repo.Get("")).ReturnsAsync(users);

        var result = (await _userService.GetLeaderboard()).ToList();

        Assert.Equal(5, result.Count);
    }

    [Fact]
    public async Task GetStats_ShouldReturnUserStatsSummary()
    {
        var username = "testuser";
        var user = new CasinoDeYann.DataAccess.Dbo.User { Id = 1, Username = username };
        var statsSummary = new UserStatsSummary();

        _usersRepositoryMock.Setup(repo => repo.GetOneByName(username)).ReturnsAsync(user);
        _statsRepositoryMock.Setup(repo => repo.GetStats(user.Id)).Returns(statsSummary);

        var result = await _userService.GetStats(username);

        Assert.Equal(statsSummary, result);
    }

    [Fact]
    public async Task GetUserProfileAsync_ShouldReturnUserProfile()
    {
        var username = "testuser";
        var user = new CasinoDeYann.DataAccess.Dbo.User { Id = 1, Username = username, Xp = 5000, Money = 2000 };
        var history = new List<CasinoDeYann.DataAccess.Dbo.Stats>
        {
            new() { Date = DateTime.UtcNow, Game = "Roulette", Bet = 100, Gain = 200 }
        };
        var statsSummary = new UserStatsSummary
        {
            UserId = 1,
            History = history,
            HighestGain = 200,
            NumberOfGames = 10,
            TotalWon = 1000,
            TotalLost = 500,
            GamesPlayedPerGame = new Dictionary<string, int> { { "Roulette", 5 } },
            GamesPlayedPerDay = new Dictionary<DateTime, int> { { DateTime.Today, 2 } }
        };

        _usersRepositoryMock.Setup(repo => repo.GetOneByName(username)).ReturnsAsync(user);
        _statsRepositoryMock.Setup(repo => repo.GetStats(user.Id)).Returns(statsSummary);

        var result = await _userService.GetUserProfileAsync(username);

        Assert.Equal(user.Xp / 1000, result.Level);
        Assert.Equal(user.Money, result.Balance);
        Assert.Single(result.History);
        Assert.Equal(statsSummary.HighestGain, result.HighestGain);
        Assert.Equal(statsSummary.NumberOfGames, result.NumberOfGames);
        Assert.Equal(statsSummary.TotalWon, result.TotalWon);
        Assert.Equal(statsSummary.TotalLost, result.TotalLost);
        Assert.Equal(statsSummary.GamesPlayedPerGame, result.GamesPlayedPerGame);
        Assert.Equal(statsSummary.GamesPlayedPerDay, result.GamesPlayedPerDay);
    }

    [Fact]
    public async Task DeleteAccountAsync_ShouldReturnTrue_WhenDeleteSuccessful()
    {
        var username = "testuser";
        _usersRepositoryMock.Setup(repo => repo.DeleteOneByName(username)).ReturnsAsync(true);

        var result = await _userService.DeleteAccountAsync(username);

        Assert.True(result);
    }

    [Fact]
    public async Task AddMoney_ShouldIncreaseUserMoney()
    {
        var username = "testuser";
        var user = new CasinoDeYann.DataAccess.Dbo.User { Username = username, Money = 1000 };
        _usersRepositoryMock.Setup(repo => repo.GetOneByName(username)).ReturnsAsync(user);
        _usersRepositoryMock.Setup(repo => repo.Update(It.IsAny<CasinoDeYann.DataAccess.Dbo.User>())).ReturnsAsync((CasinoDeYann.DataAccess.Dbo.User u) => u);

        var result = await _userService.AddMoney(username, 500);

        Assert.Equal(1500, result.Money);
    }

    [Fact]
    public async Task AddExp_ShouldIncreaseUserXp()
    {
        var username = "testuser";
        var user = new CasinoDeYann.DataAccess.Dbo.User { Username = username, Xp = 2000 };
        _usersRepositoryMock.Setup(repo => repo.GetOneByName(username)).ReturnsAsync(user);
        _usersRepositoryMock.Setup(repo => repo.Update(It.IsAny<CasinoDeYann.DataAccess.Dbo.User>())).ReturnsAsync((CasinoDeYann.DataAccess.Dbo.User u) => u);

        var result = await _userService.AddExp(username, 1000);

        Assert.Equal(3000, result.Xp);
    }

    [Fact]
    public async Task ChangeRole_ShouldUpdateUserRole()
    {
        var username = "testuser";
        var user = new CasinoDeYann.DataAccess.Dbo.User { Username = username, Role = "User" };
        _usersRepositoryMock.Setup(repo => repo.GetOneByName(username)).ReturnsAsync(user);
        _usersRepositoryMock.Setup(repo => repo.Update(It.IsAny<CasinoDeYann.DataAccess.Dbo.User>())).ReturnsAsync((CasinoDeYann.DataAccess.Dbo.User u) => u);

        var newRole = "Admin";

        var result = await _userService.ChangeRole(username, newRole);

        Assert.Equal(newRole, result.Role);
    }
}