using System.Security.Claims;
using CasinoDeYann.Controllers.Roulette;
using CasinoDeYann.Controllers.Roulette.DTOs;
using CasinoDeYann.Services.Roulette;
using CasinoDeYann.Services.Roulette.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Roulette
{
    public class RouletteControllerTests
    {
        [Fact]
        public async Task Play_Unauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            var serviceMock = new Mock<IRouletteService>(MockBehavior.Strict /*, pass ctor args if any */);
            var controller = new RouletteController(serviceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext() // no User.Identity
                }
            };
            var request = new RouletteRequest();

            // Act
            var result = await controller.Play(request);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(37)]
        public async Task Play_InvalidSingleNumber_ReturnsBadRequest(int invalidNumber)
        {
            // Arrange
            var username = "bob";
            var serviceMock = new Mock<IRouletteService>(MockBehavior.Strict);
            var controller = new RouletteController(serviceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(
                            new[] { new Claim(ClaimTypes.Name, username) }, "TestAuth"))
                    }
                }
            };

            var bets = new RouletteRequest
            {
                Singles = new[]
                {
                    new SingleBetRequest { Number = invalidNumber, Amount = 100 }
                }
            };

            // Act
            var result = await controller.Play(bets);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                  .Which.Value.Should().Be("Le numéro doit être compris entre 0 et 36.");
        }

        [Fact]
        public async Task Play_ValidRequest_ReturnsOkWithResponse()
        {
            // Arrange
            var username = "bob";
            var bets = new RouletteRequest
            {
                Singles = new[]
                {
                    new SingleBetRequest { Number = 7, Amount = 100 }
                }
            };

            var expectedModel = new RouletteModel
            (
                7,
                200,
                "You win!",
                2600
            );

            var serviceMock = new Mock<IRouletteService>(MockBehavior.Strict /*, ctor args */);
            serviceMock
                .Setup(s => s.play(username, bets))
                .ReturnsAsync(expectedModel);

            var controller = new RouletteController(serviceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(
                            new[] { new Claim(ClaimTypes.Name, username) }, "TestAuth"))
                    }
                }
            };

            // Act
            var result = await controller.Play(bets);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var resp = ok.Value.Should().BeOfType<RouletteResponse>().Subject;

            resp.WinningNumber.Should().Be(expectedModel.WinningNumber);
            resp.Gain         .Should().Be(expectedModel.Gain);
            resp.Message      .Should().Be(expectedModel.Message);
            resp.Money        .Should().Be(expectedModel.Money);

            serviceMock.Verify(s => s.play(username, bets), Times.Once);
        }
    }
}
