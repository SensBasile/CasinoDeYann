using CasinoDeYann.DataAccess.Dbo;
using CasinoDeYann.DataAccess.Interfaces;

namespace CasinoDeYann.Services;

public class UserService(IUsersRepository _usersRepository, IStatsRepository _statsRepository)
{
    public async Task<User> GetUser(string username)
    {
        return await _usersRepository.GetOneByName(username);
    }
    
    public async Task<User> GetUser(long id)
    {
        return await _usersRepository.GetOneById(id);
    }

    public async Task<User> Pay(string username, long amount)
    {
        User user = await _usersRepository.GetOneByName(username);
        if (user.Money < amount) throw new BadHttpRequestException("You don't have enough money");
        user.Money -= amount;
        return await _usersRepository.Update(user);
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
            history.Add(new GameHistoryEntryModel(stat.Id, user.Username, stat.Date, stat.Game, stat.Bet, stat.Gain, stat.IsCanceled));
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
        return await _usersRepository.DeleteOneByName(userName);
    }
    
    public async Task<User> AddMoney(string name, long amount)
    {
        var user = await GetUser(name);
        user.Money += amount;
        return await _usersRepository.Update(user);
    }
    
    public async Task<User> AddExp(string name, long amount)
    {
        var user = await GetUser(name);
        user.Xp += amount;
        return await _usersRepository.Update(user);
    }

    public async Task<User> ChangeRole(string username, string role)
    {
        var user = await GetUser(username);
        user.Role = role;
        return await _usersRepository.Update(user);
    }

    public async Task<IEnumerable<string>> Search(string query)
    { 
        return await  _usersRepository
            .FindUsernamesStartingWith(query);
    }
}