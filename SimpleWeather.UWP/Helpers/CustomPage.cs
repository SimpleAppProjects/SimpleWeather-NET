using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

namespace SimpleWeather.UWP.Helpers
{
    public abstract class CustomPage : Page, ICommandBarPage, ISnackbarManager
    {
        public string CommandBarLabel { get; set; }
        public List<muxc.NavigationViewItemBase> PrimaryCommands { get; set; }

        protected SnackbarManager SnackMgr { get; private set; }

        public void InitSnackManager()
        {
            if (SnackMgr == null)
            {
                SnackMgr = new SnackbarManager(Content as Panel);
            }
        }

        public void ShowSnackbar(Snackbar snackbar)
        {
            Dispatcher.RunOnUIThread(() =>
            {
                SnackMgr?.Show(snackbar);
            });
        }

        public void DismissAllSnackbars()
        {
            Dispatcher.RunOnUIThread(() =>
            {
                SnackMgr?.DismissAll();
            });
        }

        public void UnloadSnackManager()
        {
            DismissAllSnackbars();
            SnackMgr = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            InitSnackManager();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            UnloadSnackManager();
        }
    }
}