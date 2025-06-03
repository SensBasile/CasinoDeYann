using System.Security.Claims;
using CasinoDeYann.DataAccess.Interfaces;
using CasinoDeYann.Dbo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CasinoDeYann.BusinessLogic;

public class AuthService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IUsersRepository usersRepository, IHttpContextAccessor httpContextAccessor)
    {
        _usersRepository = usersRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var user = await _usersRepository.getOneByName(username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            return false;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        if (_httpContextAccessor.HttpContext != null)
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                principal);
        return true;
    }

    public async Task LogoutAsync()
    {
        if (_httpContextAccessor.HttpContext != null) await _httpContextAccessor.HttpContext.SignOutAsync();
    }
    
    public async Task<bool> RegisterAsync(string username, string password)
    {
        var existingUser = await _usersRepository.getOneByName(username);
        if (existingUser != null)
            return false;

        var newUser = new User
        {
            Username = username,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "User"
        };

        await _usersRepository.Insert(newUser);
        await LoginAsync(username, password);

        return true;
    }
}