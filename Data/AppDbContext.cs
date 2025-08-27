using Microsoft.EntityFrameworkCore;
using ReactGamesListAPI.Models;

namespace ReactGamesListAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserGameList> UserGameLists => Set<UserGameList>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserGameList>()
            .HasKey(ugl => new { ugl.UserId, ugl.GameId });
    }

    // public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    // {
    //     var deletedEntries = base.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted);
    //
    //     foreach (var entry in deletedEntries)
    //     {
    //         if (entry.Entity is BaseEntity user)
    //         {
    //             user.DeleteTime = DateTime.Now;
    //         }
    //     }
    //     
    //     return base.SaveChangesAsync(cancellationToken);
    // }
}