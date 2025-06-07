using CasinoDeYann.Controllers.HorseRace.DTOs;

namespace CasinoDeYann.Services.HorseRace.Models;

public class HorseRaceModel(int[] res, (int, int)[][] paces, long gains, long money, string message)
{
    public int[] Results { get; set; } = res;
    public (int, int)[][] Paces { get; set; } = paces;
    public long Gains { get; set; } = gains;
    public long Money { get; set; } = money;
    public string Message { get; set; } = message;

    public HorseRaceResponse ToResponse()
    {
        List<Pace[]> paces = new();
        for (int i = 0; i < Paces.Length; i++)
        {
            List<Pace> pace = new();
            for (int j = 0; j < Paces[i].Length; j++)
            {
                pace.Add(new Pace(Paces[i][j].Item1, Paces[i][j].Item2));
            }
            paces.Add(pace.ToArray());
        }
        
        return new HorseRaceResponse(
            Results,
            paces.ToArray(),
            Gains,
            Money,
            Message
            );
    }
}