using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RssFeeder.Shared.Extensions;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Pages.Feed;

public partial class AddFeedNavigationModalView
{
    [Parameter] public List<FeedNavigationGroupNames>? GroupNames { get; set; }
    [Parameter] public Func<FeedNavigation, Task>? OnSuccessResponse { get; set; }

    private FeedNavigation feedNavigation;
    private Modal modalRef;
    private Validations validations;
    private string labelName;
    private FeedNavigationGroupNames? selectedFeedNavigationGroup;

    public Task ShowModal()
    {
        feedNavigation = new();
        selectedFeedNavigationGroup = GroupNames?.FirstOrDefault(g => g.Initial);
        labelName = string.Empty;
        validations.ClearAll();

        return modalRef.Show();
    }

    public Task HideModal()
    {
        return modalRef.Hide();
    }

    private void OnGroupClicked(FeedNavigationGroupNames fng)
    {
        selectedFeedNavigationGroup = fng;
        feedNavigation.GroupId = fng.Id;
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