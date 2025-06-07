namespace CasinoDeYann.Services.Stats.Models;

public record UserStatsModel(
    IEnumerable<GameHistoryEntryModel> History,
    bool HasPrevious,
    bool HasNext,
    long HighestGain,
    int NumberOfGames,
    long TotalWon,
    long TotalLost,
    Dictionary<string, int> GamesPlayedPerGame ,
    Dictionary<DateTime, int> GamesPlayedPerDay
    );