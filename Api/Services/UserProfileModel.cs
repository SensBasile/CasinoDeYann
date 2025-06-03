namespace CasinoDeYann.Api.Services;

public record UserProfileModel(
    int Level,
    long Balance,
    List<GameHistoryEntryModel> History,
    long MaxWin,
    long TotalPlayed,
    long TotalWon,
    long TotalLost,
    int MaxWinStreak,
    Dictionary<string, int> GamesPlayedPerGame,
    Dictionary<string, int> GamesPlayedPerWeek
    );