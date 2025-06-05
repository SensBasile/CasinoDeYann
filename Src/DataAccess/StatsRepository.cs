using AutoMapper;
using CasinoDeYann.Src.DataAccess.Dbo;
using CasinoDeYann.Src.DataAccess.EFModels;
using CasinoDeYann.Src.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CasinoDeYann.Src.DataAccess;

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

    public async Task<PaginatedStats> Get(string sortOrder, string searchString, int pageIndex)
    {
        var stats = _context.Stats
            .Include(s => s.User)
            .AsNoTracking();
        
        if (!String.IsNullOrEmpty(searchString))
        {
            stats= stats.Where(s => s.User.Username.Contains(searchString));
        }
        
        switch (sortOrder)
        {
            case "bet_asc":
                stats = stats.OrderBy(s => s.Bet);
                break;
            case "bet_desc":
                stats = stats.OrderByDescending(s => s.Bet);
                break;
            case "gain_asc":
                stats = stats.OrderBy(s => s.Gain);
                break;
            case "gain_desc":
                stats = stats.OrderByDescending(s => s.Gain);
                break;
            case "date_asc":
                stats = stats.OrderBy(s => s.Date);
                break;
            case "date_desc":
                stats = stats.OrderByDescending(s => s.Date);
                break;
            default:
                stats = stats.OrderByDescending(s => s.Date);
                break;
        }
        var items = await stats.Skip(
                (pageIndex - 1) * 15)
            .Take(15).ToListAsync();
        return new PaginatedStats(_mapper.Map<Stats[]>(items), stats.Count() / 15 + 1);
    }
}