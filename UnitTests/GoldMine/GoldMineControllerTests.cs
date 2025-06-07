using CasinoDeYann.Controllers.GoldMine;
using CasinoDeYann.Services.GoldMineService;
using CasinoDeYann.Services.GoldMineService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Security.Claims;
using CasinoDeYann.Controllers.GoldMine.Responses;
using Xunit;

namespace UnitTests.GoldMine
{
    public class GoldMineControllerTests
    {
        private readonly Mock<GoldMineService> _mockGoldMineService;
        private readonly IMemoryCache _memoryCache;

        public GoldMineControllerTests()
        {
            _mockGoldMineService = new Mock<GoldMineService>(null!, null!);
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        private static ClaimsPrincipal GetUser(string username)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }, "mock"));
        }

        private GoldMineController CreateController(string username)
        {
            var controller = new GoldMineController(_mockGoldMineService.Object, _memoryCache)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = GetUser(username)
                    }
                }
            };
            return controller;
        }

        [Fact]
        public async Task Index_ReturnsUnauthorized_IfUserIsNull()
        {
            var controller = new GoldMineController(_mockGoldMineService.Object, _memoryCache)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var result = await controller.Index();

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsTooManyRequests_IfInCooldown()
        {
            var username = "testuser";
            var controller = CreateController(username);

            // Simulate a previous call within cooldown
            _memoryCache.Set($"goldmine-cooldown-{username}", true, TimeSpan.FromSeconds(1));

            var result = await controller.Index();

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(429, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Index_ReturnsOk_IfNotInCooldown_AndCallsService()
        {
            var username = "testuser";
            var controller = CreateController(username);

            var expectedModel = new GoldMineModel(true);
            _mockGoldMineService
                .Setup(s => s.Mine(username))
                .ReturnsAsync(expectedModel);

            var result = await controller.Index();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<GoldMineResponse>(okResult.Value);
            Assert.True(returned.Credited);

            // Ensure cooldown is set
            Assert.True(_memoryCache.TryGetValue($"goldmine-cooldown-{username}", out _));
        }
    }
}
