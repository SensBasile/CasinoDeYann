using CasinoDeYann.Src.DataAccess.Dbo;
using CasinoDeYann.Src.DataAccess.EFModels;

namespace CasinoDeYann.Src.DataAccess.Interfaces;

public interface IUsersRepository : IRepository<TUser, User>
{
    public Task<User> GetOneByName(string name);
    public Task<bool> DeleteOneByName(string name);
    Task<IEnumerable<string>> FindUsernamesStartingWith(string query);
}