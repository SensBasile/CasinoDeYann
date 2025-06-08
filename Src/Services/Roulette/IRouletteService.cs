using CasinoDeYann.Controllers.Roulette.DTOs;
using CasinoDeYann.Services.Roulette.Models;

namespace CasinoDeYann.Services.Roulette;

public interface IRouletteService
{
    public Task<RouletteModel> play(string userName, RouletteRequest bets);
}