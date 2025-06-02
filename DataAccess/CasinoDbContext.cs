using Microsoft.EntityFrameworkCore;

namespace CasinoDeYann.DataAccess;
public class CasinoDbContext : DbContext
{
    public CasinoDbContext(DbContextOptions<CasinoDbContext> options)
        : base(options) {}
}