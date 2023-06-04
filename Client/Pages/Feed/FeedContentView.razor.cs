using Microsoft.AspNetCore.Components;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Pages.Feed;

public partial class FeedContentView
{
    [Parameter] public List<FeedContent> Model { get; set; }
}