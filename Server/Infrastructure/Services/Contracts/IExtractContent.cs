using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Infrastructure.Services.Contracts
{
	public interface IExtractContent
	{
        public Task<List<FeedContent>> GetContentItems(string xmlContent);
    }
}

