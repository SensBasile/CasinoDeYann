namespace CasinoDeYann.Services;

public record UserProfileModel(
    long Level,
    long Balance,
    List<GameHistoryEntryModel> History, 
    long HighestGain,
    int NumberOfGames,
    long TotalWon,
    long TotalLost,
    // FIXME: chiant int MaxWinStreak,
    Dictionary<string, int> GamesPlayedPerGame ,
    Dictionary<DateTime, int> GamesPlayedPerDay
    );