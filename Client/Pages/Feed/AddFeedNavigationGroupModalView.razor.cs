using Blazorise;
using Microsoft.AspNetCore.Components;
using RssFeeder.Shared.Model;

namespace RssFeeder.Client.Pages.Feed;

public partial class AddFeedNavigationGroupModalView
{
    [Parameter] public Func<FeedNavigationGroup, Task>? OnSuccessResponse { get; set; }

    private FeedNavigationGroup _feedNavigationGroup = default!;
    private Modal modalRef = default!;
    private Validations validations = default!;

    public Task ShowModal()
    {
        _feedNavigationGroup = new();
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
            if (OnSuccessResponse is not null) await OnSuccessResponse(_feedNavigationGroup);

            await HideModal();
        }
    }
}