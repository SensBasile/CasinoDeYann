using System.Security.Claims;
using CasinoDeYann.Controllers.SlotMachine;
using CasinoDeYann.Services.SlotMachine;
using CasinoDeYann.Services.SlotMachine.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.SlotMachine
{
    public class SlotMachineControllerTests
    {
        private readonly Mock<ISlotMachineService> _serviceMock;
        private readonly SlotMachineController _controller;

        public SlotMachineControllerTests()
        {
            _serviceMock = new Mock<ISlotMachineService>(MockBehavior.Strict);
            _controller = new SlotMachineController(_serviceMock.Object);
        }

        private void SetUser(string? name)
        {
            var identity = name is null
                ? new ClaimsIdentity()  // no Name
                : new ClaimsIdentity(new[]
                    { new Claim(ClaimTypes.Name, name) }, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(1001)]
        public async Task Play_InvalidBet_ReturnsBadRequest(int badBet)
        {
            // Arrange: authenticated user
            SetUser("alice");

            // Act
            var result = await _controller.Play(badBet);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Play_UnauthenticatedUser_ReturnsUnauthorized()
        {
            // Arrange: no user identity
            SetUser(null);

            // Act
            var result = await _controller.Play(10);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Play_ValidRequest_CallsServiceAndReturnsOk()
        {
            // Arrange
            const string userName = "bob";
            const int bet = 50;

            // fake model returned by the service
            var fakeModel = new SlotMachineModel(
                grid: new int[5][],
                patterns: new bool[5][],
                gain: 123,
                money: 456,
                message: "You win!"
            );

            // setup service to return it
            _serviceMock
                .Setup(s => s.Play(userName, bet))
                .ReturnsAsync(fakeModel);

            SetUser(userName);

            // Act
            var actionResult = await _controller.Play(bet);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            // the controller does model.ToResponse(), so we check the properties round-trip
            dynamic response = okResult.Value!;

            Assert.Equal(fakeModel.Grid, (int[][])response.Grid);
            Assert.Equal(fakeModel.Patterns, (bool[][])response.Patterns);
            Assert.Equal(fakeModel.Gain, (long)response.Gain);
            Assert.Equal(fakeModel.UserMoney, (long)response.Money);
            Assert.Equal(fakeModel.Message, (string)response.Message);

            _serviceMock.Verify(s => s.Play(userName, bet), Times.Once);
        }
    }
}
