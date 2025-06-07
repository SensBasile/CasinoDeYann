using CasinoDeYann.DataAccess.Dbo;
using CasinoDeYann.DataAccess.EFModels;

namespace CasinoDeYann.DataAccess.Interfaces;

public interface IUsersRepository : IRepository<TUser, User>
{
    public Task<User> GetOneByName(string name);
    public Task<bool> DeleteOneByName(string name);
    Task<IEnumerable<string>> FindUsernamesStartingWith(string query);
}