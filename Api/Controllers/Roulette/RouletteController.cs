using CasinoDeYann.Api.Controllers.Roulette.DTOs;
using CasinoDeYann.Api.Services.Roulette;
using CasinoDeYann.Api.Services.Roulette.Models;
using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Api.Controllers.Roulette;

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
                if (bets.Singles[i].Item1 < 0 || bets.Singles[i].Item2 > 36)
                    return BadRequest("Le numéro doit être compris entre 0 et 36.");
            }
        }
        
        RouletteModel res = await rouletteService.play(User.Identity.Name, bets);

        return Ok(new RouletteResponse(
            res.WinningNumber,
            res.Gain,
            res.Message
        ));
    }
}