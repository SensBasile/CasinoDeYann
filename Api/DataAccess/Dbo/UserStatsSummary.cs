using CasinoDeYann.Api.DataAccess.Dbo;


public class UserStatsSummary
{
    public long UserId { get; set; }
    public int NumberOfGames { get; set; }
    public long HighestGain { get; set; }
    public List<Stats> History { get; set; }
}