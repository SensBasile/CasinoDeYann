using CasinoDeYann.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CasinoDeYann.Pages;

public class UserProfile(UsersService usersService) : PageModel
{
    // ------------ PROPRIÉTÉS POUR LE RAZOR ----------------
        public int Level { get; private set; }

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
            MaxWin = dto.MaxWin;
            TotalPlayed = dto.TotalPlayed;
            TotalWon = dto.TotalWon;
            TotalLost = dto.TotalLost;
            MaxWinStreak = dto.MaxWinStreak;
            GamesPlayedPerGame = dto.GamesPlayedPerGame;
            GamesPlayedPerWeek = dto.GamesPlayedPerWeek;
        }

        // ------------ HANDLER DE SUPPRESSION DE COMPTE ----------------

        public async Task<IActionResult> OnPostDeleteAccountAsync()
        {
            var userName = User.Identity.Name ?? throw new InvalidOperationException("Utilisateur non authentifié.");
            await usersService.DeleteAccountAsync(userName);
            return RedirectToPage("/Index");
        }
    }