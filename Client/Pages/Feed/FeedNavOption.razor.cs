using Blazorise;
using Microsoft.AspNetCore.Components;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Pages.Feed;

public partial class FeedNavOption
{
    [Parameter] public FeedNavigation? FeedItem { get; set; }

    [Parameter] public EventCallback<FeedNavigation> FeedItemChanged { get; set; }
    [Parameter] public EventCallback<FeedNavigation> OnFavoriteChangeCallback { get; set; }
    [Parameter] public EventCallback<FeedNavigation> OnDefaultChangeCallback { get; set; }
    [Parameter] public EventCallback<FeedNavigation> OnEditCallback { get; set; }
    [Parameter] public EventCallback<FeedNavigation> OnDeleteCallback { get; set; }
    
    private Dropdown moreOptionsRef = default!;

    public Dropdown GetRef() => moreOptionsRef;
}