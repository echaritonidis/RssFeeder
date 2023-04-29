namespace RssFeeder.Server.Infrastructure.Model;

public record FeedGroup : BaseEntity
{
    public string Title { get; init; }
    public string Description { get; init; }
    public string Color { get; init; }
    public int Order { get; init; }
    public virtual List<Feed> Feeds { get; init; }
}