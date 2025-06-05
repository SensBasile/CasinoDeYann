using System.Security.Claims;
using CasinoDeYann.Src.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CasinoDeYann.Pages;


public class IndexModel : PageModel
{
    private readonly UserService _userService;

    public IndexModel(UserService userService)
    {
        _userService = userService;
    }

    public IEnumerable<Src.DataAccess.Dbo.User> TopUsers { get; private set; }

    public async Task<IActionResult> OnGet()
    {
        if (User.IsInRole("Admin")
            )
        {
            return Redirect("/BackOffice");
        }

        TopUsers = await _userService.GetLeaderboard();
        return Page();
    }
}