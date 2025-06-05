namespace CasinoDeYann.Api.Services;

public record BackOfficeModel(IEnumerable<GameHistoryEntryModel> GameHistory, bool HasPrevious, bool HasNext)
{
    
}