using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace SimpleWeather.UWP.Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupPage : Page
    {
        public Frame AppFrame { get { return FrameContent; } }
        public static SetupPage Instance { get; set; }
        public LocationData Location { get; set; }

        private UISettings UISettings;

        private List<Type> Pages = new List<Type>()
        {
            typeof(SetupWelcomePage),
            typeof(SetupLocationsPage),
            typeof(SetupSettingsPage)
        };

        public SetupPage()
        {
            this.InitializeComponent();

            Instance = this;

            UISettings = new UISettings();
            UpdateAppTheme();

            Window.Current.SetTitleBar(AppTitleBar);
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += (s, e) => UpdateTitleBarLayout(s);
            CoreApplication.GetCurrentView().TitleBar.IsVisibleChanged += (s, e) => UpdateTitleBarVisibility(s);

            // remove the solid-colored backgrounds behind the caption controls and system back button if we are in left mode
            // This is done when the app is loaded since before that the actual theme that is used is not "determined" yet
            Loaded += delegate (object sender, RoutedEventArgs e)
            {
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                UpdateAppTheme(); // TitleBar color
            };

            AppFrame.Navigated += AppFrame_Navigated;
            BottomNavBar.BackButtonClicked += BackBtn_Click;
            BottomNavBar.NextButtonClicked += NextBtn_Click;

            // Setup Pages & Indicator
            if (Settings.WeatherLoaded)
            {
                Pages.Remove(typeof(SetupLocationsPage));
            }

            BottomNavBar.ItemCount = Pages.Count;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UISettings.ColorValuesChanged += UISettings_ColorValuesChanged;
            if (AppFrame.Content == null)
            {
                AppFrame.Navigate(Pages[0]);
            }
            AnalyticsLogger.LogEvent("SetupPage");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            UISettings.ColorValuesChanged -= UISettings_ColorValuesChanged;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AppFrame.SourcePageType != Pages[0] && AppFrame.CanGoBack)
            {
                AppFrame.GoBack();
            }
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        public void Next()
        {
            if (AppFrame.Content is not IPageVerification page || page.CanContinue())
            {
                var destination = AppFrame.CurrentSourcePageType;
                var destinationIdx = Pages.IndexOf(destination);
                if (destinationIdx >= Pages.Count - 1)
                {
                    // Complete
                    OnCompleted();
                }
                else if (AppFrame.CanGoForward)
                {
                    AppFrame.GoForward();
                }
                else
                {
                    var nextPage = Pages[destinationIdx + 1];

                    if (nextPage != destination)
                    {
                        var transition = new SlideNavigationTransitionInfo();

                        if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo", "Effect"))
                        {
                            transition.Effect = SlideNavigationTransitionEffect.FromRight;
                        }

                        AppFrame.Navigate(Pages[destinationIdx + 1], null, transition);
                    }
                }
            }
        }

        private void AppFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Change indicators
            BottomNavBar.SelectedIndex = Pages.IndexOf(e.SourcePageType);

            if (e.SourcePageType == Pages.Last())
            {
                BottomNavBar.ShowBackButton(false);
            }
            else if (e.SourcePageType == typeof(SetupLocationsPage))
            {
                BottomNavBar.ShowNextButton(false);
            }
        }

        private void OnCompleted()
        {
            // Retrieve setiings
            if (CoreApplication.Properties.TryGetValue(Settings.KEY_USEALERTS, out object alertsValue))
            {
                Settings.ShowAlerts = (bool)alertsValue;
                CoreApplication.Properties.Remove(Settings.KEY_USEALERTS);
            }
            if (CoreApplication.Properties.TryGetValue(Settings.KEY_REFRESHINTERVAL, out object refreshValue))
            {
                Settings.RefreshInterval = (int)refreshValue;
                CoreApplication.Properties.Remove(Settings.KEY_REFRESHINTERVAL);
            }
            if (CoreApplication.Properties.TryGetValue(Settings.KEY_TEMPUNIT, out object tempValue))
            {
                Settings.SetDefaultUnits((string)tempValue);
                CoreApplication.Properties.Remove(Settings.KEY_TEMPUNIT);
            }

            Settings.OnBoardComplete = true;
            this.Frame.Navigate(typeof(Shell), Location);
        }

        private void UpdateAppTheme()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.InactiveForegroundColor = Colors.WhiteSmoke;
            titleBar.ButtonInactiveForegroundColor = Colors.WhiteSmoke;
        }

        private async void UISettings_ColorValuesChanged(UISettings sender, object args)
        {
            // NOTE: Run on UI Thread since this may be called off the main thread
            await Dispatcher?.RunOnUIThread(() =>
            {
                UpdateAppTheme();
            });
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void UpdateTitleBarVisibility(CoreApplicationViewTitleBar coreTitleBar)
        {
            AppTitleBar.Visibility = coreTitleBar.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public string GetAppTitleFromSystem()
        {
            return App.ResLoader.GetString("AppName/Text");
        }
    }
}