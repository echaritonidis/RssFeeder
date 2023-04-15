namespace RssFeeder.Server.Infrastructure.Model;

public record Settings : BaseEntity
{
	public bool DarkMode { get; set; }
}

