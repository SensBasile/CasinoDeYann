using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Api.Controllers.GoldMine;

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