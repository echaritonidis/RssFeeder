using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RssFeeder.Client.Shared;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Pages.Feed;

public partial class FeedNavGroupView : IDisposable
{
    [CascadingParameter] public MainLayout _mainLayout { get; set; }
    [Parameter] public List<FeedNavigationGroup>? Model { get; set; }
    [Parameter] public bool ContentLoading { get; set; }
    [Parameter] public List<FeedContent> FeedContent { get; set; }
    [Parameter] public EventCallback<bool> ContentLoadingChanged { get; set; }
    [Parameter] public EventCallback<List<FeedContent>> FeedContentChanged { get; set; }
    [Inject] public IJSRuntime _jsRuntime { get; set; } = default!;
    [Inject] public HttpClient _httpClient { get; set; } = default!;
    [JSInvokable] public static async Task CloseDropDown() => await _onDomClickEventCallback.Invoke();

    private static Func<Task> _onDomClickEventCallback = default!;
    private FeedNavigation? _selectedFeedItem;
    private EditFeedModalView _editFeedModalViewRef = default!;
    private FeedNavOption _feedNavOptionRef = default!;

    public void Dispose()
    {
        _jsRuntime.InvokeVoidAsync("unbindDomChanges");
    }

    protected override async Task OnInitializedAsync()
    {
        _onDomClickEventCallback = OnDomClickEventValueAsync;

        await _jsRuntime.InvokeVoidAsync("bindDomChanges");
    }

    private Task OnDomClickEventValueAsync()
    {
        var reference = _feedNavOptionRef.GetRef();

        if (_selectedFeedItem is null) return Task.CompletedTask;
        
        _selectedFeedItem = null;
        reference.Hide();

        return Task.CompletedTask;
    }

    private Task OnMoreClickAsync(MouseEventArgs e, FeedNavigation item)
    {
        _selectedFeedItem = item;

        StateHasChanged();
        
        var reference = _feedNavOptionRef.GetRef();
        _jsRuntime.InvokeVoidAsync("updateContextPosition", new { className = "feed-context", x = e.PageX, y = e.PageY });
        return reference.Show();
    }

    private async Task OnFavoriteToggle(FeedNavigation item)
    {
        item.Favorite = !item.Favorite;

        // TODO: Add single action to update those bools instead of whole object
        await _httpClient.PutAsJsonAsync("/api/v1.0/FeedNavigation/Update", item);
    }

    private async Task OnDefaultToggle(FeedNavigation item)
    {
        Model?.ForEach(x => x.FeedNavigations?.ForEach(i => i.Default = false));
        item.Default = !item.Default;

        // TODO: Add single action to update those bools instead of whole object

        // At first reset the Default of all elements
        await _httpClient.PutAsJsonAsync("/api/v1.0/FeedNavigation/ResetDefault", Model?.Select(x => x.Id));
        await _httpClient.PutAsJsonAsync("/api/v1.0/FeedNavigation/Update", item);
    }

    private async Task OnEditContext(FeedNavigation item)
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
            var updateResponse = await _httpClient.PutAsJsonAsync($"/api/v1.0/FeedNavigation/Update", updatedFeed);

            if (updateResponse.IsSuccessStatusCode)
            {
                StateHasChanged();

                HandleUpdateResult(true);
                return;
            }

            HandleUpdateResult(false);
        }

        await _editFeedModalViewRef.ShowModal(item, SaveModalCallback);
    }

    private async Task OnDeleteRecord(FeedNavigation item)
    {
        void HandleDeleteResult(bool deleted)
        {
            if (deleted) Model?.RemoveAll(x => x.Id == item.Id);
            else
            {
                _mainLayout.ShowError("Sorry, we were unable to delete the requested item. Please try again later or contact support.");
            }
            
            StateHasChanged();
        }
        
        var deletedResponse = await _httpClient.DeleteAsync($"/api/v1.0/FeedNavigation/Delete?id={item.Id}");

        if (deletedResponse.IsSuccessStatusCode)
        {
            // FeedContents = default!;
            
            HandleDeleteResult(true);
            return;
        }
        
        HandleDeleteResult(false);
    }

    protected Task OnContentLoadingChange(bool value)
    {
        ContentLoading = value;
        return ContentLoadingChanged.InvokeAsync(value);
    }

    protected Task OnContentDataChange(List<FeedContent> feedContent)
    {
        FeedContent = feedContent;
        return FeedContentChanged.InvokeAsync(feedContent);
    }
}