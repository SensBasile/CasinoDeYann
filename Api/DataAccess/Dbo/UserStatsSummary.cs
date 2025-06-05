using CasinoDeYann.Api.DataAccess.Dbo;


public class UserStatsSummary
{
    public long UserId { get; set; }
    public int NumberOfGames { get; set; }
    public long HighestGain { get; set; }
    public List<Stats> History { get; set; }
    public long TotalWon { get; set; }
    public long TotalLost { get; set; }
    public Dictionary<string, int> GamesPlayedPerGame { get; set; } = new();
    public Dictionary<DateTime, int> GamesPlayedPerDay { get; set; } = new();
}