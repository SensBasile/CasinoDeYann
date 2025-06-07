using AutoMapper;
using CasinoDeYann.DataAccess.Dbo;
using CasinoDeYann.DataAccess.Interfaces;
using CasinoDeYann.Services;
using CasinoDeYann.Services.User;
using Microsoft.AspNetCore.Http;
using Moq;

namespace UnitTests.User;

public class UserServiceTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IStatsRepository> _statsRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _usersRepositoryMock
            .Setup(repo => repo.Update(It.IsAny<CasinoDeYann.DataAccess.Dbo.User>()))
            .ReturnsAsync((CasinoDeYann.DataAccess.Dbo.User u) =>
            {
                u.Id = 123;
                return u;
            });
        
        _mapperMock = new Mock<IMapper>();
        _statsRepositoryMock = new Mock<IStatsRepository>();
        _userService = new UserService(_usersRepositoryMock.Object, _mapperMock.Object);
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

        var result = await _userService.AddMoney(username, 500);

        Assert.Equal(1500, result);
    }

    [Fact]
    public async Task AddExp_ShouldIncreaseUserXp()
    {
        var username = "testuser";
        var user = new CasinoDeYann.DataAccess.Dbo.User { Username = username, Xp = 2000 };
        _usersRepositoryMock.Setup(repo => repo.GetOneByName(username)).ReturnsAsync(user);

        var result = await _userService.AddExp(username, 1000);

        Assert.Equal(3000, result.Xp);
    }

    [Fact]
    public async Task ChangeRole_ShouldUpdateUserRole()
    {
        var username = "testuser";
        var user = new CasinoDeYann.DataAccess.Dbo.User { Username = username, Role = "User" };
        _usersRepositoryMock.Setup(repo => repo.GetOneByName(username)).ReturnsAsync(user);

        var newRole = "Admin";

        var result = await _userService.ChangeRole(username, newRole);

        Assert.Equal(newRole, result.Role);
    }
}