using CasinoDeYann.Services;
using CasinoDeYann.Services.Stats;
using CasinoDeYann.Services.Stats.Models;
using CasinoDeYann.Services.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CasinoDeYann.Pages
{
    public class User : PageModel
    {
        private readonly IStatsService _statsService;
        private readonly IUserService _userService;

        public User(IStatsService statsService, IUserService userService)
        {
            _statsService = statsService;
            _userService = userService;
        }

        [BindProperty(SupportsGet = true)] public string Username { get; set; }

        [BindProperty(SupportsGet = true)] public int PageIndex { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)] public string SortOrder { get; set; }

        public long Level { get; private set; }
        public decimal Balance { get; private set; }
        public IEnumerable<GameHistoryEntryModel> History { get; private set; }
        public decimal MaxWin { get; private set; }
        public decimal TotalPlayed { get; private set; }
        public decimal TotalWon { get; private set; }
        public decimal TotalLost { get; private set; }
        public Dictionary<string, int> GamesPlayedPerGame { get; private set; } = new();
        public Dictionary<DateTime, int> GamesPlayedPerDay { get; private set; } = new();
        
        public string CurrentSort { get; set; }
        public string BetSort { get; set; }
        public string GainSort { get; set; }
        public string DateSort { get; set; }

        // paging flags
        public bool HasPreviousPage { get; private set; }
        public bool HasNextPage { get; private set; }

        public async Task<IActionResult> OnGetAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return NotFound();
            
            if (User.IsInRole("Admin"))
                return Redirect("/BackOffice/" + username);
            
            CurrentSort = string.IsNullOrEmpty(SortOrder)? "date_desc" : SortOrder;
            BetSort = SortOrder == "bet_desc" ? "bet_asc" : "bet_desc";
            GainSort = SortOrder == "gain_desc" ? "gain_asc" : "gain_desc";
            DateSort = SortOrder == "date_desc" ? "date_asc" : "date_desc";

            Username = username;
            
            var dto = await _statsService.GetUserProfileAsync(
                sortOrder: CurrentSort,
                userName: username,
                pageIndex: PageIndex
            );

            if (dto == null)
                return NotFound();

            Level = dto.Level;
            Balance = dto.Balance;
            History = dto.Stats.History;
            MaxWin = dto.Stats.HighestGain;
            TotalPlayed = dto.Stats.NumberOfGames;
            TotalWon = dto.Stats.TotalWon;
            TotalLost = dto.Stats.TotalLost;
            GamesPlayedPerGame = dto.Stats.GamesPlayedPerGame;
            GamesPlayedPerDay = dto.Stats.GamesPlayedPerDay;

            // pull paging info from the returned stats model
            HasPreviousPage = dto.Stats.HasPrevious;
            HasNextPage = dto.Stats.HasNext;

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAccountAsync()
        {
            var userName = User.Identity.Name ?? throw new InvalidOperationException("Utilisateur non authentifié.");
            await _userService.DeleteAccountAsync(userName);
            return RedirectToPage("/Index");
        }
    }
}
