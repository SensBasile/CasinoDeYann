using CasinoDeYann.Services.GoldMineService.Models;
using CasinoDeYann.Services.Stats;

namespace CasinoDeYann.Services.GoldMineService;

public class GoldMineService(UserService userService, StatsService statsService)
{
    private const int MineValue = 1;

    public async Task<GoldMineModel> Mine(string userName)
    {
        var callingUser = await userService.GetUser(userName);
        if (callingUser.Money >= 100) return new GoldMineModel(false);
        
        callingUser = await userService.AddMoney(callingUser.Username, MineValue);
        _ = userService.AddExp(callingUser.Username, MineValue);
        
        await statsService.Create(new GameHistoryEntryModel(-1, callingUser.Username, DateTime.Now, "Gold Mine", 0, MineValue, false));

        return new GoldMineModel(true);
    }
}