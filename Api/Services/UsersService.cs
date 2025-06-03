using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.DataAccess.Interfaces;

namespace CasinoDeYann.Api.Services;

public class UsersService
{
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }
    
    public async Task<IEnumerable<User>> getLeaderboard()
    {
        return (await _usersRepository.Get())
            .OrderByDescending(u => u.Money)
            .Take(10);
    }
}