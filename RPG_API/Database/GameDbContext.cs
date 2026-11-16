using Microsoft.EntityFrameworkCore;
using RPG_API.Entities;

namespace RPG_API.Database;

// De DbContext is de verbinding tussen de C#-classes (entities) en de MySQL-tabellen.
public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Enemy> Enemies => Set<Enemy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seeding: deze vijanden staan na de migration direct in de database.
        // De stats komen uit de constructor van iedere subclass.
        modelBuilder.Entity<Skeleton>().HasData(new Skeleton { Id = 1 });
        modelBuilder.Entity<Goblin>().HasData(new Goblin { Id = 2 });
        modelBuilder.Entity<Orc>().HasData(new Orc { Id = 3 });
        modelBuilder.Entity<BellWarden>().HasData(new BellWarden { Id = 4 });
    }
}
