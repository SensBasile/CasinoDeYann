using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Controllers;

public class SlotMachine : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}