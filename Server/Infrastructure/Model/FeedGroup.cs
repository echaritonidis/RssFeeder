namespace RssFeeder.Server.Infrastructure.Model;

public record FeedGroup : BaseEntity
{
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public string Color { get; init; } = "#000";
    public int Order { get; init; }
    public bool Initial { get; init; }
    public virtual List<Feed> Feeds { get; init; } = new();
}