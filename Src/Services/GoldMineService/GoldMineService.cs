using CasinoDeYann.Services.GoldMineService.Models;
using CasinoDeYann.Services.Stats;
using CasinoDeYann.Services.Stats.Models;
using CasinoDeYann.Services.User;

namespace CasinoDeYann.Services.GoldMineService;

public class GoldMineService(IUserService userService, IStatsService statsService)
{
    private const int MineValue = 5;

    public virtual async Task<GoldMineModel> Mine(string userName)
    {
        var callingUser = await userService.GetUser(userName);
        if (callingUser == null || callingUser.Money >= 100) return new GoldMineModel(false);
        
        await userService.AddMoney(callingUser.Username, MineValue);
        _ = await userService.AddExp(callingUser.Username, MineValue);
        
        await statsService.Create(new GameHistoryEntryModel(-1, callingUser.Username, DateTime.Now, "Gold Mine", 0, MineValue, false));

        return new GoldMineModel(true);
    }
}