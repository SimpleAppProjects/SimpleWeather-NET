using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.ComponentModel;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Main;
using SimpleWeather.UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
#if WINDOWS_UWP
using Windows.UI;
#endif
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
    public sealed partial class SetupPage : ViewModelPage, ISetupNavigator, IViewModelProvider
    {
        public Frame AppFrame { get { return FrameContent; } }
        public static ISetupNavigator Instance { get; private set; }
        private SetupViewModel ViewModel { get; }

        private readonly UISettings UISettings;
        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        private readonly List<Type> Pages = new List<Type>()
        {
            typeof(SetupWelcomePage),
            typeof(SetupLocationsPage),
            typeof(SetupSettingsPage)
        };

        public SetupPage()
        {
            this.InitializeComponent();

            Instance = this;
            ViewModel = GetViewModel<SetupViewModel>();

            UISettings = new UISettings();
            UpdateAppTheme();

#if WINDOWS_UWP
            Window.Current.SetTitleBar(AppTitleBar);
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += (s, e) => UpdateTitleBarLayout(s);
            CoreApplication.GetCurrentView().TitleBar.IsVisibleChanged += (s, e) => UpdateTitleBarVisibility(s);
#endif

            // remove the solid-colored backgrounds behind the caption controls and system back button if we are in left mode
            // This is done when the app is loaded since before that the actual theme that is used is not "determined" yet
            Loaded += delegate (object sender, RoutedEventArgs e)
            {
#if WINDOWS_UWP
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
#endif
                UpdateAppTheme(); // TitleBar color
            };

            AppFrame.Navigated += AppFrame_Navigated;
            BottomNavBar.BackButtonClicked += BackBtn_Click;
            BottomNavBar.NextButtonClicked += NextBtn_Click;

            // Setup Pages & Indicator
            if (SettingsManager.WeatherLoaded)
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
            Back();
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        public void Back()
        {
            if (AppFrame.SourcePageType != Pages[0] && AppFrame.CanGoBack)
            {
                AppFrame.GoBack();
            }
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
            SettingsManager.WeatherLoaded = true;
            SettingsManager.OnBoardComplete = true;
            this.Frame.Navigate(typeof(Shell), ViewModel.LocationData);
        }

        private void UpdateAppTheme()
        {
#if WINDOWS_UWP
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.InactiveForegroundColor = Colors.WhiteSmoke;
            titleBar.ButtonInactiveForegroundColor = Colors.WhiteSmoke;
#endif
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
#if WINDOWS_UWP
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
#endif
        }

        private void UpdateTitleBarVisibility(CoreApplicationViewTitleBar coreTitleBar)
        {
#if WINDOWS_UWP
            AppTitleBar.Visibility = coreTitleBar.IsVisible ? Visibility.Visible : Visibility.Collapsed;
#endif
        }
    }
}