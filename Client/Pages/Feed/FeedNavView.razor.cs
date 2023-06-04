using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RssFeeder.Client.Shared;
using RssFeeder.Shared.Extensions;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Pages.Feed;

public partial class FeedNavView
{
    [Parameter] public List<FeedNavigation>? Model { get; set; }
    [Parameter] public Func<bool, Task> OnContentLoadingCallback { get; set; }
    [Parameter] public Func<List<FeedContent>, Task> OnContentDataCallback { get; set; }
    [Parameter] public Func<MouseEventArgs, FeedNavigation, Task> OnMoreCallback { get; set; }
    [Inject] public HttpClient _httpClient { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var defaultNavigationItem = Model?.SingleOrDefault(x => x.Default);
        if (defaultNavigationItem != null) await OnSelectFeed(defaultNavigationItem);
    }
    
    private async Task OnFeedNavClickAsync(FeedNavigation item)
    {
        // If it's already selected skip
        if (Model.Single(x => x.Id == item.Id).Active) return;

        Model.ForEach(i => i.Active = false);
        item.Active = true;

        await OnSelectFeed(item);
    }

    private async Task OnSelectFeed(FeedNavigation feedNavigation)
    {
        try
        {
            await OnContentLoadingCallback(true);
            
            var queryParams = new Dictionary<string, string>()
            {
                ["href"] = feedNavigation.Href
            };

            var feedContents = await _httpClient.GetFromJsonWithParamsAsync<List<FeedContent>>("/api/v1.0/FeedNavigation/GetContent", queryParams);
            await OnContentDataCallback(feedContents);
        }
        finally
        {
            await OnContentLoadingCallback(false);
        }

        StateHasChanged();
    }
}

