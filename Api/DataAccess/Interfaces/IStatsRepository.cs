using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.DataAccess.EFModels;

namespace CasinoDeYann.Api.DataAccess.Interfaces;

public interface IStatsRepository : IRepository<TStats, Stats>
{
    public UserStatsSummary GetStats(long userId);
}