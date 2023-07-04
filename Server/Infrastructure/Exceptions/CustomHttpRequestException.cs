using System.Net;

namespace RssFeeder.Server.Infrastructure.Exceptions
{
    public class CustomHttpRequestException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public string ReasonPhrase { get; }

        public CustomHttpRequestException(HttpStatusCode statusCode, string reasonPhrase) : base($"HTTP request failed with status code {(int)statusCode} ({statusCode}): {reasonPhrase}")
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
        }
    }
}
