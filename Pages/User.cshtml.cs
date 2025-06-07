using CasinoDeYann.Services;
using CasinoDeYann.Services.Stats;
using CasinoDeYann.Services.Stats.Models;
using CasinoDeYann.Services.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CasinoDeYann.Pages;

public class User(IStatsService statsService, IUserService userService) : PageModel
{
    public string Username { get; private set; }

    public long Level { get; private set; }
    public decimal Balance { get; private set; }
    public IEnumerable<GameHistoryEntryModel> History { get; private set; }
    public DateTime Date { get; private set; }
    public decimal MaxWin { get; private set; }
    public decimal TotalPlayed { get; private set; }
    public decimal TotalWon { get; private set; }
    public decimal TotalLost { get; private set; }
    public Dictionary<string, int> GamesPlayedPerGame { get; private set; } = new();
    public Dictionary<DateTime, int> GamesPlayedPerDay { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return NotFound();
        }
        
        if (User.IsInRole("Admin")
           )
        {
            return Redirect("/BackOffice/" + username);
        }

        Username = username;

        var dto = await statsService.GetUserProfileAsync("", username, 1);

        if (dto == null)
        {
            return NotFound(); // User not found
        }

        Level = dto.Level;
        Balance = dto.Balance;
        History = dto.Stats.History;
        MaxWin = dto.Stats.HighestGain;
        TotalPlayed = dto.Stats.NumberOfGames;
        TotalWon = dto.Stats.TotalWon;
        TotalLost = dto.Stats.TotalLost;
        GamesPlayedPerGame = dto.Stats.GamesPlayedPerGame;
        GamesPlayedPerDay = dto.Stats.GamesPlayedPerDay;

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAccountAsync()
    {
        var userName = User.Identity.Name ?? throw new InvalidOperationException("Utilisateur non authentifié.");
        await userService.DeleteAccountAsync(userName);
        return RedirectToPage("/Index");
    }
}
