namespace CasinoDeYann.Api.Services;

public record GameHistoryEntryModel(
        string Username,
        DateTime Date,
        string Game,
        long Bet,
        long Gain
        );