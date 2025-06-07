using CasinoDeYann.Services;
using CasinoDeYann.Services.User;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CasinoDeYann.Pages;

public class GoldMine(UserContextService userContextService) : PageModel
{
    public Services.User.Models.User? CurrentUser;
    
    public async Task OnGetAsync()
    {
        CurrentUser = await userContextService.GetCurrentUserAsync();
    }
}