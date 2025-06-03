using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.DataAccess.Interfaces;

namespace CasinoDeYann.Api.Services;

public class UsersService(IUsersRepository usersRepository)
{
    public async Task<IEnumerable<User>> GetLeaderboard()
    {
        return (await usersRepository.Get())
            .OrderByDescending(u => u.Money)
            .Take(10);
    }

    public async Task<UserProfileModel> GetUserProfileAsync(string userName)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAccountAsync(string userName)
    {
        throw new NotImplementedException();
    }
}