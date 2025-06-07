using CasinoDeYann.DataAccess.Dbo;
using CasinoDeYann.DataAccess.Interfaces;

namespace CasinoDeYann.Services.Stats;

public class StatsService
{
    private readonly IStatsRepository _statsRepository;
    
    private readonly UserService _usersService;

    public StatsService(IStatsRepository statsRepository, UserService usersService)
    {
        _statsRepository = statsRepository;
        _usersService = usersService;
    }

    public async Task<GameHistoryEntryModel> Create(GameHistoryEntryModel model)
    {
        var user = await _usersService.GetUser(model.Username);
        var stats = new DataAccess.Dbo.Stats();
        stats.UserId = user.Id;
        stats.Date = model.Date;
        stats.Bet = model.Bet;
        stats.Gain = model.Gain;
        stats.Game = model.Game; 
        var res = await _statsRepository.Insert(stats);
        
        return new GameHistoryEntryModel(stats.Id, user.Username, res.Date, res.Game, res.Bet, res.Gain, false);
    }


    public async Task<BackOfficeModel> GetBackOffice(string sortOrder, string searchString, int pageIndex)
    {
        var stats = await _statsRepository.Get(sortOrder, searchString, pageIndex);
        
        
        
        List<GameHistoryEntryModel> models = new List<GameHistoryEntryModel>();

        foreach (var stat in stats.Stats)
        {
            models.Add(new GameHistoryEntryModel(
                stat.Id,
                stat.User.Username, 
                stat.Date, 
                stat.Game, 
                stat.Bet, 
                stat.Gain, 
                stat.IsCanceled
                ));
        }
        
        return new BackOfficeModel(models, pageIndex > 1, stats.TotalPages > pageIndex);
    }

    public async Task<DataAccess.Dbo.Stats> Cancel(int id)
    {
        var stat = await _statsRepository.GetOneById(id);
        
        var user = await _usersService.GetUser(stat.UserId);
        
        await _usersService.AddMoney(user.Username, stat.Bet - stat.Gain);

        stat.IsCanceled = true;
        
        return await _statsRepository.Update(stat);

    }
}