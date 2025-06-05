using CasinoDeYann.Src.DataAccess.Dbo;
using CasinoDeYann.Src.DataAccess.EFModels;

namespace CasinoDeYann.Src.DataAccess.Interfaces;

public interface IStatsRepository : IRepository<TStats, Stats>
{
    public UserStatsSummary GetStats(long userId);
    Task<PaginatedStats> Get(string sortOrder, string searchString, int pageIndex);
}