namespace CasinoDeYann.Api.DataAccess.Dbo;

public class Stats : EFModels.IObjectWithId, IObjectWithId
{
    public long Id { get; set; }
    public long Gain { get; set; }
    public long Bet { get; set; }
    public string Game { get; set; }
}