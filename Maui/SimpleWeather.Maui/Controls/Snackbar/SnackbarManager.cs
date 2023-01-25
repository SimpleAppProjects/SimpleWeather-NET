using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using toolkitAlerts = CommunityToolkit.Maui.Alerts;

namespace SimpleWeather.Maui.Controls
{
    internal class SnackbarManager
    {
#if !WINDOWS
        private ISnackbar snackbarView;
#else
        private SnackbarContent snackbarView;
#endif
        private readonly Stack<Snackbar> snacks;
        private readonly IDispatcherTimer timer;
        private readonly
#if WINDOWS
            Layout
#else
            IView
#endif
            parentView;
        private IView anchorView;

        public IView AnchorView
        {
            get { return anchorView; }
            set { anchorView = value; }
        }

#if WINDOWS
        public SnackbarManager(Layout parent)
#else
        public SnackbarManager(IView parent)
#endif
        {
            if (parent == null)
                throw new ArgumentException("Parent is null", nameof(parent));

            parentView = parent;
            snacks = new Stack<Snackbar>();

            timer = Dispatcher.GetForCurrentThread().CreateTimer();
            timer.Tick += Timer_Tick;

#if !WINDOWS
            toolkitAlerts.Snackbar.Shown += Snackbar_Shown;
            toolkitAlerts.Snackbar.Dismissed += Snackbar_Dismissed;
#endif
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            while (snacks.Count > 0)
            {
                RemoveSnack(SnackbarDismissEvent.Timeout);
            }

            UpdateView();
        }

        public void Show(Snackbar snackbar)
        {
            // Add current snackbar to stack
            snacks.Push(snackbar);

            // Update SnackBar view
            UpdateView();
        }

        public void DismissAll()
        {
            if (snacks.Count == 0)
            {
                UpdateView();
                return;
            }

            while (snacks.Count > 0)
            {
                // Pop out SnackPair
                Snackbar snackbar = snacks.Pop();
                // Perform dismiss action
                snackbar?.Dismissed?.Invoke(snackbar, SnackbarDismissEvent.Programmatic);
            }

            snacks.Clear();
            UpdateView();
        }

        /**
         * Removes the current snackbar after it times out
         */

        private void RemoveSnack(SnackbarDismissEvent @event)
        {
            if (snacks.Count == 0) return;
            // Pop out Snackbar
            Snackbar snackbar = snacks.Pop();
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
                if (snackbarView != null)
                {
                    snackbarView.Dismiss();
#if WINDOWS
                    parentView?.Children.Remove(snackbarView);
#endif
                    snackbarView = null;
                }
            }
            else
            {
#if !WINDOWS
                var snackView = toolkitAlerts.Snackbar.Make(snackbar.Message, action: snackbar?.ButtonAction != null ? () =>
                {
                    snackbar?.ButtonAction?.Invoke();
                    // Now dismiss the Snackbar
                    snackbar?.Dismissed?.Invoke(snackbar, SnackbarDismissEvent.Action);
                    if (snacks.Count > 0) snacks.Pop();
                    UpdateView();
                }
                : null, actionButtonText: snackbar.ButtonLabel ?? string.Empty, TimeSpan.FromMinutes(1), anchor: anchorView,
                visualOptions: new SnackbarOptions()
                {
                    //ActionButtonTextColor = ,
                    //TextColor = ,
                    //BackgroundColor = ,
                });

                snackbarView?.Dismiss();
                snackbarView = snackView;
                snackbarView?.Show();
#else
                // Check if InAppNotification view exists
                if (snackbarView == null)
                {
                    var snackView = new SnackbarContent();
                    snackView.Shown += Snackbar_Shown;
                    snackView.Dismissed += Snackbar_Dismissed;
                    parentView?.Children.Add(snackView);
                    snackbarView = snackView;
                }

                // Update button command
                snackbarView.BindingContext = snackbar;

                if (snackbarView.FindByName("ActionButton") is Button button)
                {
                    button.Command = new RelayCommand(() =>
                    {
                        snackbar?.ButtonAction?.Invoke();
                        // Now dismiss the Snackbar
                        snackbar?.Dismissed?.Invoke(snackbar, SnackbarDismissEvent.Action);
                        if (snacks.Count > 0) snacks.Pop();
                        UpdateView();
                    });
                }
                snackbarView.Show();
#endif

                timer.Stop();
                int durationMs = (int)snackbar.Duration;
                timer.Interval = TimeSpan.FromMilliseconds(durationMs);
                timer.Start();
            }
        }

        private void Snackbar_Shown(object sender, EventArgs e)
        {
#if !WINDOWS
            if (snackbarView is not null && sender == snackbarView)
#endif
            {
                var snackbar = GetCurrentSnackbar();
                snackbar?.Shown?.Invoke(snackbar);
            }
        }

        private void Snackbar_Dismissed(object sender, EventArgs e)
        {
            if (snackbarView is not null && sender == snackbarView)
            {
                RemoveSnack(SnackbarDismissEvent.User);
            }
        }

        private void Snackbar_Dismissed(object sender, SnackbarDismissedEventArgs e)
        {
#if !WINDOWS
            if (snackbarView is not null && sender == snackbarView)
#endif
            {
                if (e.DismissKind != SnackbarDismissEvent.Timeout)
                {
                    RemoveSnack(e.DismissKind);
                }
            }
        }

        private Snackbar GetCurrentSnackbar()
        {
            if (snacks.Count == 0) return null;
            return snacks.Peek();
        }
    }
}
