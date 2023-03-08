using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RssFeeder.Client.Events;
using RssFeeder.Shared.Extensions;
using RssFeeder.Shared.Model;
using System.Net.Http.Json;

namespace RssFeeder.Client.Pages;

public partial class Index : IDisposable
{
    [Inject] public IJSRuntime _jsRuntime { get; set; } = default!;
    [Inject] public HttpClient _httpClient { get; set; } = default!;
    [Inject] public NotifyEventService _notifyEventService { get; set; } = default!;

    public List<FeedNavigation> FeedNavigations { get; set; } = default!;
    public List<FeedContent> FeedContents { get; set; } = default!;

    protected bool ContentLoading { get; set; }

    public void Dispose()
    {
        this._notifyEventService.EventClick -= this.InvalidateFeed;
    }

    protected override async Task OnInitializedAsync()
    {
        _notifyEventService.EventClick += this.InvalidateFeed;

        await LoadData();
    }

    protected async Task LoadData()
    {
        var response = await _httpClient.GetAsync("/api/v1.0/Feed/GetAll");

        if (response.IsSuccessStatusCode)
        {
            FeedNavigations = await response.Content.ReadFromJsonAsync<List<FeedNavigation>>();

            if (FeedNavigations != null)
            {
                var defaultItem = FeedNavigations.SingleOrDefault(x => x.Default);
                if (defaultItem != null) await OnSelectFeed(defaultItem);
            }
        }
    }

    protected void InvalidateFeed(object? sender, EventArgs e)
    {
        FeedNavigations.Add((FeedNavigation)sender!);
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

    protected async Task OnDefaultToggle(FeedNavigation feedNavigation)
    {
        // TODO: Add single action to update those bools instead of whole object
        await _httpClient.PutAsJsonAsync("/api/v1.0/Feed/Update", feedNavigation);
    }

    protected async Task OnFavoriteToggle(FeedNavigation feedNavigation)
    {
        // TODO: Add single action to update those bools instead of whole object
        await _httpClient.PutAsJsonAsync("/api/v1.0/Feed/Update", feedNavigation);
    }

    protected async Task ExportExcel()
    {
        var result = await _httpClient.PostAsJsonAsync("/api/v1.0/Feed/ExportExcel", FeedContents);

        if (result.IsSuccessStatusCode)
        {
            var content = await result.Content.ReadAsStringAsync();

            await _jsRuntime.InvokeAsync<object>("saveFile", "file.xlsx", content);
        }
    }
}

