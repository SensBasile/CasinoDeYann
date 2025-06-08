using CasinoDeYann.Services;
using CasinoDeYann.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Controllers.User;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Get(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Ok(new List<string>());

        return Ok(await userService.Search(query));
    }
}