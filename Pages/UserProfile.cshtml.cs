using CasinoDeYann.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CasinoDeYann.Pages;

public class UserProfile(UsersService usersService) : PageModel
{
    // ------------ PROPRIÉTÉS POUR LE RAZOR ----------------
        public long Level { get; private set; }

        // Solde actuel
        public decimal Balance { get; private set; }

        // Historique des parties : date, nom du jeu, mise, gain
        public List<GameHistoryEntryModel> History { get; private set; } = new();

        public decimal MaxWin { get; private set; }            // Gain maximal en une partie
        public decimal TotalPlayed { get; private set; }        // Somme totale pariée
        public decimal TotalWon { get; private set; }           // Somme totale gagnée
        public decimal TotalLost { get; private set; }          // Somme totale perdue
        public int MaxWinStreak { get; private set; }           // Plus longue série de victoires

        public Dictionary<string, int> GamesPlayedPerGame { get; private set; } = new();

        public Dictionary<string, int> GamesPlayedPerWeek { get; private set; } = new();

        // ------------ PAGE LIFECYCLE ----------------

        public async Task OnGetAsync()
        {
            var userName = User.Identity.Name ?? throw new InvalidOperationException("Utilisateur non authentifié.");

            var dto = await usersService.GetUserProfileAsync(userName);

            Level = dto.Level;
            Balance = dto.Balance;
            History = dto.History;  // List<GameHistoryEntry>
            MaxWin = dto.HighestGain;
            TotalPlayed = dto.NumberOfGames;
            TotalWon = dto.TotalWon;
            TotalLost = dto.TotalLost;
            GamesPlayedPerGame = dto.GamesPlayedPerGame;
        }

        // ------------ HANDLER DE SUPPRESSION DE COMPTE ----------------

        public async Task<IActionResult> OnPostDeleteAccountAsync()
        {
            var userName = User.Identity.Name ?? throw new InvalidOperationException("Utilisateur non authentifié.");
            // FIXME
            /// await usersService.DeleteAccountAsync(userName);
            return RedirectToPage("/Index");
        }
    }