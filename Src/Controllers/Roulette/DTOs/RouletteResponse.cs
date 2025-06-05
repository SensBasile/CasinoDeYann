namespace CasinoDeYann.Src.Controllers.Roulette.DTOs;

public record RouletteResponse(
    int WinningNumber,
    long Gain,
    string Message
);