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

    public async Task<User> GetOneByName(string name)
    {
        try
        { 
            var query = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == name);
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

    public async Task<bool> DeleteOneByName(string name)
    {
        User user = await GetOneByName(name);
        return await Delete(user.Id);
    }
}