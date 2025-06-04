using CasinoDeYann.Api.Controllers.GoldMine.Responses;
using CasinoDeYann.Api.DataAccess.Interfaces;
using CasinoDeYann.Api.Services.GoldMineService.Models;
using CasinoDeYann.Api.Services.SlotMachine.Models;
using CasinoDeYann.Api.Services.Stats;

namespace CasinoDeYann.Api.Services.GoldMineService;

public class GoldMineService(IUsersRepository usersRepository, StatsService statsService)
{
    private readonly int mineValue = 1;

    public async Task<GoldMineModel> Mine(string userName)
    {
        var callingUser = await usersRepository.GetOneByName(userName);
        if (callingUser.Money >= 100) return new GoldMineModel(false);
        callingUser = await usersRepository.AddMoney(callingUser.Username, mineValue);
        
        await statsService.Create(new GameHistoryEntryModel(callingUser.Id, "Gold Mine", 0, mineValue));

        return new GoldMineModel(true);
    }
}