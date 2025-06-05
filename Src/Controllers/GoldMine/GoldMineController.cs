using CasinoDeYann.Src.Services.GoldMineService;
using CasinoDeYann.Src.Services.GoldMineService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CasinoDeYann.Src.Controllers.GoldMine;

[Route("api/[controller]")]
[ApiController]

public class GoldMineController(GoldMineService goldMineService, IMemoryCache cache) : Controller
{
    private static readonly TimeSpan Cooldown = TimeSpan.FromSeconds(0.1);

    [HttpPost("mine")]
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.Name is not string username)
            return Unauthorized();

        string cacheKey = $"goldmine-cooldown-{username}";

        if (cache.TryGetValue(cacheKey, out _))
        {
            return StatusCode(429, "Too many requests. Please wait a moment.");
        }

        cache.Set(cacheKey, true, Cooldown);
        GoldMineModel res = await goldMineService.Mine(username);
        return Ok(res.ToResponse());
    }
}