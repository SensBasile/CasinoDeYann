using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.DataAccess.Interfaces;
using CasinoDeYann.Api.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    private readonly UsersService _usersService;

    public IndexModel(UsersService usersService)
    {
        _usersService = usersService;
    }

    public IEnumerable<User> TopUsers { get; private set; }

    public async Task OnGet()
    {
        TopUsers = await _usersService.GetLeaderboard();
    }
}