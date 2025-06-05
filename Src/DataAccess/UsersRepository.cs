using AutoMapper;
using CasinoDeYann.Src.DataAccess.Dbo;
using CasinoDeYann.Src.DataAccess.EFModels;
using CasinoDeYann.Src.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CasinoDeYann.Src.DataAccess;

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

    public async Task<IEnumerable<string>> FindUsernamesStartingWith(string query)
    {
        return await _context.Users
            .Where(u => u.Username.StartsWith(query))
            .OrderBy(u => u.Username)
            .Select(u => u.Username)
            .Take(5)
            .ToListAsync();
    }
}