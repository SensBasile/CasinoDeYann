namespace CasinoDeYann.Controllers.Roulette.DTOs;

public class RouletteRequest
{
    public SingleBetRequest[]? Singles { get; set; }
    public long? Even { get; set; }
    public long? Odd { get; set; }
    public long? Red { get; set; }
    public long? Black { get; set; }
    public long? FirstTier { get; set; }
    public long? SecondTier { get; set; }
    public long? ThirdTier { get; set; }
    public long? FirstRow { get; set; }
    public long? SecondRow { get; set; }
    public long? ThirdRow { get; set; }
    public long? FirstHalf { get; set; }
    public long? SecondHalf { get; set; }
}