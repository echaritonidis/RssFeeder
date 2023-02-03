using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using RssFeeder.Server.Infrastructure.Services.Contracts;
using RssFeeder.Server.Infrastructure.Utils;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Infrastructure.Services.Implementations
{
	public class ExtractContent : IExtractContent
	{
        private readonly DateRegexUtil _dateRegexUtil;

        public ExtractContent(DateRegexUtil dateRegexUtil)
        {
            _dateRegexUtil = dateRegexUtil;
        }

        public List<FeedContent> GetContentItems(string xmlContent)
        {
            List<FeedContent> result = new();

            var xml = XDocument.Parse(xmlContent);

            var channel = xml.Descendants("channel");
            var items = channel.Descendants("item");

            foreach (var item in items)
            {
                var pubDate = ((string)item.Element("pubDate"));
                var match = _dateRegexUtil.IsMatch(pubDate);

                if (match.Success)
                {
                    pubDate = match.Value;
                }

                result.Add
                (
                    new FeedContent
                    {
                        Title = ((string)item.Element("title")),
                        Link = ((string)item.Element("link")),
                        Description = ((string)item.Element("description")),
                        PubDate = pubDate
                    }
                );
            }

            return result;
        }
    }
}

