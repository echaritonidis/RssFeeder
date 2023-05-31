using Microsoft.AspNetCore.Components;
using RssFeeder.Client.Events;
using RssFeeder.Shared.Model;
using System.Net.Http.Json;
using RssFeeder.Client.Pages.Feed;

namespace RssFeeder.Client.Shared;

public partial class NavMenu
{
    [Inject] public HttpClient _httpClient { get; set; } = default!;
    [Inject] public NotifyEventService _notifyEventService { get; set; } = default!;
    public List<FeedNavigationGroupNames>? FeedNavigationGroupNames { get; set; }

    private AddFeedNavigationGroupModalView _addFeedNavigationGroupModalViewRef = default!;
    private AddFeedNavigationModalView _addFeedNavigationModalViewRef = default!;

    private Task ShowFeedNavigationGroupCreateModal() => _addFeedNavigationGroupModalViewRef.ShowModal();
    private Task ShowFeedNavigationCreateModal() => _addFeedNavigationModalViewRef.ShowModal();

    protected override async Task OnInitializedAsync()
    {
        var response = await _httpClient.GetAsync("/api/v1.0/FeedNavigationGroup/GetGroupList");

        if (response.IsSuccessStatusCode)
        {
            FeedNavigationGroupNames = await response.Content.ReadFromJsonAsync<List<FeedNavigationGroupNames>>();
        }
    }

    private async Task OnSaveGroupResponse(FeedNavigationGroup feedNavigationGroup)
    {
        // TODO: Do something with the response like show an alert
        await _httpClient.PostAsJsonAsync<FeedNavigationGroup>("/api/v1.0/FeedNavigationGroup", feedNavigationGroup);

        _notifyEventService.NotifyFeedNavigationGroupClick(feedNavigationGroup);
    }
    
    private async Task OnSaveFeedResponse(FeedNavigation feedNavigation)
    {
        // TODO: Do something with the response like show an alert
        await _httpClient.PostAsJsonAsync<FeedNavigation>("/api/v1.0/FeedNavigation", feedNavigation);

        _notifyEventService.NotifyFeedNavigationClick(feedNavigation);
    }
}

