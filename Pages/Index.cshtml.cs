using System.Security.Claims;
using CasinoDeYann.Services;
using CasinoDeYann.Services.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CasinoDeYann.Pages;


public class IndexModel : PageModel
{
    private readonly IUserService _userService;

    public IndexModel(IUserService userService)
    {
        _userService = userService;
    }

    public IEnumerable<Services.User.Models.User> TopUsers { get; private set; }

    public async Task<IActionResult> OnGet()
    {
        if (User.IsInRole("Admin"))
        {
            return Redirect("/BackOffice");
        }

        TopUsers = await _userService.GetLeaderboard();
        return Page();
    }
}