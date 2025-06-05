using CasinoDeYann.Src.Controllers.Roulette.DTOs;
using CasinoDeYann.Src.Services.Roulette;
using CasinoDeYann.Src.Services.Roulette.Models;
using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Src.Controllers.Roulette;

[ApiController]
[Route("api/[controller]")]
public class RouletteController(RouletteService rouletteService) : ControllerBase
{

    // GET /api/roulette/play/
    [HttpPost("play/")]
    public async Task<IActionResult> Play([FromBody] RouletteRequest bets)
    {
        if (User.Identity == null || User.Identity.Name == null)
        {
            return Unauthorized();
        }

        if (bets.Singles != null)
        {
            for (var i = 0; i < bets.Singles.Length; i++)
            {
                if (bets.Singles[i].Number < 0 || bets.Singles[i].Number > 36)
                    return BadRequest("Le numéro doit être compris entre 0 et 36.");
            }
        }
        
        RouletteModel res = await rouletteService.play(User.Identity.Name, bets);

        return Ok(new RouletteResponse(
            res.WinningNumber,
            res.Gain,
            res.Message,
            res.Money
        ));
    }
}