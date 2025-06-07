namespace CasinoDeYann.Services.Stats;

public record BackOfficeModel(IEnumerable<GameHistoryEntryModel> GameHistory, bool HasPrevious, bool HasNext)
{
    
}