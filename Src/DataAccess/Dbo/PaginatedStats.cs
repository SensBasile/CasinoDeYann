namespace CasinoDeYann.Src.DataAccess.Dbo;

public class PaginatedStats(IEnumerable<Stats> stats, int totalPages)
{
    public IEnumerable<Stats> Stats { get; } = stats;
    public int TotalPages { get; } = totalPages;
}
