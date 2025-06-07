using CasinoDeYann.Services;
using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Controllers.User;

[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Ok(new List<string>());

        return Ok(await _userService.Search(query));
    }
    
}