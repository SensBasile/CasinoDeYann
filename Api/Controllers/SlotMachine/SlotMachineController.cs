using CasinoDeYann.Api.Services.SlotMachine;
using CasinoDeYann.Api.Services.SlotMachine.Models;
using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Api.Controllers.SlotMachine;

[Route("api/[controller]")] // SlotMachine
[ApiController]
public class SlotMachineController(SlotMachineService slotMachineService) : Controller
{
    [HttpPost("play/{bet}")]
    public async Task<IActionResult> Play(int bet)
    {
        if (User.Identity == null || User.Identity.Name == null)
        {
            return Unauthorized();
        }

        if (bet <= 0 || bet > 1000)
        {
            return BadRequest();
        }

        SlotMachineModel res = await slotMachineService.Play(User.Identity.Name, bet);
        return Ok(res.ToResponse());
    }
}