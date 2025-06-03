using CasinoDeYann.DataAccess.EfModels;
using CasinoDeYann.Dbo;

namespace CasinoDeYann.DataAccess.Interfaces;

public interface IUsersRepository : IRepository<TUser, User>
{
    public Task<User> getOneByName(string name);
}