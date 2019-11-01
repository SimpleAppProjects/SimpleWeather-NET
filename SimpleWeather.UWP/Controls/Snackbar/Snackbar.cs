using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public enum SnackbarDuration
    {
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
        Programmatic = InAppNotificationDismissKind.Programmatic,
        //
        // Summary:
        //     When user explicitly dismissed the notification.
        User = InAppNotificationDismissKind.User,
        //
        // Summary:
        //     When the system dismissed the notification after timeout.
        Timeout = InAppNotificationDismissKind.Timeout,
        //
        // Summary:
        //     When the user dismissed the notification by clicking the action button.
        Action = 3
    }

    /**
     * Wrapper for the Snackbar/InAppNotification
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

        public static Snackbar Make(String message, SnackbarDuration duration)
        {
            Snackbar snackbar = new Snackbar()
            {
                Message = message,
                Duration = duration
            };

            return snackbar;
        }

        public void SetAction(String buttonTxt, Action action)
        {
            ButtonLabel = buttonTxt?.ToUpper();
            ButtonAction = action;
        }
    }
}
