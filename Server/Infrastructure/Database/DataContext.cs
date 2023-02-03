using Microsoft.EntityFrameworkCore;
using RssFeeder.Server.Infrastructure.Model;

namespace RssFeeder.Server.Infrastructure.Database;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //modelBuilder.Entity<Feed>().HasData
        //(
        //    new Feed() { Id = Guid.NewGuid(), Name = "Fidle", Href = "https://fiddle.xml" },
        //    new Feed() { Id = Guid.NewGuid(), Name = "Orches", Href = "https://orches.xml" }
        //);
    }

    public DbSet<Feed> Feed { get; set; }
    public DbSet<Tags> Tags { get; set; }
    public DbSet<Settings> Settings { get; set; }
}