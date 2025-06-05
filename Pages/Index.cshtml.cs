using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CasinoDeYann.Pages;

public class IndexModel : PageModel
{
    private readonly UserService _userService;

    public IndexModel(UserService userService)
    {
        _userService = userService;
    }

    public IEnumerable<Api.DataAccess.Dbo.User> TopUsers { get; private set; }

    public async Task OnGet()
    {
        TopUsers = await _userService.GetLeaderboard();
    }
}