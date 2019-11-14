using System;
using System.Collections.Generic;
using System.Linq;
using SimpleWeather.Utils;
using Windows.ApplicationModel;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Core;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using System.Threading.Tasks;
using SimpleWeather.WeatherData;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Main;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationViewItemInvokedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs;
using NavigationViewPaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page, ICommandBarPage, IBackRequestedPage
    {
        public string CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        public SettingsPage()
        {
            this.InitializeComponent();

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("Nav_Settings/Label");
        }

        public async Task<bool> OnBackRequested()
        {
            if (SettingsFrame?.Content is IBackRequestedPage backRequestedPage)
            {
                return await backRequestedPage.OnBackRequested();
            }

            return false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var firstItem = SettingsNavView.MenuItems.First();

            if (SettingsNavView.SelectedItem == firstItem)
            {
                SettingsFrame_Navigated(SettingsFrame, e);
            }
            else
            {
                SettingsNavView.SelectedItem = firstItem;
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            Type pageType;
            switch ((args.SelectedItem as NavigationViewItem)?.Tag)
            {
                case "General":
                default:
                    pageType = typeof(Settings_General);
                    break;
                case "Credits":
                    pageType = typeof(Settings_Credits);
                    break;
                case "OSSLibs":
                    pageType = typeof(Settings_OSSLibs);
                    break;
                case "About":
                    pageType = typeof(Settings_About);
                    break;
            }
            SettingsFrame.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
        }

        private void SettingsFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (SettingsFrame?.Content is Page page && VisualTreeHelperExtensions.FindChild<ScrollViewer>(page.Content as FrameworkElement, null, true) is ScrollViewer scrollViewer)
            {
                // NOTE: ChangeView does not work here for some reason
                scrollViewer.ScrollToVerticalOffset(0);
            }
            if (SettingsFrame?.Content is IFrameContentPage contentPage)
            {
                contentPage.OnNavigatedToPage(e);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (SettingsFrame?.Content is IFrameContentPage contentPage)
            {
                contentPage.OnNavigatedFromPage(e);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (SettingsFrame?.Content is IFrameContentPage contentPage)
            {
                contentPage.OnNavigatingFromPage(e);
            }
        }
    }
}
