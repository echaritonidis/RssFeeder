namespace RssFeeder.Server.Infrastructure.Dto;

public class FeedDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string Title { get; set; }
    public string Href { get; set; }
    public List<LabelDto> Labels { get; set; }
    public bool Favorite { get; set; }
    public bool Default { get; set; }
}