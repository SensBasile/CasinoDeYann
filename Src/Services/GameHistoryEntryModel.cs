namespace CasinoDeYann.Services;

public record GameHistoryEntryModel(
    long Id,
    string Username,
    DateTime Date,
    string Game,
    long Bet,
    long Gain,
    bool IsCancelled
        );