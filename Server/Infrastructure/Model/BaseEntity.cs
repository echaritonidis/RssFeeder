namespace RssFeeder.Server.Infrastructure.Model;

public abstract record BaseEntity 
{
    public Guid Id { get; set; }
}