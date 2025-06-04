using AutoMapper;
using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.DataAccess.Interfaces;

namespace CasinoDeYann.Api.Services;

public class UsersService(IUsersRepository usersRepository, IStatsRepository statsRepository)
{
    public async Task<User> GetUser(string username)
    {
        return await usersRepository.GetOneByName(username);
    }

    public async Task<bool> Pay(string username, long amount)
    {
        var user = await usersRepository.GetOneByName(username);
        if (user.Money < amount) return false;
        user.Money -= amount;
        await usersRepository.Update(user);
        return true;
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
            user.Xp % 1000, 
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
}