namespace CasinoDeYann.DataAccess.EFModels;

public class TStats : IObjectWithId
{
    public long Id { get; set; }
    
    public DateTime  Date { get; set; }
    public long Gain { get; set; }
    public long Bet { get; set; }
    public string Game { get; set; }
    
    public long UserId { get; set; }
    
    public virtual TUser User { get; set; }
    
    public bool IsCanceled { get; set; }
}