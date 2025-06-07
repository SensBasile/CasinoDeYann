using CasinoDeYann.Src.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CasinoDeYann.Pages.BackOffice;

[Authorize(Roles = "Admin")]
public class UserModel : PageModel
{
    private readonly UserService _userService;

    public UserModel(UserService userService)
    {
        _userService = userService;
    }

    [BindProperty(SupportsGet = true)]
    public string Username { get; set; }

    public UserProfileModel User { get; set; }

    [BindProperty]
    public int Amount { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        User = await _userService.GetUserProfileAsync(Username);
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