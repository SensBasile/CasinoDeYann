using CasinoDeYann.DataAccess.Dbo;

namespace CasinoDeYann.Services;

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
