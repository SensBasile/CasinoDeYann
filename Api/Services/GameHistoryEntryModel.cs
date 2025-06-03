namespace CasinoDeYann.Api.Services;

public record GameHistoryEntryModel(
        DateTime Date,
        string GameName,
        long Bet,
        long Gain
        );