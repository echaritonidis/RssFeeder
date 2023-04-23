using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Shared.Feed;

public partial class FeedNavView : IDisposable
{
    [Inject] IJSRuntime _jsRuntime { get; set; }
    [Parameter] public List<FeedNavigation> Model { get; set; }
    [Parameter] public EventCallback<FeedNavigation> OnSelectFeedCallback { get; set; }
    [Parameter] public EventCallback<FeedNavigation> OnFavoriteChangeCallback { get; set; }
    [Parameter] public EventCallback<FeedNavigation> OnDefaultChangeCallback { get; set; }
    [Parameter] public Func<FeedNavigation, Task> OnEditCallback { get; set; }
    [Parameter] public Func<Guid, Task> OnDeleteCallback { get; set; }
    [JSInvokable] public static async Task CloseDropDown() => await OnDomClickEventCallback?.Invoke();

    private FeedNavigation? SelectedFeedItem;
    private FeedNavOption FeedNavOptionRef;
    private static Func<Task> OnDomClickEventCallback;

    public void Dispose()
    {
        _jsRuntime.InvokeVoidAsync("unbindDomChanges");
    }

    protected override async Task OnInitializedAsync()
    {
        OnDomClickEventCallback = OnDomClickEventValueAsync;

        await _jsRuntime.InvokeVoidAsync("bindDomChanges");
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

        await this.OnFavoriteChangeCallback.InvokeAsync(item);
    }

    private async Task OnDefaultToggle(FeedNavigation item)
    {
        Model.ForEach(i => i.Default = false);
        item.Default = !item.Default;

        await this.OnDefaultChangeCallback.InvokeAsync(item);
    }

    private async Task OnEditContext(FeedNavigation item) => await OnEditCallback(item);
    
    private async Task OnDeleteRecord(FeedNavigation item) => await OnDeleteCallback(item.Id);

    private async Task OnFeedNavClickAsync(FeedNavigation item)
    {
        // If it's already selected skip
        if (Model.Single(x => x.Id == item.Id).Active) return;

        Model.ForEach(i => i.Active = false);
        item.Active = true;

        await OnSelectFeedCallback.InvokeAsync(item);
    }
}

