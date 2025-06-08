using CasinoDeYann.DataAccess.Dbo;
using CasinoDeYann.DataAccess.Interfaces;
using CasinoDeYann.Services.User.Models;

namespace CasinoDeYann.Services.User;

public class UserService(IUsersRepository usersRepository) : IUserService
{
    public async Task<Models.User?> GetUser(string username)
    {
        var user = await usersRepository.GetOneByName(username);
        if (user == null) return null;
        return new Models.User(
            user.Id,
            user.Username,
            user.Money,
            user.Xp,
            user.Role
        );
    }
    
    public async Task<Models.User?> GetUser(long id)
    {
        var user = await usersRepository.GetOneById(id);
        if (user == null) return null;
        return new Models.User(
            user.Id,
            user.Username,
            user.Money,
            user.Xp,
            user.Role
        );
    }

    public async Task<Models.User> Pay(string username, long amount)
    {
        var user = await usersRepository.GetOneByName(username);
        if (user.Money < amount) 
            throw new BadHttpRequestException("You don't have enough money");
        
        user.Money -= amount;
        var updated = await usersRepository.Update(user);
        
        return new Models.User(
            updated.Id,
            updated.Username,
            updated.Money,
            updated.Xp,
            updated.Role
        );
    }

    public async Task<IEnumerable<Models.User>> GetLeaderboard()
    {
        var users = await usersRepository.Get();
        return users
            .OrderByDescending(u => u.Money)
            .Take(10)
            .Select(u => new Models.User(
                u.Id,
                u.Username,
                u.Money,
                u.Xp,
                u.Role
            ));
    }

    public async Task<bool> DeleteAccountAsync(string userName)
        => await usersRepository.DeleteOneByName(userName);

    public async Task<long> AddMoney(string name, long amount)
    {
        var user = await usersRepository.GetOneByName(name);
        user.Money += amount;
        var updated = await usersRepository.Update(user);
        return updated.Money;
    }

    public async Task<Models.User> AddExp(string name, long amount)
    {
        var user = await usersRepository.GetOneByName(name);
        user.Xp += amount;
        var updated = await usersRepository.Update(user);
        return new Models.User(
            updated.Id,
            updated.Username,
            updated.Money,
            updated.Xp,
            updated.Role
        );
    }

    public async Task<Models.User> ChangeRole(string username, string role)
    {
        var user = await usersRepository.GetOneByName(username);
        user.Role = role;
        var updated = await usersRepository.Update(user);
        return new Models.User(
            updated.Id,
            updated.Username,
            updated.Money,
            updated.Xp,
            updated.Role
        );
    }

    public async Task<Models.User> Update(Models.User userModel)
    {
        // Map Model.User → Dbo.User explicitly
        var dboUser = new DataAccess.Dbo.User
        {
            Id       = userModel.Id,
            Username = userModel.Username,
            Money    = userModel.Money,
            Xp       = userModel.Xp,
            Role     = userModel.Role
            // NOTE: Password is not set here; adjust if needed.
        };

        var updated = await usersRepository.Update(dboUser);

        return new Models.User(
            updated.Id,
            updated.Username,
            updated.Money,
            updated.Xp,
            updated.Role
        );
    }

    public async Task<IEnumerable<string>> Search(string query)
        => await usersRepository.FindUsernamesStartingWith(query);
}
