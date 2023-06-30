using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RssFeeder.Shared.Extensions;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Pages.Feed;

public partial class EditFeedModalView
{
    private Func<FeedNavigation, Task> OnSuccessResponse;
    
    private FeedNavigation feedNavigationToEdit;
    private Modal modalRef;
    private Validations validations;
    private string labelName;

    public Task ShowModal(FeedNavigation feedNavigation, Func<FeedNavigation, Task> successCallback)
    {
        feedNavigationToEdit = feedNavigation;
        OnSuccessResponse = successCallback;
        
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
            await HideModal();

            await OnSuccessResponse(feedNavigationToEdit);
        }
    }

    private void OnAddNewLabel()
    {
        if (feedNavigationToEdit.FeedLabels == null) feedNavigationToEdit.FeedLabels = new();

        feedNavigationToEdit.FeedLabels.Add(new FeedLabel
        {
            Name = labelName
        });

        labelName = string.Empty;
    }

    private void OnBadgeClose(FeedLabel label)
    {
        feedNavigationToEdit.FeedLabels?.Remove(label);
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