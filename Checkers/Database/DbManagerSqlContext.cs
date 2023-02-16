using Entities;
using Microsoft.EntityFrameworkCore;

namespace IO;

public sealed class DbManagerSqlContext : DbContext
{
    public DbSet<GameState>? States { get; set; }
    public DbSet<Game>? Games { get; set; }
    public DbSet<Square>? Squares { get; set; }
    public DbSet<GameOptions>? GameOptions { get; set; }
    private const string SqliteName = "checkers.db";

    public DbManagerSqlContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={SqliteName}");
}
