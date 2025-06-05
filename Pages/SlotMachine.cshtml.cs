using CasinoDeYann.Src.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CasinoDeYann.Pages;

public class SlotMachine(UserContextService userContextService) : PageModel
{
    public Src.DataAccess.Dbo.User? CurrentUser;
    
    public async Task OnGetAsync()
    {
        CurrentUser = await userContextService.GetCurrentUserAsync();
    }
}