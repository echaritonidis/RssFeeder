using System.Net.Http.Json;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RssFeeder.Shared.Extensions;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Shared.Feed;

public partial class FeedNavGroupView
{
    [Parameter] public List<FeedNavigationGroup> Model { get; set; }
    [Parameter] public bool ContentLoading { get; set; }
    [Parameter] public List<FeedContent> FeedContent { get; set; }
    [Parameter] public EventCallback<bool> ContentLoadingChanged { get; set; }
    [Parameter] public EventCallback<List<FeedContent>> FeedContentChanged { get; set; }
    
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