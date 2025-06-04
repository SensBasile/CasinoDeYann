namespace CasinoDeYann.Api.Services;

public record GameHistoryEntryModel(
        // FIXME: pour l'instant on fait par id la chronologieDateTime Date,
        long UserId,
        string Game,
        long Bet,
        long Gain
        );