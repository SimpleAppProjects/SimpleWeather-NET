using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SimpleWeather.UWP.Controls
{
    public sealed class SnackbarManager
    {
        private Stack<Snackbar> Snacks;
        private Panel ParentView;
        private SnackbarContent SnackbarView;
        private DispatcherTimer timer;

        public SnackbarManager(Panel Parent)
        {
            if (Parent == null)
                throw new ArgumentException("Parent is not a Panel control", nameof(Parent));

            ParentView = Parent;
            Snacks = new Stack<Snackbar>();

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, object e)
        {
            while (Snacks.Count > 0)
            {
                RemoveSnack(SnackbarDismissEvent.Timeout);
            }

            UpdateView();
        }

        public void Show(Snackbar snackbar)
        {
            // Add current snackbar to stack
            Snacks.Push(snackbar);

            // Update SnackBar view
            UpdateView();
        }

        public void DismissAll()
        {
            if (Snacks.Count == 0)
            {
                UpdateView();
                return;
            }

            while (Snacks.Count > 0)
            {
                // Pop out SnackPair
                Snackbar snackbar = Snacks.Pop();
                // Perform dismiss action
                snackbar?.Dismissed?.Invoke(snackbar, SnackbarDismissEvent.Programmatic);
            }

            Snacks.Clear();
            UpdateView();
        }

        /**
         * Removes the current snackbar after it times out
         */

        private void RemoveSnack(SnackbarDismissEvent @event)
        {
            if (Snacks.Count == 0) return;
            // Pop out Snackbar
            Snackbar snackbar = Snacks.Pop();
            // Perform dismiss action
            snackbar?.Dismissed?.Invoke(snackbar, @event);
            // Return and let mgr updateview
        }

        /**
         * Update the Snackbar view
         */

        private void UpdateView()
        {
            // Get current Snackbar
            Snackbar snackbar = GetCurrentSnackbar();
            if (snackbar == null)
            {
                // Remove callbacks
                timer.Stop();
                // Dismiss view if there are no omore snackbars
                if (SnackbarView != null)
                {
                    SnackbarView.Dismiss();
                    ParentView?.Children.Remove(SnackbarView);
                    SnackbarView = null;
                }
            }
            else
            {
                // Check if InAppNotification view exists
                if (SnackbarView == null)
                {
                    var snackbarView = new SnackbarContent();
                    snackbarView.Shown += Snackbar_Shown;
                    snackbarView.Dismissed += Snackbar_Dismissed;
                    ParentView?.Children.Add(snackbarView);
                    SnackbarView = snackbarView;
                }

                // Update button command
                SnackbarView.DataContext = snackbar;

                if (SnackbarView.FindName("ActionButton") is Button button)
                {
                    button.Command = new RelayCommand(() =>
                    {
                        snackbar?.ButtonAction?.Invoke();
                        // Now dismiss the Snackbar
                        snackbar?.Dismissed?.Invoke(snackbar, SnackbarDismissEvent.Action);
                        if (Snacks.Count > 0) Snacks.Pop();
                        UpdateView();
                    });
                }
                if (!SnackbarView.IsShowing)
                {
                    SnackbarView.Show(SnackbarDuration.Forever);
                }

                timer.Stop();
                int durationMs = (int)snackbar.Duration;
                timer.Interval = TimeSpan.FromMilliseconds(durationMs);
                timer.Start();
            }
        }

        private void Snackbar_Shown(object sender, EventArgs e)
        {
            var snackbar = GetCurrentSnackbar();
            snackbar?.Shown?.Invoke(snackbar);
        }

        private void Snackbar_Dismissed(object sender, SnackbarDismissedEventArgs e)
        {
            if (e.DismissKind != SnackbarDismissEvent.Timeout)
            {
                RemoveSnack(e.DismissKind);
            }
        }

        private Snackbar GetCurrentSnackbar()
        {
            if (Snacks.Count == 0) return null;
            return Snacks.Peek();
        }
    }
}