using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.DataAccess.EFModels;

namespace CasinoDeYann.Api.DataAccess.Interfaces;

public interface IUsersRepository : IRepository<TUser, User>
{
    public User GetOneByName(string name);
    public Task<User> AddMoney(string name, long amount);
}