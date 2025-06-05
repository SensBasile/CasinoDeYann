using CasinoDeYann.Api.Controllers.Roulette.DTOs;
using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.DataAccess.Interfaces;
using CasinoDeYann.Api.Services.Roulette.Models;

namespace CasinoDeYann.Api.Services.Roulette;

public class RouletteService(UserService userService)
{
    private readonly Random _random = new();
    
    private static readonly HashSet<int> RedNumbers = new()
    {
        1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36
    };

    public async Task<RouletteModel> play(string userName, RouletteRequest bets)
    {
        long totalBet = getTotalBetValue(bets);
        User user = await userService.Pay(userName, totalBet);
        
        _ = await userService.AddExp(userName, totalBet / 100 + 10);
        
        int winningNumber = _random.Next(0, 37);

        long gain = ComputeGains(bets, winningNumber);

        user = await userService.AddMoney(userName, gain);
        _ = await userService.AddExp(userName, gain / 700 + 35);

        return new RouletteModel(
            winningNumber,
            gain,
            gain > totalBet ? $"Bravo vous avez remporté {gain - totalBet} yanns" : $"Dommage vous avez perdu {totalBet - gain} yanns",
            user.Money
        );
    }

    private long getTotalBetValue(RouletteRequest bets)
    {
        long acc = bets.Singles?.Sum(s => s.Item2) ?? 0;

        var properties = typeof(RouletteRequest).GetProperties();
        
        foreach (var prop in properties)
        {
            if (prop.PropertyType == typeof(long?))
            {
                var value = (long?)prop.GetValue(bets);
                acc += value ?? 0;
            }
        }

        return acc;
    }

    private long Singles((int, long)[] bets, int winningNumber)
    {
        const float mult = 36f;
        for (int i = 0; i < bets.Length; i++)
        {
            if (bets[i].Item1 == winningNumber) return (long) Math.Floor(bets[i].Item2 * mult);
        }
        return 0;
    }
    
    private long Even(long bet, int winningNumber)
    {
        const float mult = 2f;
        return winningNumber % 2 == 0 ? (long) Math.Floor(bet * mult) : 0;
    }
    
    private long Odd(long bet, int winningNumber)
    {
        const float mult = 2f;
        return winningNumber % 2 == 1 ? (long) Math.Floor(bet * mult) : 0;
    }
    
    private long Red(long bet, int winningNumber)
    {
        const float mult = 2f;
        return RedNumbers.Contains(winningNumber) ? (long)Math.Floor(bet * mult) : 0;
    }

    private long Black(long bet, int winningNumber)
    {
        const float mult = 2f;
        return winningNumber != 0 && !RedNumbers.Contains(winningNumber)
            ? (long)Math.Floor(bet * mult)
            : 0;
    }
    
    private long FirstTier(long bet, int winningNumber)
    {
        const float mult = 3f;
        return winningNumber >= 1 && winningNumber <= 12 ? (long)Math.Floor(bet * mult) : 0;
    }

    private long SecondTier(long bet, int winningNumber)
    {
        const float mult = 3f;
        return winningNumber >= 13 && winningNumber <= 24 ? (long)Math.Floor(bet * mult) : 0;
    }

    private long ThirdTier(long bet, int winningNumber)
    {
        const float mult = 3f;
        return winningNumber >= 25 && winningNumber <= 36 ? (long)Math.Floor(bet * mult) : 0;
    }

    
    private long FirstRow(long bet, int winningNumber)
    {
        const float mult = 3f;
        return winningNumber >= 1 && winningNumber <= 36 && (winningNumber - 1) % 3 == 0
            ? (long)Math.Floor(bet * mult)
            : 0;
    }

    private long SecondRow(long bet, int winningNumber)
    {
        const float mult = 3f;
        return winningNumber >= 1 && winningNumber <= 36 && (winningNumber - 2) % 3 == 0
            ? (long)Math.Floor(bet * mult)
            : 0;
    }

    private long ThirdRow(long bet, int winningNumber)
    {
        const float mult = 3f;
        return winningNumber >= 1 && winningNumber <= 36 && winningNumber % 3 == 0
            ? (long)Math.Floor(bet * mult)
            : 0;
    }

    
    private long FirstHalf(long bet, int winningNumber)
    {
        const float mult = 2f;
        return winningNumber >= 1 && winningNumber <= 18 ? (long)Math.Floor(bet * mult) : 0;
    }

    private long SecondHalf(long bet, int winningNumber)
    {
        const float mult = 2f;
        return winningNumber >= 19 && winningNumber <= 36 ? (long)Math.Floor(bet * mult) : 0;
    }


    private long ComputeGains(RouletteRequest bets, int winningNumber)
    {
        long acc = 0;
        if (bets.Singles != null) acc += Singles(bets.Singles, winningNumber);
        if (bets.Even != null) acc += Even(bets.Even.Value, winningNumber);
        if (bets.Odd != null) acc += Odd(bets.Odd.Value, winningNumber);
        if (bets.Red != null) acc += Red(bets.Red.Value, winningNumber);
        if (bets.Black != null) acc += Black(bets.Black.Value, winningNumber);
        if (bets.FirstTier != null) acc += FirstTier(bets.FirstTier.Value, winningNumber);
        if (bets.SecondTier != null) acc += SecondTier(bets.SecondTier.Value, winningNumber);
        if (bets.ThirdTier != null) acc += ThirdTier(bets.ThirdTier.Value, winningNumber);
        if (bets.FirstRow != null) acc += FirstRow(bets.FirstRow.Value, winningNumber);
        if (bets.SecondRow != null) acc += SecondRow(bets.SecondRow.Value, winningNumber);
        if (bets.ThirdRow != null) acc += ThirdRow(bets.ThirdRow.Value, winningNumber);
        if (bets.FirstHalf != null) acc += FirstHalf(bets.FirstHalf.Value, winningNumber);
        if (bets.SecondHalf != null) acc += SecondHalf(bets.SecondHalf.Value, winningNumber);
        return acc;
    }
}