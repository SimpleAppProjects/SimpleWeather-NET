using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        public Frame AppFrame { get { return FrameContent; } }
        public static Shell Instance { get; set; }
        public Brush BurgerBackground
        {
            get { return HamburgerButton.Background; }
            set { HamburgerButton.Background = value; }
        }

        public Shell()
        {
            Instance = this;
            this.InitializeComponent();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested += Shell_BackRequested;

            AppFrame.Navigating += AppFrame_Navigating;
            AppFrame.Navigated += AppFrame_Navigated;
            AppFrame.CacheSize = 1;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Setup background task
            Windows.ApplicationModel.Background.BackgroundExecutionManager.RemoveAccess();
            await App.BGTaskHandler.RegisterBackgroundTask();

            // Navigate to WeatherNow page
            if (AppFrame.Content == null)
            {
                AppFrame.Navigate(typeof(WeatherNow), e.Parameter);
            }
        }

        private void AppFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (AppFrame.CanGoBack)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            if (e.SourcePageType == typeof(WeatherNow))
            {
                WeatherButton.Background = new SolidColorBrush(App.AppColor);

                SettingsButton.Background = new SolidColorBrush(Colors.Transparent);
                LocationsButton.Background = new SolidColorBrush(Colors.Transparent);
            }
            else if (e.SourcePageType == typeof(SettingsPage))
            {
                SettingsButton.Background = new SolidColorBrush(App.AppColor);

                WeatherButton.Background = new SolidColorBrush(Colors.Transparent);
                LocationsButton.Background = new SolidColorBrush(Colors.Transparent);
            }
            else if (e.SourcePageType == typeof(LocationsPage))
            {
                LocationsButton.Background = new SolidColorBrush(App.AppColor);

                WeatherButton.Background = new SolidColorBrush(Colors.Transparent);
                SettingsButton.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        private void Shell_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (AppFrame == null)
                return;

            // Navigate back if possible, and if the event has not 
            // already been handled .
            if (AppFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                AppFrame.GoBack();
            }
        }

        private void AppFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (HamBurgerMenu.IsPaneOpen)
                HamBurgerMenu.IsPaneOpen = !HamBurgerMenu.IsPaneOpen;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            HamBurgerMenu.IsPaneOpen = !HamBurgerMenu.IsPaneOpen;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppFrame.SourcePageType != typeof(SettingsPage))
                AppFrame.Navigate(typeof(SettingsPage), null);
        }

        private void WeatherButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppFrame.SourcePageType != typeof(WeatherNow))
            {
                AppFrame.Navigate(typeof(WeatherNow), null);

                AppFrame.BackStack.Clear();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void LocationsButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppFrame.SourcePageType != typeof(LocationsPage))
                AppFrame.Navigate(typeof(LocationsPage), null);
        }
    }
}