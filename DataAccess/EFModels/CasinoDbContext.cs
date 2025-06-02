using Microsoft.EntityFrameworkCore;

namespace CasinoDeYann.DataAccess.EfModels;
public class CasinoDbContext : DbContext
{
    public CasinoDbContext(DbContextOptions<CasinoDbContext> options)
        : base(options) {}
    
    public DbSet<TUser> Users { get; set; }
}