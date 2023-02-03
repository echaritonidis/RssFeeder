using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace RssFeeder.Shared.Extensions;

public static class HttpClientExtensions
{
    public static async Task<TOutput> GetFromJsonWithParamsAsync<TOutput>(this HttpClient client, string uri, Dictionary<string, string> queryParams)
    {
        var url = QueryHelpers.AddQueryString(uri, queryParams);

        return await client.GetFromJsonAsync<TOutput>(url);
    }
}

