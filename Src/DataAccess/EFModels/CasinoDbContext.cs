using Microsoft.EntityFrameworkCore;

namespace CasinoDeYann.DataAccess.EFModels;
public class CasinoDbContext : DbContext
{
    public CasinoDbContext(DbContextOptions<CasinoDbContext> options)
        : base(options) {}
    
    public DbSet<TUser> Users { get; set; }
    public DbSet<TStats> Stats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Les dates sont écrites avec DateTime.Now (heure locale, sans fuseau),
        // comme le faisait datetime2 sous SQL Server. Npgsql refuse d'écrire une
        // DateTime non-UTC dans "timestamp with time zone" (son type par défaut),
        // d'où le type sans fuseau. Le conteneur tourne en TZ=Europe/Paris.
        modelBuilder.Entity<TStats>()
            .Property(s => s.Date)
            .HasColumnType("timestamp without time zone");
    }
}
