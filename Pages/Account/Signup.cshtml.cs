using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using CasinoDeYann.Src.Services;

namespace CasinoDeYann.Pages.Account
{
    public class SignupModel : PageModel
    {
        private readonly AuthService _authService;

        public SignupModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }
        
        [BindProperty]
        public bool IsAdmin { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return Page();
            }

            var result = await _authService.RegisterAsync(Username, Password, IsAdmin ? "Admin" : "User");
            if (!result)
            {
                ErrorMessage = "Username already taken.";
                return Page();
            }

            return RedirectToPage("/Index");
        }
    }
}