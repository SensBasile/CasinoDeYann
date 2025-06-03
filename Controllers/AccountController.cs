using CasinoDeYann.BusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Controllers;

public class AccountController : Controller
{
    private readonly AuthService _authService;

    public AccountController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (await _authService.LoginAsync(username, password))
            return RedirectToAction("Index", "Home");

        ViewBag.Error = "Nom d'utilisateur ou mot de passe invalide";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Login");
    }
}