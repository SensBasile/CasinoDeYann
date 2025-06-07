namespace CasinoDeYann.Controllers.Roulette.DTOs;

public record RouletteResponse(
    int WinningNumber,
    long Gain,
    string Message,
    long Money
);