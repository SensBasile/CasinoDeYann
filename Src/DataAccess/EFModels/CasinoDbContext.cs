using Microsoft.EntityFrameworkCore;

namespace CasinoDeYann.Src.DataAccess.EFModels;
public class CasinoDbContext : DbContext
{
    public CasinoDbContext(DbContextOptions<CasinoDbContext> options)
        : base(options) {}
    
    public DbSet<TUser> Users { get; set; }
    public DbSet<TStats> Stats { get; set; }
}