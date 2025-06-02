using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Controllers;

public class GoldMine : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}