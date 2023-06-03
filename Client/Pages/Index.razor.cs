using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RssFeeder.Shared.Model;
using System.Net.Http.Json;
using RssFeeder.Client.Events;

namespace RssFeeder.Client.Pages;

public partial class Index : IDisposable
{
    [Inject] public IJSRuntime _jsRuntime { get; set; } = default!;
    [Inject] public HttpClient _httpClient { get; set; } = default!;
    [Inject] public NotifyEventService _notifyEventService { get; set; } = default!;
    public List<FeedNavigationGroup>? FeedNavigationGroups { get; set; }
    public List<FeedContent>? FeedContents { get; set; }
    protected bool ContentLoading { get; set; }
    
    public void Dispose()
    {
        _notifyEventService.InvalidateFeedNavigationGroupClick -= this.InvalidateFeedNavigationGroup;
        _notifyEventService.InvalidateFeedNavigationClick -= this.InvalidateFeedNavigation;
    }

    protected override async Task OnInitializedAsync()
    {
        _notifyEventService.InvalidateFeedNavigationGroupClick += this.InvalidateFeedNavigationGroup;
        _notifyEventService.InvalidateFeedNavigationClick += this.InvalidateFeedNavigation;
        
        var response = await _httpClient.GetAsync("/api/v1.0/FeedNavigationGroup/GetFeedGroups");

        if (response.IsSuccessStatusCode)
        {
            FeedNavigationGroups = await response.Content.ReadFromJsonAsync<List<FeedNavigationGroup>>();
        }
    }
    
    private void InvalidateFeedNavigationGroup(object? sender, EventArgs e)
    {
        FeedNavigationGroups ??= new();
        FeedNavigationGroups.Add((FeedNavigationGroup)sender!);
        
        this.InvokeAsync(StateHasChanged);
    }

    private void InvalidateFeedNavigation(object? sender, EventArgs e)
    {
        var newFeedNavigation = (FeedNavigation)sender!;
        var feedNavigationGroup = FeedNavigationGroups?.Where(g => g.Id == newFeedNavigation.GroupId).FirstOrDefault();
        
        if (feedNavigationGroup is null) return;
        
        feedNavigationGroup.FeedNavigations ??= new();
        feedNavigationGroup.FeedNavigations.Add(newFeedNavigation);
        
        this.InvokeAsync(StateHasChanged);
    }
    
    // protected async Task ExportExcel()
    // {
    //     var result = await _httpClient.PostAsJsonAsync("/api/v1.0/FeedNavigation/ExportExcel", FeedContents);
    //
    //     if (result.IsSuccessStatusCode)
    //     {
    //         var content = await result.Content.ReadAsStringAsync();
    //
    //         await _jsRuntime.InvokeAsync<object>("saveFile", "file.xlsx", content);
    //     }
    // }
}