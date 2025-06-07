namespace CasinoDeYann.Controllers.HorseRace.DTOs;

public class Pace
{
    public int Dist { get; set; }
    public int Speed { get; set; }

    public Pace(int dist, int speed)
    {
        Dist = dist;
        Speed = speed;
    }
}