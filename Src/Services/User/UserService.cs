using AutoMapper;
using CasinoDeYann.DataAccess.Dbo;
using CasinoDeYann.DataAccess.Interfaces;
using CasinoDeYann.Services.Stats;
using CasinoDeYann.Services.User.Models;

namespace CasinoDeYann.Services.User;

public class UserService(IUsersRepository usersRepository, IMapper mapper) : IUserService
{
    public async Task<Models.User> GetUser(string username)
    {
        var user = await usersRepository.GetOneByName(username);
        return mapper.Map<Models.User>(user);
    }
    
    public async Task<Models.User> GetUser(long id)
    {
        var user = await usersRepository.GetOneById(id);
        return mapper.Map<Models.User>(user);
    }

    public async Task<Models.User> Pay(string username, long amount)
    {
        var user = await usersRepository.GetOneByName(username);
        if (user.Money < amount) throw new BadHttpRequestException("You don't have enough money");
        user.Money -= amount;
        var updated = await usersRepository.Update(user);
        return mapper.Map<Models.User>(updated);
    }

    public async Task<IEnumerable<Models.User>> GetLeaderboard()
    {
        var users = await usersRepository.Get();
        return users
            .OrderByDescending(u => u.Money)
            .Take(10)
            .Select(mapper.Map<Models.User>);
    }

    public async Task<bool> DeleteAccountAsync(string userName)
    {
        return await usersRepository.DeleteOneByName(userName);
    }

    public async Task<long> AddMoney(string name, long amount)
    {
        var user = await usersRepository.GetOneByName(name);
        user.Money += amount;
        return (await usersRepository.Update(user)).Money;
        ;
    }

    public async Task<Models.User> AddExp(string name, long amount)
    {
        var user = await usersRepository.GetOneByName(name);
        user.Xp += amount;
        var updated = await usersRepository.Update(user);
        return mapper.Map<Models.User>(updated);
    }

    public async Task<Models.User> ChangeRole(string username, string role)
    {
        var user = await usersRepository.GetOneByName(username);
        user.Role = role;
        var updated = await usersRepository.Update(user);
        return mapper.Map<Models.User>(updated);
    }

    public async Task<Models.User> Update(Models.User user)
    {
        var dboUser = mapper.Map<DataAccess.Dbo.User>(user);
        var updated = await usersRepository.Update(dboUser);
        return mapper.Map<Models.User>(updated);
    }

    public async Task<IEnumerable<string>> Search(string query)
    {
        return await usersRepository.FindUsernamesStartingWith(query);
    }
}
