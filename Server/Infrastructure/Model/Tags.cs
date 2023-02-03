namespace RssFeeder.Server.Infrastructure.Model;

public record Tags : BaseEntity
{
	public string? Name { get; init; }
	public string? Color { get; init; }
}

