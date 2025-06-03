using CasinoDeYann.Services.SlotMachine;
using CasinoDeYann.Services.SlotMachine.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Controllers.SlotMachine;

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

        SlotMachineDTO res = await slotMachineService.Play(User.Identity.Name);
        return Ok(res.ToResponse());
    }
}