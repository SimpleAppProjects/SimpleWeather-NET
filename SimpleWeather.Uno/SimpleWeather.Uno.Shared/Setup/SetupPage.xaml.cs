using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using SimpleWeather.ComponentModel;
using SimpleWeather.Preferences;
using SimpleWeather.Uno.Helpers;
using SimpleWeather.Uno.Main;
using SimpleWeather.Uno.ViewModels;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace SimpleWeather.Uno.Setup
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

            // remove the solid-colored backgrounds behind the caption controls and system back button if we are in left mode
            // This is done when the app is loaded since before that the actual theme that is used is not "determined" yet
            Loaded += delegate (object sender, RoutedEventArgs e)
            {
#if WINDOWS
                SetupAppTitleBar();
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

                        if (ApiInformation.IsPropertyPresent("Microsoft.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo", "Effect"))
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
#if WINDOWS
            UpdateTitleBarTheme();
#endif
        }

        private async void UISettings_ColorValuesChanged(UISettings sender, object args)
        {
            // NOTE: Run on UI Thread since this may be called off the main thread
            await DispatcherQueue?.EnqueueAsync(() =>
            {
                UpdateAppTheme();
            });
        }
    }
}