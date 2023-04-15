using System;

namespace RssFeeder.Shared.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Validates a URL.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static bool ValidateUrl(this string? url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out Uri validatedUri)) //.NET URI validation.
        {
            return (validatedUri.Scheme == Uri.UriSchemeHttp || validatedUri.Scheme == Uri.UriSchemeHttps);
        }

        return false;
    }
}
