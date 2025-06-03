using CasinoDeYann.Api.Controllers.GoldMine.Responses;
using CasinoDeYann.Api.DataAccess.Interfaces;
using CasinoDeYann.Api.Services.GoldMineService.Models;
using CasinoDeYann.Api.Services.SlotMachine.Models;

namespace CasinoDeYann.Api.Services.GoldMineService;

public class GoldMineService(IUsersRepository usersRepository)
{
    private readonly int mineValue = 1;

    public async Task<GoldMineModel> Mine(string userName)
    {
        var callingUser = usersRepository.GetOneByName(userName);
        if (callingUser.Money >= 100) throw new BadHttpRequestException("You have too much money to mine");
        callingUser = await usersRepository.AddMoney(callingUser.Username, mineValue);

        return new GoldMineModel(true);
    }
}