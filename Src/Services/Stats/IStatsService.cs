using CasinoDeYann.DataAccess.Dbo;
using CasinoDeYann.Services.Stats.Models;
using CasinoDeYann.Services.User.Models;

namespace CasinoDeYann.Services.Stats
{
    public interface IStatsService
    {
        Task<UserStatsSummary> GetUserStats(long userId);
        Task<UserStatsModel> GetPlayerStats(string sortOrder, User.Models.User user, int pageIndex);
        Task<GameHistoryEntryModel> Create(GameHistoryEntryModel model);
        Task<BackOfficeModel> GetBackOffice(string sortOrder, string searchString, int pageIndex);
        Task<DataAccess.Dbo.Stats> Cancel(int id);
        public Task<UserProfileModel> GetUserProfileAsync(string sortOrder, string userName, int pageIndex);
    }
}