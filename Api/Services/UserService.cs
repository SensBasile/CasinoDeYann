using AutoMapper;
using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.DataAccess.Interfaces;

namespace CasinoDeYann.Api.Services;

public class UserService(IUsersRepository usersRepository, IStatsRepository statsRepository)
{
    public async Task<User> GetUser(string username)
    {
        return await usersRepository.GetOneByName(username);
    }

    public async Task<User> Pay(string username, long amount)
    {
        User user = await usersRepository.GetOneByName(username);
        if (user.Money < amount) throw new BadHttpRequestException("You don't have enough money");
        user.Money -= amount;
        return await usersRepository.Update(user);
    }

    public async Task<IEnumerable<User>> GetLeaderboard()
    {
        return (await usersRepository.Get())
            .OrderByDescending(u => u.Money)
            .Take(10);
    }

    public async Task<UserStatsSummary> GetStats(string name)
    {
        var user = await usersRepository.GetOneByName(name);
        return statsRepository.GetStats(user.Id);
    }

    public async Task<UserProfileModel> GetUserProfileAsync(string userName)
    {
        var stats = await GetStats(userName);
        
        var user = await usersRepository.GetOneByName(userName); 
        
        List<GameHistoryEntryModel> history = new List<GameHistoryEntryModel>();

        foreach (var stat in stats.History)
        {
            history.Add(new GameHistoryEntryModel(user.Id, stat.Date, stat.Game, stat.Bet, stat.Gain));
        }


        return new UserProfileModel(
            user.Xp / 1000, 
            user.Money, history, stats.HighestGain, 
            stats.NumberOfGames,
            stats.TotalWon,
            stats.TotalLost, 
            stats.GamesPlayedPerGame, 
            stats.GamesPlayedPerDay);
    }

    public async Task<bool> DeleteAccountAsync(string userName)
    {
        return await usersRepository.DeleteOneByName(userName);
    }
    
    public async Task<User> AddMoney(string name, long amount)
    {
        var user = await GetUser(name);
        user.Money += amount;
        return await usersRepository.Update(user);
    }
    
    public async Task<User> AddExp(string name, long amount)
    {
        var user = await GetUser(name);
        user.Xp += amount;
        return await usersRepository.Update(user);
    }
}