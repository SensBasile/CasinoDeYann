using CasinoDeYann.DataAccess.EfModels;
using CasinoDeYann.Dbo;

namespace CasinoDeYann.DataAccess.Interfaces;

public interface IUsersRepository : IRepository<TUser, User>
{
    public User GetOneByName(string name);
    public Task<User> AddMoney(string name, int amount);
}