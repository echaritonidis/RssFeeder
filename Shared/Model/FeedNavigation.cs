namespace RssFeeder.Shared.Model;

public class FeedNavigation
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Href { get; set; }
    public List<FeedLabel>? FeedLabels { get; set; }
    public bool Active { get; set; }
    public bool Favorite { get; set; }
    public bool Default { get; set; }
}