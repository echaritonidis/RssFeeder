namespace RssFeeder.Shared.Model;

public record FeedContent
{
	public string Title { get; init; }
	public string Link { get; init; }
	public string ImageBase64 { get; init; }

    public string Description { get; init; }
	public string PubDate { get; init; }
}

