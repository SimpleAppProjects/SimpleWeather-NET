using SimpleWeather.Utils;
using SimpleWeather.UWP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page, ICommandBarPage, IBackRequestedPage
    {
        public string CommandBarLabel { get; set; }
        public List<muxc.NavigationViewItemBase> PrimaryCommands { get; set; }

        public SettingsPage()
        {
            this.InitializeComponent();

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("Nav_Settings/Content");
            AnalyticsLogger.LogEvent("SettingsPage");
        }

        public Task<bool> OnBackRequested()
        {
            if (SettingsFrame?.Content is IBackRequestedPage backRequestedPage)
            {
                return backRequestedPage.OnBackRequested();
            }

            return Task.FromResult(false);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnalyticsLogger.LogEvent("SettingsPage: OnNavigatedTo");

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

        private void NavigationView_SelectionChanged(muxc.NavigationView sender, muxc.NavigationViewSelectionChangedEventArgs args)
        {
            Type pageType;
            switch ((args.SelectedItem as muxc.NavigationViewItem)?.Tag)
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

            AnalyticsLogger.LogEvent("SettingsPage: NavigationView_SelectionChanged",
                new Dictionary<string, string>()
                {
                    { "PageType", pageType?.Name }
                });

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