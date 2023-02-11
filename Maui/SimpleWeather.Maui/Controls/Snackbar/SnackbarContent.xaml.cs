using SimpleWeather.NET.Controls;

namespace SimpleWeather.Maui.Controls;

public partial class SnackbarContent : ContentView
{
    private readonly IDispatcherTimer timer;

    public Snackbar SnackbarModel
    {
        get { return this.BindingContext as Snackbar; }
    }

    public SnackbarContent()
    {
        InitializeComponent();

        this.IsVisible = false;

        timer = Dispatcher.CreateTimer();
        timer.Tick += Timer_Tick;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        Dismiss(SnackbarDismissEvent.Timeout);
    }

    public void Dismiss()
    {
        Dismiss(SnackbarDismissEvent.Programmatic);
    }

    private void Dismiss(SnackbarDismissEvent dismissEvent)
    {
        this.IsVisible = false;
        Dismissed(this, new SnackbarDismissedEventArgs()
        {
            DismissKind = dismissEvent
        });
    }

    public bool IsShowing => this.IsVisible;

    public void Show()
    {
        Show(SnackbarDuration.Forever);
    }

    public void Show(SnackbarDuration duration)
    {
        timer.Stop();

        this.IsVisible = true;
        Shown(this, EventArgs.Empty);

        int durationMs = (int)duration;
        if (durationMs > 0)
        {
            timer.Interval = TimeSpan.FromMilliseconds(durationMs);
            timer.Start();
        }
    }

    public event EventHandler Shown;
    public event EventHandler<SnackbarDismissedEventArgs> Dismissed;

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Dismiss(SnackbarDismissEvent.User);
    }
}