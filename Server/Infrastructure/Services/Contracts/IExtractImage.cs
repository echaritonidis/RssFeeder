namespace RssFeeder.Server.Infrastructure.Services.Contracts
{
	public interface IExtractImage
    {
		public Task<string> GetImageBase64ByHref(string href);
	}
}

