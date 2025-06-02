using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Controllers;

public class BlackJack : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}