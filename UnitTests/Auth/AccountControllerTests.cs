using System.Security.Claims;
using CasinoDeYann.Controllers.Account;
using CasinoDeYann.DataAccess.Interfaces;
using CasinoDeYann.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Auth
{
    public class AccountControllerTests
    {
        private readonly Mock<IUsersRepository> _usersRepoMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<AuthenticationService> _authServiceMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _usersRepoMock = new Mock<IUsersRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextMock = new Mock<HttpContext>();

            // Simuler HttpContext.SignInAsync / SignOutAsync
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock.Setup(a => a.SignInAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>())).Returns(Task.CompletedTask);

            authServiceMock.Setup(a => a.SignOutAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<AuthenticationProperties>())).Returns(Task.CompletedTask);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            _httpContextMock.Setup(x => x.RequestServices).Returns(serviceProviderMock.Object);
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(_httpContextMock.Object);

            var authService = new AuthService(_usersRepoMock.Object, _httpContextAccessorMock.Object);
            _controller = new AccountController(authService);
        }

        [Fact]
        public void Login_Get_ReturnsViewResult()
        {
            var result = _controller.Login();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_Post_ValidCredentials_RedirectsToHomeIndex()
        {
            // Arrange
            var username = "validUser";
            var password = "validPass";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            _usersRepoMock.Setup(r => r.GetOneByName(username)).ReturnsAsync(new CasinoDeYann.DataAccess.Dbo.User
            {
                Username = username,
                Password = hashedPassword,
                Role = "Player"
            });

            // Act
            var result = await _controller.Login(username, password);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task Login_Post_InvalidCredentials_ReturnsViewWithError()
        {
            _usersRepoMock.Setup(r => r.GetOneByName("invalidUser"))
                .ReturnsAsync((CasinoDeYann.DataAccess.Dbo.User?)null);

            var result = await _controller.Login("invalidUser", "wrongPass");

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("Nom d'utilisateur ou mot de passe invalide", _controller.ViewBag.Error);
        }

        [Fact]
        public async Task Logout_RedirectsToLogin()
        {
            var result = await _controller.Logout();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
        }
    }
}
