using CasinoDeYann.DataAccess.Dbo;
using CasinoDeYann.DataAccess.EFModels;

namespace CasinoDeYann.DataAccess.Interfaces;

public interface IStatsRepository : IRepository<TStats, Stats>
{
    public UserStatsSummary GetStats(long userId);
    Task<PaginatedStats> Get(string sortOrder, string searchString, int pageIndex);
}