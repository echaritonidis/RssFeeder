using Blazorise;
using Microsoft.AspNetCore.Components;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Shared.Feed;

public partial class FeedNavOption
{
    private FeedNavigation? _feedItemValue;

    [Parameter]
    public FeedNavigation? FeedItem
    {
        get => _feedItemValue;
        set
        {
            if (_feedItemValue == value) return;
            _feedItemValue = value;
        }
    }

    [Parameter] public EventCallback<FeedNavigation> FeedItemChanged { get; set; }
    [Parameter] public EventCallback<FeedNavigation> OnFavoriteChangeCallback { get; set; }
    [Parameter] public EventCallback<FeedNavigation> OnDefaultChangeCallback { get; set; }
    [Parameter] public EventCallback<FeedNavigation> OnEditCallback { get; set; }
    [Parameter] public EventCallback<FeedNavigation> OnDeleteCallback { get; set; }
    
    private Dropdown moreOptionsRef;

    public Dropdown GetRef() => moreOptionsRef;
}