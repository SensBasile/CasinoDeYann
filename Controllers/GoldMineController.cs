using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GoldMineController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}