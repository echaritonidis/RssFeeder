using Microsoft.AspNetCore.Components;
using RssFeeder.Client.Events;
using RssFeeder.Client.Shared.Feed;
using RssFeeder.Shared.Model;
using System.Net.Http.Json;

namespace RssFeeder.Client.Shared;

public partial class NavMenu
{
    [Inject] public HttpClient _httpClient { get; set; }

    [Inject] public NotifyEventService _notifyEventService { get; set; }

    private AddFeedModalView addFeedModalViewRef;

    Task OnAddFeed()
    {
        return addFeedModalViewRef.ShowModal();
    }

    async Task OnSaveResponse(FeedNavigation feedNavigation)
    {
        await _httpClient.PostAsJsonAsync<FeedNavigation>("Feed", feedNavigation);

        _notifyEventService.NotifyEventClick(feedNavigation);
    }
}

