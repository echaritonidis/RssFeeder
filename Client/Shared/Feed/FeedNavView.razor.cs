﻿using System;
using Microsoft.AspNetCore.Components;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Shared.Feed;

public partial class FeedNavView
{
    [Parameter] public List<FeedNavigation> Model { get; set; }

    [Parameter] public EventCallback<FeedNavigation> OnSelectFeedCallback { get; set; }

    [Parameter] public EventCallback<FeedNavigation> OnFavoriteChangeCallback { get; set; }

    [Parameter] public EventCallback<FeedNavigation> OnDefaultChangeCallback { get; set; }

    private async Task OnFavoriteToggle(FeedNavigation item)
    {
        item.Favorite = !item.Favorite;

        await this.OnFavoriteChangeCallback.InvokeAsync(item);
    }

    private async Task OnDefaultToggle(FeedNavigation item)
    {
        item.Default = !item.Default;

        await this.OnDefaultChangeCallback.InvokeAsync(item);
    }
}

