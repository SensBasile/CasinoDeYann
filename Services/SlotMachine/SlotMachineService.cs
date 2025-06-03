using CasinoDeYann.DataAccess.Interfaces;
using CasinoDeYann.Services.SlotMachine.DTOs;

namespace CasinoDeYann.Services.SlotMachine;

public class SlotMachineService
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
    
    private readonly IUsersRepository _usersRepository;
    private readonly Random _random = new();


    public SlotMachineService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }
    
    public async Task<SlotMachineDTO> Play(string userName)
    {
        var grid = new List<int[]>();
        
        for (int i = 0; i < h; i++)
        {
            var row = new List<int>();
            for (int j = 0; j < w; j++)
            {
                row.Add(_random.Next(_symbols.Length));
            }
            grid.Add(row.ToArray());
        }

        var callingUser = _usersRepository.GetOneByName(userName);
        var gain = ComputeGain(grid.ToArray());
        callingUser = await _usersRepository.AddMoney(callingUser.Username, gain);
        return new SlotMachineDTO(
            grid.ToArray(), 
            callingUser.Money,
            gain > 0 ? "Bravo vous avez gagné !!!" : "Retentez votre chance"
            );
    }

    private int ComputeGain(int[][] toArray)
    {
        return 100;
    }
}