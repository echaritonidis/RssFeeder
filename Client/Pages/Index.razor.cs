using Microsoft.AspNetCore.Components;
using RssFeeder.Client.Events;
using RssFeeder.Shared.Extensions;
using RssFeeder.Shared.Model;
using System.Net.Http.Json;

namespace RssFeeder.Client.Pages;

public partial class Index : IDisposable
{
    [Inject] public HttpClient _httpClient { get; set; }

    [Inject] public NotifyEventService _notifyEventService { get; set; }

    protected List<FeedNavigation> FeedNavigations { get; set; }
    protected List<FeedContent> FeedContents { get; set; }

    protected bool ContentLoading { get; set; }

    public void Dispose()
    {
        this._notifyEventService.EventClick -= this.InvalidateFeed;
    }

    protected override async Task OnInitializedAsync()
    {
        _notifyEventService.EventClick += this.InvalidateFeed;

        FeedNavigations = await _httpClient.GetFromJsonAsync<List<FeedNavigation>>("/api/v1.0/Feed/GetAll");

        var defaultItem = FeedNavigations.SingleOrDefault(x => x.Default);

        if (defaultItem != null) OnSelectFeed(defaultItem);
    }

    protected void InvalidateFeed(object? sender, EventArgs e)
    {
        FeedNavigations.Add((FeedNavigation)sender);
        this.InvokeAsync(StateHasChanged);
    }

    protected async Task OnSelectFeed(FeedNavigation feedNavigation)
    {
        try
        {
            ContentLoading = true;

            var queryParams = new Dictionary<string, string>()
            {
                ["href"] = feedNavigation.Href
            };

            FeedContents = await _httpClient.GetFromJsonWithParamsAsync<List<FeedContent>>("/api/v1.0/Feed/GetContent", queryParams);
        }
        finally
        {
            ContentLoading = false;
        }

        StateHasChanged();
    }
}

