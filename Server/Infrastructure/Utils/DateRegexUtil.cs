using System.Text.RegularExpressions;

namespace RssFeeder.Server.Infrastructure.Utils
{
	public partial class DateRegexUtil
	{
        [GeneratedRegex(@"\\w+, [0-9]+ \\w+ [0-9]+")]
        private partial Regex DateFormatRegex();

        private readonly Regex _dateFormatRegex;

        public Match IsMatch(string input) => _dateFormatRegex.Match(input);

        public DateRegexUtil()
        {
            _dateFormatRegex = DateFormatRegex();
        }
	}
}

