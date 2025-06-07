using CasinoDeYann.Controllers.HorseRace.DTOs;
using CasinoDeYann.DataAccess.Dbo;
using CasinoDeYann.Services.HorseRace.Models;

namespace CasinoDeYann.Services.HorseRace;

public class HorseRaceService(UserService userService)
{
    private const int HorsesNumber = 4;
    private const int TrackLength = 100;
    private readonly Random _random = new();
    
    public async Task<HorseRaceModel> Play(string username, HorseRaceRequest bet)
    {
        if (bet.Third != null && bet.Second == null) throw new BadHttpRequestException("You have to bet on a second if you bet on a third");
        
        long totalBet = TotalBet(bet);
        User callingUser = await userService.Pay(username, totalBet);
        
        _ = await userService.AddExp(username, totalBet / 100 + 20);

        List<(int, int)[]> paces = [];
        for (int i = 0; i < HorsesNumber; i++)
        {
            List<(int, int)> pacesList = new();
            int dist = 0;
            while (dist < TrackLength)
            {
                var pace = (TrackLength - dist < 20 ? TrackLength - dist : _random.Next(5, 20), _random.Next(50, 150));
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
        
        if (bet.First.Horse == results[0]) gains += bet.First.Amount * 4;
        if (bet.Second != null && bet.Second.Horse == results[1]) gains += bet.Second.Amount * 3;
        if (bet.Third != null && bet.Third.Horse == results[2]) gains += bet.Third.Amount * 2;

        return gains;
    }

    private int[] FinishOrder((int, int)[][] paces)
    {
        var sums = new List<(int, float)>();
        for (int i = 0; i < HorsesNumber; i++)
        {
            float sum = 0f;
            for (int j = 0; j < paces[i].Length; j++)
            {
                sum += paces[i][j].Item1 * (1f / paces[i][j].Item2);
            }
            sums.Add((i, sum));
        }
        
        var result = new List<int>();
        for (int i = 0; i < HorsesNumber; i++)
        {
            (int, float) max = (-1, 0);
            for (int j = 0; j < sums.Count; j++)
            {
                if (sums[j].Item2 > max.Item2) max = sums[j];
            }
            result.Add(max.Item1);
            sums.Remove(max);
        }
        
        return result.ToArray();
    }

    private long TotalBet(HorseRaceRequest bet)
    {
        return bet.First.Amount + (bet.Second?.Amount ?? 0) + (bet.Third?.Amount ?? 0);
    }
}