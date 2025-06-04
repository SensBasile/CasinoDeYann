using AutoMapper;
using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.DataAccess.Interfaces;

namespace CasinoDeYann.Api.Services;

public class UsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IStatsRepository _statsRepository;

    public UsersService(IUsersRepository usersRepository, IStatsRepository statsRepository)
    {
        _usersRepository = usersRepository;
        _statsRepository = statsRepository;
    }
    
    public async Task<IEnumerable<User>> GetLeaderboard()
    {
        return (await _usersRepository.Get())
            .OrderByDescending(u => u.Money)
            .Take(10);
    }

    public async Task<UserStatsSummary> GetStats(string name)
    {
        var user = await _usersRepository.GetOneByName(name);
        return _statsRepository.GetStats(user.Id);
    }

    public async Task<UserProfileModel> GetUserProfileAsync(string userName)
    {
        var stats = await GetStats(userName);
        
        var user = await _usersRepository.GetOneByName(userName); 
        
        List<GameHistoryEntryModel> history = new List<GameHistoryEntryModel>();

        foreach (var stat in stats.History)
        {
            history.Add(new GameHistoryEntryModel(user.Id, stat.Game, stat.Bet, stat.Gain));
        }


        return new UserProfileModel(
            user.Xp % 1000, 
            user.Money, history, stats.HighestGain, 
            stats.NumberOfGames,
            -1, 
            -1, new Dictionary<string, int>());
    }

    public async Task<bool> DeleteAccountAsync(string userName)
    {
        return await _usersRepository.DeleteOneByName(userName);
    }
}