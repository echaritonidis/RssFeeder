using System.Xml.Linq;
using RssFeeder.Server.Infrastructure.Services.Contracts;
using RssFeeder.Server.Infrastructure.Utils;
using RssFeeder.Shared.Extensions;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Infrastructure.Services.Implementations
{
	public class ExtractContent : IExtractContent
	{
        private readonly IExtractImage _extractImage;
        private readonly DateRegexUtil _dateRegexUtil;

        public ExtractContent(IExtractImage extractImage, DateRegexUtil dateRegexUtil)
        {
            _extractImage = extractImage;
            _dateRegexUtil = dateRegexUtil;
        }

        public async Task<List<FeedContent>> GetContentItems(string xmlContent)
        {
            List<FeedContent> result = new();

            var xml = XDocument.Parse(xmlContent);

            var channel = xml.Descendants("channel");
            var items = channel.Descendants("item");

            foreach (var item in items)
            {
                var link = item.GetElement("link");

                if (string.IsNullOrEmpty(link))
                {
                    link = item.GetElement("guid");
                }

                var pubDate = item.GetElement("pubDate");
                var match = _dateRegexUtil.IsMatch(pubDate);

                if (match.Success)
                {
                    pubDate = match.Value;
                }

                result.Add
                (
                    new FeedContent
                    {
                        Title = item.GetElement("title"),
                        Link = link,
                        ImageBase64 = await _extractImage.GetImageBase64ByHref(link),
                        Description = item.GetElement("description"),
                        PubDate = pubDate
                    }
                );
            }

            return result;
        }
    }
}

