using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RssFeeder.Client.Events;
using RssFeeder.Shared.Model;
using System.Net.Http.Json;
using RssFeeder.Client.Shared;
using RssFeeder.Client.Shared.Feed;

namespace RssFeeder.Client.Pages;

public partial class Index : IDisposable
{
    [Inject] public IJSRuntime _jsRuntime { get; set; } = default!;
    [Inject] public HttpClient _httpClient { get; set; } = default!;
    [Inject] public NotifyEventService _notifyEventService { get; set; } = default!;
    public List<FeedNavigationGroup> FeedNavigationGroups { get; set; } = default!;
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
        var response = await _httpClient.GetAsync("/api/v1.0/FeedNavigationGroup/GetAll");

        if (response.IsSuccessStatusCode)
        {
            FeedNavigationGroups = await response.Content.ReadFromJsonAsync<List<FeedNavigationGroup>>();
        }
    }

    protected void InvalidateFeed(object? sender, EventArgs e)
    {
        FeedNavigationGroups.Add((FeedNavigationGroup)sender!);
        this.InvokeAsync(StateHasChanged);
    }
    
    protected async Task ExportExcel()
    {
        var result = await _httpClient.PostAsJsonAsync("/api/v1.0/FeedNavigation/ExportExcel", FeedContents);

        if (result.IsSuccessStatusCode)
        {
            var content = await result.Content.ReadAsStringAsync();

            await _jsRuntime.InvokeAsync<object>("saveFile", "file.xlsx", content);
        }
    }
}