using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RssFeeder.Shared.Extensions;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Shared.Feed;

public partial class AddFeedModalView
{
    [Parameter] public Func<FeedNavigation, Task> OnSuccessResponse { get; set; }

    private FeedNavigation feedNavigation;
    private Modal modalRef;
    private Validations validations;
    private string labelName;

    public Task ShowModal()
    {
        feedNavigation = new();
        labelName = string.Empty;
        validations.ClearAll();

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

    private void OnAddNewLabel()
    {
        if (feedNavigation.FeedLabels == null) feedNavigation.FeedLabels = new();

        feedNavigation.FeedLabels.Add(new FeedLabel
        {
            Name = labelName
        });

        labelName = string.Empty;
    }

    private void OnBadgeClose(FeedLabel label)
    {
        feedNavigation.FeedLabels?.Remove(label);
    }

    private void OnKeyPressInLabel(KeyboardEventArgs eventArgs)
    {
        if (eventArgs.Key == "Enter")
        {
            OnAddNewLabel();
        }
    }

    private void ValidateHyperlink(ValidatorEventArgs e)
    {
        var link = Convert.ToString(e.Value);

        e.Status = link.ValidateUrl()
                ? ValidationStatus.Success
                : ValidationStatus.Error;
    }
}