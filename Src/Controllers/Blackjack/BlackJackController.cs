﻿using Microsoft.AspNetCore.Mvc;

namespace CasinoDeYann.Src.Controllers.Blackjack;

[Route("api/[controller]")]
[ApiController]
public class BlackJackController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}