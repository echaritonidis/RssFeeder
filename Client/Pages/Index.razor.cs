using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RssFeeder.Client.Events;
using RssFeeder.Shared.Extensions;
using RssFeeder.Shared.Model;
using System.Net.Http.Json;
using RssFeeder.Client.Shared;
using RssFeeder.Client.Shared.Feed;

namespace RssFeeder.Client.Pages;

public partial class Index : IDisposable
{
    [CascadingParameter] public MainLayout _mainLayout { get; set; }
    [Inject] public IJSRuntime _jsRuntime { get; set; } = default!;
    [Inject] public HttpClient _httpClient { get; set; } = default!;
    [Inject] public NotifyEventService _notifyEventService { get; set; } = default!;
    
    public List<FeedNavigation> FeedNavigations { get; set; } = default!;
    public List<FeedContent> FeedContents { get; set; } = default!;

    protected bool ContentLoading { get; set; }
    
    private EditFeedModalView editFeedModalViewRef = default!;

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


        // At first reset the Default of all elements
        await _httpClient.PutAsJsonAsync("/api/v1.0/Feed/ResetDefault", FeedNavigations.Select(x => x.Id));

        await _httpClient.PutAsJsonAsync("/api/v1.0/Feed/Update", feedNavigation);
    }

    protected async Task OnFavoriteToggle(FeedNavigation feedNavigation)
    {
        // TODO: Add single action to update those bools instead of whole object
        await _httpClient.PutAsJsonAsync("/api/v1.0/Feed/Update", feedNavigation);
    }

    protected async Task OnShowEditContext(FeedNavigation feedNavigation)
    {
        void HandleUpdateResult(bool updated)
        {
            if (updated) StateHasChanged();
            else
            {
                _mainLayout.ShowError("We apologize, but we could not update the requested information. Please check your input and try again. If the issue persists, contact support.");
            }
        }

        async Task SaveModalCallback(FeedNavigation updatedFeed)
        {
            var updateResponse = await _httpClient.PutAsJsonAsync($"/api/v1.0/Feed/Update", updatedFeed);

            if (updateResponse.IsSuccessStatusCode)
            {
                StateHasChanged();

                HandleUpdateResult(true);
                return;
            }

            HandleUpdateResult(false);
        }

        await editFeedModalViewRef.ShowModal(feedNavigation, SaveModalCallback);
    }

    protected async Task OnDeleteNavigation(Guid id)
    {
        void HandleDeleteResult(bool deleted)
        {
            if (deleted) FeedNavigations.RemoveAll(x => x.Id == id);
            else
            {
                _mainLayout.ShowError("Sorry, we were unable to delete the requested item. Please try again later or contact support.");
            }
            
            StateHasChanged();
        }
        
        var deletedResponse = await _httpClient.DeleteAsync($"/api/v1.0/Feed/Delete?id={id}");

        if (deletedResponse.IsSuccessStatusCode)
        {
            FeedContents = default!;
            
            HandleDeleteResult(true);
            return;
        }
        
        HandleDeleteResult(false);
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