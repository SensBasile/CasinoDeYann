using CasinoDeYann.DataAccess.Interfaces;
using CasinoDeYann.Dbo;

namespace CasinoDeYann.BusinessLogic;

public class UserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUsersRepository _usersRepository;

    public UserContextService(IHttpContextAccessor httpContextAccessor, IUsersRepository usersRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _usersRepository = usersRepository;
    }

    public Task<User> GetCurrentUserAsync()
    {
        var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Task.FromResult<User?>(null);

        return Task.FromResult(_usersRepository.GetOneByName(username));
    }
}
