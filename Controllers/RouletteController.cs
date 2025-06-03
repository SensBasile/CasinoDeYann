using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RouletteController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}