using CasinoDeYann.Api.Controllers.GoldMine.Responses;
using CasinoDeYann.Api.DataAccess.Interfaces;
using CasinoDeYann.Api.Services.GoldMineService.Models;
using CasinoDeYann.Api.Services.SlotMachine.Models;
using CasinoDeYann.Api.Services.Stats;

namespace CasinoDeYann.Api.Services.GoldMineService;

public class GoldMineService(UserService userService, StatsService statsService)
{
    private const int MineValue = 1;

    public async Task<GoldMineModel> Mine(string userName)
    {
        var callingUser = await userService.GetUser(userName);
        if (callingUser.Money >= 100) return new GoldMineModel(false);
        
        callingUser = await userService.AddMoney(callingUser.Username, MineValue);
        _ = userService.AddExp(callingUser.Username, MineValue);
        
        await statsService.Create(new GameHistoryEntryModel(callingUser.Id, DateTime.Now, "Gold Mine", 0, MineValue));

        return new GoldMineModel(true);
    }
}