namespace RssFeeder.Server.Infrastructure.Model;

public record Tags : BaseEntity
{
	public string? Name { get; init; }
	public string? Color { get; init; }
    public Guid FeedId { get; init; }
    public virtual Feed Feed { get; init; }
}

