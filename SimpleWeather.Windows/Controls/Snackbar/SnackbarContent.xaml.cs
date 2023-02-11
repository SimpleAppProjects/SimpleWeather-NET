using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls
{
    public sealed partial class SnackbarContent : UserControl
    {
        private DispatcherTimer timer;

        public Snackbar SnackbarModel
        {
            get { return this.DataContext as Snackbar; }
        }

        public SnackbarContent()
        {
            this.InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, object e)
        {
            Dismiss(SnackbarDismissEvent.Timeout);
        }

        public void Dismiss()
        {
            Dismiss(SnackbarDismissEvent.Programmatic);
        }

        private void Dismiss(SnackbarDismissEvent dismissEvent)
        {
            this.Infobar.IsOpen = false;
            Dismissed(this, new SnackbarDismissedEventArgs()
            {
                DismissKind = dismissEvent
            });
        }

        public bool IsShowing => this.Infobar.IsOpen;

        public void Show()
        {
            Show(SnackbarDuration.Forever);
        }

        public void Show(SnackbarDuration duration)
        {
            timer.Stop();

            this.Infobar.IsOpen = true;
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

        private void Infobar_CloseButtonClick(InfoBar sender, object args)
        {
            Dismissed(this, new SnackbarDismissedEventArgs()
            {
                DismissKind = SnackbarDismissEvent.User
            });
        }

        internal static InfoBarSeverity ToSeverity(SnackbarInfoType type)
        {
            return (InfoBarSeverity)type;
        }
    }
}
