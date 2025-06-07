namespace CasinoDeYann.DataAccess.Dbo;

public class Stats : EFModels.IObjectWithId, IObjectWithId
{
    public long Id { get; set; }
    
    public DateTime Date { get; set; }
    public long Gain { get; set; }
    public long Bet { get; set; }
    
    public string Game { get; set; }
    
    public long UserId { get; set; }
    
    public User User { get; set; }
    public bool IsCanceled { get; set; }
}