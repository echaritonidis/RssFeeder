using System.Xml.Linq;

namespace RssFeeder.Shared.Extensions;

public static class XmlExtensions
{
    public static string GetElement(this XElement item, string name)
    {
        return item.Element(name)?.Value ?? string.Empty;
    }
}
