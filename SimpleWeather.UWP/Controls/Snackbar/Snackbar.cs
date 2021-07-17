using SimpleWeather.Utils;
using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using muxc = Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public enum SnackbarDuration
    {
        Forever = 0,
        VeryShort = 2000,
        Short = 3500,
        Long = 5000
    }

    //
    // Summary:
    //     Enumeration to describe how a Snackbar was dismissed
    public enum SnackbarDismissEvent
    {
        //
        // Summary:
        //     When the system dismissed the notification.
        Programmatic = 0,

        //
        // Summary:
        //     When user explicitly dismissed the notification.
        User = 1,

        //
        // Summary:
        //     When the system dismissed the notification after timeout.
        Timeout = 2,

        //
        // Summary:
        //     When the user dismissed the notification by clicking the action button.
        Action = 3
    }

    public enum SnackbarInfoType
    {
        Informational = muxc.InfoBarSeverity.Informational,
        Success = muxc.InfoBarSeverity.Success,
        Warning = muxc.InfoBarSeverity.Warning,
        Error = muxc.InfoBarSeverity.Error
    }

    /**
     * Wrapper for the Snackbar/InAppNotification/Infobar
     * Snackbar is managed by SnackbarManager
     */

    public sealed class Snackbar : DependencyObject
    {
        private Snackbar()
        {
        }

        public string Message { get; set; }
        public SnackbarDuration Duration { get; set; }
        public string ButtonLabel { get; set; }
        public Action ButtonAction { get; set; }
        public Action<Snackbar> Shown { get; set; }
        public Action<Snackbar, SnackbarDismissEvent> Dismissed { get; set; }

        public string Title { get; set; }
        public muxc.IconSource Icon { get; set; }
        public SnackbarInfoType InfoType { get; set; } = SnackbarInfoType.Informational;

        public static Snackbar Make(String message, SnackbarDuration duration, SnackbarInfoType infoType = SnackbarInfoType.Informational)
        {
            Snackbar snackbar = new Snackbar()
            {
                Message = message,
                Duration = duration,
                InfoType = infoType
            };

            return snackbar;
        }

        public static Snackbar MakeSuccess(String message, SnackbarDuration duration)
        {
            return Snackbar.Make(message, duration, SnackbarInfoType.Success);
        }

        public static Snackbar MakeWarning(String message, SnackbarDuration duration)
        {
            return Snackbar.Make(message, duration, SnackbarInfoType.Warning);
        }

        public static Snackbar MakeError(String message, SnackbarDuration duration)
        {
            return Snackbar.Make(message, duration, SnackbarInfoType.Error);
        }

        public static Task<Snackbar> MakeAsync(CoreDispatcher dispatcher, String message, SnackbarDuration duration, SnackbarInfoType infoType = SnackbarInfoType.Informational)
        {
            return DispatcherExtensions.RunOnUIThread(dispatcher, () =>
            {
                return Snackbar.Make(message, duration, infoType);
            });
        }

        public void SetAction(String buttonTxt, Action action)
        {
            ButtonLabel = buttonTxt;
            ButtonAction = action;
        }
    }
}