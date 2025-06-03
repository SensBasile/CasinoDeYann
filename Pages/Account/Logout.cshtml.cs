using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using CasinoDeYann.Api.Services;

namespace CasinoDeYann.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly AuthService _authService;

        public LogoutModel(AuthService authService)
        {
            _authService = authService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _authService.LogoutAsync();
            return RedirectToPage("/Index");
        }

        public void OnGet()
        {
        }
    }
}