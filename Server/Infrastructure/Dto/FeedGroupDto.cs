namespace RssFeeder.Server.Infrastructure.Dto;

public class FeedGroupDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    public int Order { get; set; }
    public List<FeedDto> Feeds { get; set; }
}