using CasinoDeYann.Api.DataAccess.Interfaces;

namespace CasinoDeYann.Api.Services.Stats;

public class StatsService
{
    private readonly IStatsRepository _statsRepository;
    
    private readonly IUsersRepository _usersRepository;

    public StatsService(IStatsRepository statsRepository, IUsersRepository usersRepository)
    {
        _statsRepository = statsRepository;
        _usersRepository = usersRepository;
    }

    public async Task<GameHistoryEntryModel> Create(GameHistoryEntryModel model)
    {
        var user = await _usersRepository.GetOneByName(model.Username);
        var stats = new DataAccess.Dbo.Stats();
        stats.UserId = user.Id;
        stats.Date = model.Date;
        stats.Bet = model.Bet;
        stats.Gain = model.Gain;
        stats.Game = model.Game; 
        var res = await _statsRepository.Insert(stats);
        
        return new GameHistoryEntryModel(user.Username, res.Date, res.Game, res.Bet, res.Gain);
    }


    public async Task<BackOfficeModel> GetBackOffice(string sortOrder, string searchString, int pageIndex)
    {
        var stats = await _statsRepository.Get(sortOrder, searchString, pageIndex);
        
        
        
        List<GameHistoryEntryModel> models = new List<GameHistoryEntryModel>();

        foreach (var stat in stats.Stats)
        {
            models.Add(new GameHistoryEntryModel(
                stat.User.Username, stat.Date, stat.Game, stat.Bet, stat.Gain
                ));
        }
        
        return new BackOfficeModel(models, pageIndex > 1, stats.TotalPages > pageIndex);
    }
}