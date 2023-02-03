using Blazorise;
using Microsoft.AspNetCore.Components;
using RssFeeder.Shared.Extensions;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Shared.Feed;

public partial class AddFeedModalView
{
    [Parameter] public Func<FeedNavigation, Task> OnSuccessResponse { get; set; }

    private FeedNavigation feedNavigation = new();
    private Modal modalRef;
    private Validations validations;
    private string tagName;

    public Task ShowModal()
    {
        return modalRef.Show();
    }

    public Task HideModal()
    {
        return modalRef.Hide();
    }

    private async Task OnSaveFeed()
    {
        if (await validations.ValidateAll())
        {
            if (OnSuccessResponse is not null) await OnSuccessResponse(feedNavigation);

            await HideModal();
        }
    }

    private void OnAddNewTag()
    {
        if (feedNavigation.Tags == null) feedNavigation.Tags = new();

        feedNavigation.Tags.Add(new FeedTag
        {
            Name = tagName
        });

        tagName = null;
    }


    protected void OnBadgeClose(FeedTag feedTag)
    {
        feedNavigation.Tags.Remove(feedTag);
    }

    protected void ValidateHyperlink(ValidatorEventArgs e)
    {
        var link = Convert.ToString(e.Value);

        e.Status = link.ValidateUrl()
                ? ValidationStatus.Success
                : ValidationStatus.Error;
    }
}