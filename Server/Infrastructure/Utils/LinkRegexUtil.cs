using System.Text.RegularExpressions;

namespace RssFeeder.Server.Infrastructure.Utils
{
	public partial class LinkRegexUtil
    {
        [GeneratedRegex(@"^(http|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$")]
        private partial Regex LinkFormatRegex();

        private readonly Regex _linkFormatRegex;

        public Match IsMatch(string input) => _linkFormatRegex.Match(input);

        public LinkRegexUtil()
        {
            _linkFormatRegex = LinkFormatRegex();
        }
	}
}

