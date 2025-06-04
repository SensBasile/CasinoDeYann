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
        var userStats = _context.Stats.Where(s => s.UserId == userId).ToList();

        var totalWon = userStats.Sum(s => s.Gain);
        var totalLost = userStats.Sum(s => s.Bet);

        var gamesPlayedPerGame = userStats
            .GroupBy(s => s.Game)
            .ToDictionary(g => g.Key, g => g.Count());

        var gamesPlayedPerDay = userStats
            .GroupBy(s => s.Date.Date)
            .ToDictionary(g => g.Key, g => g.Count());

        return new UserStatsSummary
        {
            UserId = userId,
            NumberOfGames = userStats.Count(),
            HighestGain = userStats.Max(s => (long?)s.Gain) ?? 0,
            History = _mapper.Map<List<Stats>>(userStats
                .OrderByDescending(s => s.Date)
                .Take(15)
                .ToList()),
            TotalWon = totalWon,
            TotalLost = totalLost,
            GamesPlayedPerGame = gamesPlayedPerGame,
            GamesPlayedPerDay = gamesPlayedPerDay,
        };
    }
    
}