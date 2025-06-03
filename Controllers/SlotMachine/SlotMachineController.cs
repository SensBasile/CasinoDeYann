using CasinoDeYann.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Controllers.SlotMachine;

[Route("api/[controller]")] // SlotMachine
[ApiController]
public class SlotMachineController : Controller
{
    private readonly int w = 5;
    private readonly int h = 5;
    
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
    
    private readonly IUsersRepository usersRepository;
    
    public SlotMachineController(IUsersRepository usersRepository)
    {
        this.usersRepository = usersRepository;
    }
    
    private readonly Random _random = new();

    [HttpPost("play")]
    public IActionResult Play()
    {
        var grid = new List<int[]>();

        for (int i = 0; i < h; i++)
        if (User.Identity == null || User.Identity.Name == null)
        {
            return Unauthorized();
        }

        var callingUser = usersRepository.GetOneByName(User.Identity.Name);
        
        for (int i = 0; i < 3; i++)
        {
            var row = new List<int>();
            for (int j = 0; j < w; j++)
            {
                row.Add(_random.Next(_symbols.Length));
            }
            grid.Add(row.ToArray());
        }

        var gain = ComputeGain(grid.ToArray());
        
        usersRepository.AddMoney(callingUser.Username, gain);
        

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
        return 100; // stonks
    }
}