using CasinoDeYann.Services;
using CasinoDeYann.Services.Stats;
using CasinoDeYann.Services.Stats.Models;
using CasinoDeYann.Services.User;
using CasinoDeYann.Services.User.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CasinoDeYann.Pages.BackOffice;

[Authorize(Roles = "Admin")]
public class UserModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IStatsService _statsService;

    public UserModel(IUserService userService, IStatsService statsService)
    {
        _userService = userService;
        _statsService = statsService;
    }

    [BindProperty(SupportsGet = true)]
    public string Username { get; set; }

    public UserProfileModel User { get; set; }

    [BindProperty]
    public int Amount { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            return NotFound();
        }
        // FIXME
        User = await _statsService.GetUserProfileAsync("", Username, 1);
        if (User == null)
        {
            return NotFound();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAddMoneyAsync()
    {
        await _userService.AddMoney(Username, Amount);
        
        return RedirectToPage(new { username = Username });
    }

    public async Task<IActionResult> OnPostMakeAdminAsync()
    {
        await _userService.ChangeRole(Username, "Admin");
        return RedirectToPage(new { username = Username });
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        await _userService.DeleteAccountAsync(Username);
        return RedirectToPage("/BackOffice/Index");
    }
}