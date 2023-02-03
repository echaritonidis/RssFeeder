namespace RssFeeder.Client.Events;

public class NotifyEventService
{
    public event EventHandler? EventClick;

    public void NotifyEventClick(object sender)
    {
        if (this.EventClick != null) this.EventClick(sender, EventArgs.Empty);
    }
}