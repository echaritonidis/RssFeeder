using Microsoft.AspNetCore.Components;

namespace RssFeeder.Client.Shared;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
    protected bool DisplayError { get; set; }
    protected string? ErrorMessage { get; set; }

    private Timer _timer = default!;

    public void Dispose()
    {
        _timer?.Dispose();
    }
    
    private void InitializeTimer()
    {
        _timer?.Dispose();
        _timer = new Timer
        (
            callback: async state =>
            {
                ErrorMessage = null;
                DisplayError = false;

                await InvokeAsync(StateHasChanged);
            },
            state: null,
            dueTime: TimeSpan.FromSeconds(10),
            period: Timeout.InfiniteTimeSpan
        );
    }
    
    public void ShowError(string message)
    {
        DisplayError = true;
        ErrorMessage = message;

        StateHasChanged();
        
        InitializeTimer();
    }
}

