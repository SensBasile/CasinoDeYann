namespace CasinoDeYann.Services.User;

public class UserContextService(IHttpContextAccessor httpContextAccessor, IUserService userService)
{
    public Task<User.Models.User> GetCurrentUserAsync()
    {
        var username = httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Task.FromResult<User.Models.User>(null);

        return userService.GetUser(username)!;
    }
}
