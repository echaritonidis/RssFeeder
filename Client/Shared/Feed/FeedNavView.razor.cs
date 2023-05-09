using System.Net.Http.Json;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RssFeeder.Shared.Extensions;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Shared.Feed;

public partial class FeedNavView : IDisposable
{
    [CascadingParameter] public MainLayout _mainLayout { get; set; }
    [Inject] public IJSRuntime _jsRuntime { get; set; } = default!;
    [Inject] public HttpClient _httpClient { get; set; } = default!;
    [Parameter] public List<FeedNavigation> Model { get; set; }
    [Parameter] public Func<bool, Task> OnContentLoadingCallback { get; set; }
    [Parameter] public Func<List<FeedContent>, Task> OnContentDataCallback { get; set; }
    [JSInvokable] public static async Task CloseDropDown() => await OnDomClickEventCallback?.Invoke();

    private static Func<Task> OnDomClickEventCallback;
    private FeedNavigation? SelectedFeedItem;
    private FeedNavOption FeedNavOptionRef = default!;
    private EditFeedModalView editFeedModalViewRef = default!;

    public void Dispose()
    {
        _jsRuntime.InvokeVoidAsync("unbindDomChanges");
    }

    protected override async Task OnInitializedAsync()
    {
        OnDomClickEventCallback = OnDomClickEventValueAsync;

        await _jsRuntime.InvokeVoidAsync("bindDomChanges");
        
        var defaultNavigationItem = Model.SingleOrDefault(x => x.Default);
        if (defaultNavigationItem != null) await OnSelectFeed(defaultNavigationItem);
    }

    private Task OnDomClickEventValueAsync()
    {
        var reference = FeedNavOptionRef.GetRef();

        if (SelectedFeedItem is not null)
        {
            SelectedFeedItem = null;
            reference.Hide();
        }

        return Task.CompletedTask;
    }
    
    private Task OnMoreClickAsync(MouseEventArgs e, FeedNavigation item)
    {
        SelectedFeedItem = item;

        var reference = FeedNavOptionRef.GetRef();
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
        Model.ForEach(i => i.Default = false);
        item.Default = !item.Default;

        // TODO: Add single action to update those bools instead of whole object


        // At first reset the Default of all elements
        await _httpClient.PutAsJsonAsync("/api/v1.0/FeedNavigation/ResetDefault", Model.Select(x => x.Id));
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

        await editFeedModalViewRef.ShowModal(item, SaveModalCallback);
    }

    private async Task OnDeleteRecord(FeedNavigation item)
    {
        void HandleDeleteResult(bool deleted)
        {
            if (deleted) Model.RemoveAll(x => x.Id == item.Id);
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

