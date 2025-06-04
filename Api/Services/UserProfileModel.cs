namespace CasinoDeYann.Api.Services;

public record UserProfileModel(
    long Level,
    long Balance,
    List<GameHistoryEntryModel> History, 
    long HighestGain,
    int NumberOfGames,
    long TotalWon,
    long TotalLost,
    // FIXME: chiant int MaxWinStreak,
    Dictionary<string, int> GamesPlayedPerGame
    // FIXME: on a plus la temporalité Dictionary<string, int> GamesPlayedPerWeek
    );