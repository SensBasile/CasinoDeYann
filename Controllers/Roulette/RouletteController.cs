using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Controllers.Roulette;

[ApiController]
[Route("api/[controller]")]
public class RouletteController : ControllerBase
{
    private readonly Random _random = new();

    // GET /api/roulette/play/{playerNumber}
    [HttpGet("play/{playerNumber}")]
    public IActionResult Play(int playerNumber)
    {
        if (playerNumber < 0 || playerNumber > 36)
        {
            return BadRequest("Le numéro doit être compris entre 0 et 36.");
        }

        int winningNumber = _random.Next(0, 37); // 0 à 36 inclus
        bool isWin = playerNumber == winningNumber;

        return Ok(new
        {
            playerNumber,
            winningNumber,
            isWin,
            message = isWin
                ? "🎉 Félicitations, vous avez gagné !"
                : $"❌ Dommage ! Le numéro gagnant était {winningNumber}."
        });
    }
}