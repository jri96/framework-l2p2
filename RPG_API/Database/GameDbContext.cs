using Microsoft.EntityFrameworkCore;
using RPG_API.Entities;

namespace RPG_API.Database;

// De DbContext is de verbinding tussen de C#-classes (entities) en de MySQL-tabellen.
public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<Player> Players => Set<Player>();
}
