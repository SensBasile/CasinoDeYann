using CasinoDeYann.Controllers.User;
using CasinoDeYann.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.User
{
    public class UserControllerTests
    {
        private readonly Mock<UserService> _userServiceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<UserService>(null, null);
            _controller = new UserController(_userServiceMock.Object);
        }

        [Fact]
        public async Task Get_ShouldReturnEmptyList_WhenQueryIsNullOrWhitespace()
        {
            var result = await _controller.Get(null) as OkObjectResult;
            var data = result?.Value as List<string>;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(data);
            Assert.Empty(data);

            result = await _controller.Get("") as OkObjectResult;
            data = result?.Value as List<string>;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(data);
            Assert.Empty(data);

            result = await _controller.Get("   ") as OkObjectResult;
            data = result?.Value as List<string>;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(data);
            Assert.Empty(data);
        }

        [Fact]
        public async Task Get_ShouldReturnSearchResults_WhenQueryIsValid()
        {
            var query = "test";
            var expectedResults = new List<string> { "user1", "user2" };

            _userServiceMock.Setup(s => s.Search(query)).ReturnsAsync(expectedResults);

            var result = await _controller.Get(query) as OkObjectResult;
            var data = result?.Value as List<string>;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(data);
            Assert.Equal(expectedResults, data);
        }

        [Fact]
        public async Task Get_ShouldReturnEmptyList_WhenSearchReturnsEmpty()
        {
            var query = "test";
            var expectedResults = new List<string>();

            _userServiceMock.Setup(s => s.Search(query)).ReturnsAsync(expectedResults);

            var result = await _controller.Get(query) as OkObjectResult;
            var data = result?.Value as List<string>;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(data);
            Assert.Empty(data);
        }
    }
}
