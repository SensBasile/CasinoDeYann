using CasinoDeYann.Src.DataAccess.Dbo;
using CasinoDeYann.Src.DataAccess.Interfaces;

namespace CasinoDeYann.Src.Services;

public class UserContextService(IHttpContextAccessor httpContextAccessor, UserService userService)
{
    public Task<User?> GetCurrentUserAsync()
    {
        var username = httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Task.FromResult<User?>(null);

        return userService.GetUser(username)!;
    }
}
