using CasinoDeYann.Controllers.HorseRace.DTOs;
using CasinoDeYann.Services.HorseRace;
using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Controllers.HorseRace;

[Route("api/[controller]")] // HorseRace
[ApiController]
public class HorseRaceController(HorseRaceService horseRaceService) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Play(HorseRaceRequest bets)
    {
        if (User.Identity == null || User.Identity.Name == null)
        {
            return Unauthorized();
        }
        
        var res = await horseRaceService.Play(User.Identity.Name, bets);
        return Ok(res.ToResponse());
    }
}
