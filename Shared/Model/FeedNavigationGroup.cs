namespace RssFeeder.Shared.Model;

public class FeedNavigationGroup
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Color { get; set; }
    public int Order { get; set; }
    public bool Initial { get; set; }
    public List<FeedNavigation>? FeedNavigations { get; set; }
}