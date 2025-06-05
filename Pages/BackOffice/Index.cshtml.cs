// Pages/Backoffice.cshtml.cs

using CasinoDeYann.Api.Services;
using CasinoDeYann.Api.Services.Stats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CasinoDeYann.Pages.BackOffice;

[Authorize(Roles = "Admin")]
public class BackofficeModel : PageModel
{
    private readonly StatsService _statsService;
        private readonly ILogger<IndexModel> _logger;

        public BackofficeModel(StatsService statsService, ILogger<IndexModel> logger)
        {
            _statsService = statsService;
            _logger = logger;
        }

        public IEnumerable<GameHistoryEntryModel> GameHistory { get; set; } = Enumerable.Empty<GameHistoryEntryModel>();

        public string CurrentSort { get; set; }
        public string BetSort { get; set; }
        public string GainSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }

        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        [BindProperty(SupportsGet = true)] public string SearchString { get; set; }
        [BindProperty(SupportsGet = true)] public string SortOrder { get; set; }
        [BindProperty(SupportsGet = true)] public int PageIndex { get; set; } = 1;

        public async Task OnGetAsync()
        {
            CurrentSort = SortOrder;
            BetSort = SortOrder == "bet_asc" ? "bet_desc" : "bet_asc";
            GainSort = SortOrder == "gain_asc" ? "gain_desc" : "gain_asc";
            DateSort = SortOrder == "date_asc" ? "date_desc" : "date_asc";
            CurrentFilter = SearchString;

            var result = await _statsService.GetBackOffice(CurrentSort, SearchString, PageIndex);

            GameHistory = result.GameHistory;
            HasNextPage = result.HasNext;
            HasPreviousPage = result.HasPrevious;
        }
}
