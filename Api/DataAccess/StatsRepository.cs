using AutoMapper;
using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.DataAccess.EFModels;
using CasinoDeYann.Api.DataAccess.Interfaces;

namespace CasinoDeYann.Api.DataAccess;

public class StatsRepository: Repository<TStats, Stats>, IStatsRepository
{
    public StatsRepository(CasinoDbContext context, ILogger<StatsRepository> logger, IMapper mapper) : base(context, logger, mapper)
    {
    }


    public UserStatsSummary GetStats(long userId)
    {
        return new UserStatsSummary
        {
            UserId = userId,
            NumberOfGames = _context.Stats.Where(s => s.UserId == userId).Select(s => s.Game).Distinct().Count(),
            HighestGain = _context.Stats.Where(s => s.UserId == userId).Max(s => (long?)s.Gain) ?? 0,
            Last15Stats = _mapper.Map<List<Stats>>(_context.Stats.Where(s => s.UserId == userId)
                .OrderByDescending(s => s.Id)
                .Take(15)
                .ToList())
        };
    }
    
}