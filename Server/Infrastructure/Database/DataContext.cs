using Microsoft.EntityFrameworkCore;
using RssFeeder.Server.Infrastructure.Model;

namespace RssFeeder.Server.Infrastructure.Database;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FeedGroup>()
            .HasMany(p => p.Feeds)
            .WithOne(c => c.FeedGroup)
            .HasForeignKey(c => c.FeedGroupId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Feed>()
            .HasMany(p => p.Labels)
            .WithOne(c => c.Feed)
            .HasForeignKey(c => c.FeedId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Add default group
        modelBuilder.Entity<FeedGroup>().HasData(new FeedGroup { Title = "Unclassified", Description = "Generic specific category or topic" });
    }

    public DbSet<FeedGroup> FeedGroup { get; set; }
    public DbSet<Feed> Feed { get; set; }
    public DbSet<Label> Label { get; set; }
    public DbSet<Settings> Settings { get; set; }
}