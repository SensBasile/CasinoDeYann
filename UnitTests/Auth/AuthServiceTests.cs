using System.Security.Claims;
using CasinoDeYann.DataAccess.Interfaces;
using CasinoDeYann.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Moq;

namespace UnitTests.Auth
{
    public class AuthServiceTests
    {
        private readonly Mock<IUsersRepository> _userRepoMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<IAuthenticationService> _authServiceMock;

        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUsersRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextMock = new Mock<HttpContext>();
            _authServiceMock = new Mock<IAuthenticationService>();

            // Configuration du contexte HTTP
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(_authServiceMock.Object);

            _httpContextMock.Setup(x => x.RequestServices)
                .Returns(serviceProviderMock.Object);

            _httpContextAccessorMock.Setup(x => x.HttpContext)
                .Returns(_httpContextMock.Object);

            _authService = new AuthService(_userRepoMock.Object, _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsTrueAndSignsIn()
        {
            // Arrange
            var username = "john";
            var password = "secret";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            _userRepoMock.Setup(r => r.GetOneByName(username)).ReturnsAsync(new CasinoDeYann.DataAccess.Dbo.User
            {
                Username = username,
                Password = hashedPassword,
                Role = "Admin"
            });

            _authServiceMock.Setup(a => a.SignInAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>())).Returns(Task.CompletedTask);

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            Assert.True(result);
            _authServiceMock.Verify(a => a.SignInAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<ClaimsPrincipal>(),
                null), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsFalse()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetOneByName("baduser")).ReturnsAsync((CasinoDeYann.DataAccess.Dbo.User?)null);

            // Act
            var result = await _authService.LoginAsync("baduser", "wrong");

            // Assert
            Assert.False(result);
            _authServiceMock.Verify(a => a.SignInAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()), Times.Never);
        }

        [Fact]
        public async Task LogoutAsync_CallsSignOut()
        {
            // Arrange
            _authServiceMock.Setup(a => a.SignOutAsync(
                It.IsAny<HttpContext>(),
                null,
                null)).Returns(Task.CompletedTask);

            // Act
            await _authService.LogoutAsync();

            // Assert
            _authServiceMock.Verify(a => a.SignOutAsync(
                It.IsAny<HttpContext>(),
                null,
                null), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_UserAlreadyExists_ReturnsFalse()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetOneByName("existingUser")).ReturnsAsync(new CasinoDeYann.DataAccess.Dbo.User());

            // Act
            var result = await _authService.RegisterAsync("existingUser", "pass", "User");

            // Assert
            Assert.False(result);
            _userRepoMock.Verify(r => r.Insert(It.IsAny<CasinoDeYann.DataAccess.Dbo.User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_RegistersAndLogsIn()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetOneByName("newUser")).ReturnsAsync((CasinoDeYann.DataAccess.Dbo.User?)null);
            _userRepoMock.Setup(r => r.Insert(It.IsAny<CasinoDeYann.DataAccess.Dbo.User>())) .ReturnsAsync((CasinoDeYann.DataAccess.Dbo.User u) =>new CasinoDeYann.DataAccess.Dbo.User { Id = 1,
                Username = u.Username,
                Password = u.Password,
                Money = u.Money,
                Role = u.Role,
                Xp = u.Xp});

            _authServiceMock.Setup(a => a.SignInAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<ClaimsPrincipal>(),
                null)).Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterAsync("newUser", "password123", "Player");

            // Assert
            Assert.True(result);
            _userRepoMock.Verify(r => r.Insert(It.Is<CasinoDeYann.DataAccess.Dbo.User>(u => u.Username == "newUser")), Times.Once);
        }
    }
}
