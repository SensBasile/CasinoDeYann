using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using CasinoDeYann.Api.Services;

namespace CasinoDeYann.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AuthService _authService;

        public LoginModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Optional: clear previous error
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _authService.LoginAsync(Username, Password))
            {
                return RedirectToPage("/Index");
            }

            ErrorMessage = "Invalid username or password";
            return Page();
        }
    }
}