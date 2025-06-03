using AutoMapper;
using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.DataAccess.EFModels;
using CasinoDeYann.Api.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CasinoDeYann.Api.DataAccess;

public class UsersRepository : Repository<TUser, User>, IUsersRepository
{
    public UsersRepository(CasinoDbContext context, ILogger<UsersRepository> logger, IMapper mapper) : base(context, logger, mapper)
    { }

    public User GetOneByName(string name)
    {
        try
        { 
            var query = _context.Users.AsNoTracking().FirstOrDefault(x => x.Username == name);
            if (query == null)
                return null;
            return _mapper.Map<User>(query);
        }
        catch (Exception ex)
        {
            _logger.LogError("error on db", ex);
            return null;
        }
    }

    public async Task<User> AddMoney(string name, long amount)
    {
        var user = GetOneByName(name);
        user.Money += amount;
        return await Update(user);
    }
}