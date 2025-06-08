namespace CasinoDeYann.Services.User
{
    public interface IUserService
    {
        Task<User.Models.User> GetUser(long id);
        Task<User.Models.User> GetUser(string username);
        Task<long> AddMoney(string name, long amount);
        Task<User.Models.User> AddExp(string name, long amount);
        Task<bool> DeleteAccountAsync(string userName);
        Task<User.Models.User> ChangeRole(string username, string role);

        public Task<Models.User> Pay(string username, long amount);
        Task<IEnumerable<Models.User>> GetLeaderboard();
        Task<IEnumerable<string>> Search(string query);
    }
}