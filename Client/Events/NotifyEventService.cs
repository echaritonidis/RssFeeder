namespace RssFeeder.Client.Events;

public class NotifyEventService
{
    public event EventHandler? InvalidateFeedNavigationGroupClick;
    public event EventHandler? InvalidateFeedNavigationClick;

    public void NotifyFeedNavigationGroupClick(object sender)
    {
        this.InvalidateFeedNavigationGroupClick?.Invoke(sender, EventArgs.Empty);
    }
    
    public void NotifyFeedNavigationClick(object sender)
    {
        this.InvalidateFeedNavigationClick?.Invoke(sender, EventArgs.Empty);
    }
}