using CasinoDeYann.Api.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CasinoDeYann.Pages;

public class Roulette(UserContextService userContextService) : PageModel
{
    public Api.DataAccess.Dbo.User? CurrentUser;
    
    public async Task OnGetAsync()
    {
        CurrentUser = await userContextService.GetCurrentUserAsync();
    }
}