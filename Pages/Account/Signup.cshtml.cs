using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using CasinoDeYann.Services;
using Microsoft.Extensions.Hosting;

namespace CasinoDeYann.Pages.Account
{
    public class SignupModel : PageModel
    {
        private readonly AuthService _authService;
        private readonly IHostEnvironment _environment;

        public SignupModel(AuthService authService, IHostEnvironment environment)
        {
            _authService = authService;
            _environment = environment;
        }

        // La création d'un compte admin à l'inscription n'est permise qu'en
        // développement. En prod, la case est masquée ET ignorée côté serveur
        // (un IsAdmin=true forgé dans la requête ne donne pas le rôle).
        public bool CanCreateAdmin => _environment.IsDevelopment();

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

            var result = await _authService.RegisterAsync(Username, Password, IsAdmin && CanCreateAdmin ? "Admin" : "User");
            if (!result)
            {
                ErrorMessage = "Username already taken.";
                return Page();
            }

            return RedirectToPage("/Index");
        }
    }
}