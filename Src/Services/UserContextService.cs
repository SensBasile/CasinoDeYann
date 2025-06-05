using CasinoDeYann.Src.DataAccess.Dbo;
using CasinoDeYann.Src.DataAccess.Interfaces;

namespace CasinoDeYann.Src.Services;

public class UserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUsersRepository _usersRepository;

    public UserContextService(IHttpContextAccessor httpContextAccessor, IUsersRepository usersRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _usersRepository = usersRepository;
    }

    public Task<User?> GetCurrentUserAsync()
    {
        var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Task.FromResult<User?>(null);

        return _usersRepository.GetOneByName(username)!;
    }
}
