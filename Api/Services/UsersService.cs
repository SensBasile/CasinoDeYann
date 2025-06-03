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
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAccountAsync(string userName)
    {
        return await _usersRepository.DeleteOneByName(userName);
    }
}