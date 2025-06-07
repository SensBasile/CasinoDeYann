using CasinoDeYann.Controllers.HorseRace.DTOs;
using CasinoDeYann.DataAccess.Dbo;
using CasinoDeYann.Services.HorseRace.Models;
using CasinoDeYann.Services.User;

namespace CasinoDeYann.Services.HorseRace;

public class HorseRaceService(IUserService userService)
{
    private const int HorsesNumber = 4;
    private const int TrackLength = 100;
    private readonly Random _random = new();
    
    public async Task<HorseRaceModel> Play(string username, HorseRaceRequest bet)
    {
        if (bet.First == null) throw new BadHttpRequestException("You must bet on the first");
        if (bet.Third != null && bet.Second == null) throw new BadHttpRequestException("You have to bet on a second if you bet on a third");
        
        var callingUser = await userService.Pay(username, bet.Amount);
        
        _ = await userService.AddExp(username, bet.Amount / 100 + 20);

        List<(int, int)[]> paces = [];
        for (int i = 0; i < HorsesNumber; i++)
        {
            List<(int, int)> pacesList = new();
            int dist = 0;
            while (dist < TrackLength)
            {
                var pace = (TrackLength - dist < 20 ? TrackLength - dist : _random.Next(5, 20), _random.Next(35, 60));
                dist += pace.Item1;
                pacesList.Add((pace.Item1, pace.Item2));
            }

            paces.Add(pacesList.ToArray());
        }

        var run = paces.ToArray();
        var results = FinishOrder(run);
        long gains = ComputeGains(results, bet);
        
        if (gains > 0) callingUser = await userService.AddMoney(username, gains);
        
        return new HorseRaceModel(
            results,
            run,
            gains,
            callingUser.Money,
            gains > 0 ? $"Bravo ! Vous avez gagné {gains} Yans" : "Dommage, retentez votre chance"
            );
    }

    private long ComputeGains(int[] results, HorseRaceRequest bet)
    {
        long gains = 0;
        
        if (bet.First == results[0]) gains += bet.Amount * 4;
        if (bet.Second == results[1]) gains *= 3;
        if (bet.Third == results[2]) gains *= 2;

        return gains;
    }

    private int[] FinishOrder((int, int)[][] paces)
    {
        var sums = new List<(int, int)>();
        for (int i = 0; i < HorsesNumber; i++)
        {
            int sum = 0;
            for (int j = 0; j < paces[i].Length; j++)
            {
                sum += paces[i][j].Item1 * paces[i][j].Item2;
            }
            sums.Add((i, sum));
        }
        
        var result = new List<int>();
        for (int i = 0; i < HorsesNumber; i++)
        {
            (int, int) max = (-1, 0);
            for (int j = 0; j < sums.Count; j++)
            {
                if (sums[j].Item2 > max.Item2) max = sums[j];
            }
            result.Add(max.Item1);
            sums.Remove(max);
        }
        
        return result.ToArray();
    }
}