using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Controllers.SlotMachine;

[Route("api/[controller]")] // SlotMachine
[ApiController]
public class SlotMachineController : Controller
{
    private readonly string[] _symbols = [
    "/assets/SlotMachine/bell/bell.png",
    "/assets/SlotMachine/cherry/cherry.png",
    "/assets/SlotMachine/diamond/diamond.png",
    "/assets/SlotMachine/heart/heart.png",
    "/assets/SlotMachine/horseshoe/horseshoe.png",
    "/assets/SlotMachine/seven/seven.png",
    "/assets/SlotMachine/watermelon/watermelon.png",
    "/assets/SlotMachine/wildcard/wildcard.png",
    "/assets/SlotMachine/yann/yann.png",
    ];
    
    private readonly Random _random = new();

    [HttpPost("play")]
    public IActionResult Play()
    {
        var grid = new List<int[]>();

        for (int i = 0; i < 5; i++)
        {
            var row = new List<int>();
            for (int j = 0; j < 5; j++)
            {
                row.Add(_random.Next(_symbols.Length));
            }
            grid.Add(row.ToArray());
        }

        var gain = ComputeGain(grid.ToArray());

        var response = new
        {
            grid,
            message = gain > 0 ? "Bravo vous avez gagné !!!" : "Retentez votre chance",
        };

        return Ok(response);
    }

    private int ComputeGain(int[][] grid)
    {
        // TODO
        return 0;
    }
}