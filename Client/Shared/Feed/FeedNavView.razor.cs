using Blazorise;
using Microsoft.AspNetCore.Components;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Shared.Feed;

public partial class FeedNavView
{
    [Parameter] public List<FeedNavigation> Model { get; set; }

    [Parameter] public EventCallback<FeedNavigation> OnSelectFeedCallback { get; set; }

    [Parameter] public EventCallback<FeedNavigation> OnFavoriteChangeCallback { get; set; }

    [Parameter] public EventCallback<FeedNavigation> OnDefaultChangeCallback { get; set; }

    protected FeedNavigation? SelectedFeedItem { get; set; }

    private FeedNavOption FeedNavOptionRef;

    private Task OnMoreClickAsync(FeedNavigation item)
    {
        SelectedFeedItem = item;

        StateHasChanged();

        var reference = FeedNavOptionRef.GetRef();

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

    private async Task OnFeedNavClickAsync(FeedNavigation item)
    {
        Model.ForEach(i => i.Active = false);
        item.Active = true;

        await OnSelectFeedCallback.InvokeAsync(item);
    }
}

