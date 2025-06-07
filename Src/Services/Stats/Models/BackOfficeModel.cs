namespace CasinoDeYann.Services.Stats.Models;

public record BackOfficeModel(IEnumerable<GameHistoryEntryModel> GameHistory, bool HasPrevious, bool HasNext)
{
    
}