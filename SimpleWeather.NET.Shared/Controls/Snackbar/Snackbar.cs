#if WINDOWS
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Threading.Tasks;
#endif

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls
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
        Informational,
        Success,
        Warning,
        Error,
    }

    /**
     * Wrapper for the Snackbar/InAppNotification/Infobar
     * Snackbar is managed by SnackbarManager
     */
    public sealed partial class Snackbar
#if WINDOWS
        : DependencyObject
#endif
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
#if WINDOWS
        public IconSource Icon { get; set; }
#endif
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

        public static Task<Snackbar> MakeAsync(
#if WINDOWS
            DispatcherQueue dispatcher,
#endif
            String message, SnackbarDuration duration, SnackbarInfoType infoType = SnackbarInfoType.Informational)
        {
#if WINDOWS
            return dispatcher.EnqueueAsync(() =>
#else
            return MainThread.InvokeOnMainThreadAsync(() =>
#endif
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