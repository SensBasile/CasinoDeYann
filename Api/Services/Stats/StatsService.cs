using CasinoDeYann.Api.DataAccess.Interfaces;

namespace CasinoDeYann.Api.Services.Stats;

public class StatsService
{
    private readonly IStatsRepository _statsRepository;

    public StatsService(IStatsRepository statsRepository)
    {
        _statsRepository = statsRepository;
    }

    public async Task<GameHistoryEntryModel> Create(GameHistoryEntryModel model)
    {
        var stats = new DataAccess.Dbo.Stats();
        stats.UserId = model.UserId;
        stats.Bet = model.Bet;
        stats.Gain = model.Gain;
        stats.Game = model.Game; 
        var res = await _statsRepository.Insert(stats);
        
        return new GameHistoryEntryModel(res.UserId, res.Game, res.Bet, res.Gain);
    }
}