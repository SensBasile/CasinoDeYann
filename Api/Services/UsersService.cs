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
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAccountAsync(string userName)
    {
        return await usersRepository.DeleteOneByName(userName);
    }
}