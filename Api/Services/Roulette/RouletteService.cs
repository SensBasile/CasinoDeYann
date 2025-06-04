using CasinoDeYann.Api.Controllers.Roulette.DTOs;
using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.DataAccess.Interfaces;
using CasinoDeYann.Api.Services.Roulette.Models;

namespace CasinoDeYann.Api.Services.Roulette;

public class RouletteService(UsersService usersService)
{
    private readonly Random _random = new();

    public async Task<RouletteModel> play(string userName, RouletteRequest bets)
    {
        long totalBet = getTotalBetValue(bets);
        if (! await usersService.Pay(userName, totalBet)) 
            throw new BadHttpRequestException("You don't have enough money");
        
        int winningNumber = _random.Next(0, 37);

        long gain = ComputeGains(bets, winningNumber);

        return new RouletteModel(
            winningNumber,
            gain,
            gain > totalBet ? $"Bravo vous avez remporté {gain - totalBet} yanns" : $"Dommage vous avez perdu {totalBet - gain} yanns"
        );
    }

    private long getTotalBetValue(RouletteRequest bets)
    {
        return 0;
    }

    private long ComputeGains(RouletteRequest bets, int winningNumber)
    {
        return 100;
    }
}