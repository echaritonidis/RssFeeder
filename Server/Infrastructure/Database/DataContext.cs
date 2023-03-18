using Microsoft.EntityFrameworkCore;
using RssFeeder.Server.Infrastructure.Model;

namespace RssFeeder.Server.Infrastructure.Database;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Feed>()
            .HasMany(p => p.Tags)
            .WithOne(c => c.Feed)
            .HasForeignKey(c => c.FeedId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<Feed> Feed { get; set; }
    public DbSet<Tags> Tags { get; set; }
    public DbSet<Settings> Settings { get; set; }
}