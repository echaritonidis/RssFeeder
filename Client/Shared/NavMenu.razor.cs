using Microsoft.AspNetCore.Components;
using RssFeeder.Client.Events;
using RssFeeder.Client.Shared.Feed;
using RssFeeder.Shared.Model;
using System.Net.Http.Json;

namespace RssFeeder.Client.Shared;

public partial class NavMenu
{
    [Inject] public HttpClient _httpClient { get; set; } = default!;

    [Inject] public NotifyEventService _notifyEventService { get; set; } = default!;

    private AddFeedModalView addFeedModalViewRef = default!;

    Task OnAddFeed()
    {
        return addFeedModalViewRef.ShowModal();
    }

    async Task OnSaveResponse(FeedNavigation feedNavigation)
    {
        // TODO: Do something with the response like show an alert
        await _httpClient.PostAsJsonAsync<FeedNavigation>("/api/v1.0/Feed", feedNavigation);

        _notifyEventService.NotifyEventClick(feedNavigation);
    }
}

