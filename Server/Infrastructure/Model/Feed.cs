namespace RssFeeder.Server.Infrastructure.Model;

public record Feed : BaseEntity
{
    public DateTime CreatedAt { get; init; }
    public DateTime ModifiedAt { get; init; }
    public string Title { get; init; }
    public string Href { get; init; }
    public bool Favorite { get; init; }
    public bool Default { get; init; }
    public Guid FeedGroupId { get; init; }
    public virtual FeedGroup FeedGroup { get; init; }
    public virtual List<Label>? Labels { get; init; }
}