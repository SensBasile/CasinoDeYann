namespace CasinoDeYann.Controllers.HorseRace.DTOs;

public class HorseRaceRequest
{
    public required HorseBet First;
    public HorseBet? Second;
    public HorseBet? Third;
}