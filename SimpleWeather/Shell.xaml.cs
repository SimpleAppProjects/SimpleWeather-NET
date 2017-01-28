using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        public Frame AppFrame { get { return Content; } }

        public Shell()
        {
            this.InitializeComponent();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested += Shell_BackRequested;

            AppFrame.Navigating += AppFrame_Navigating;

            if (AppFrame.Content == null)
            {
                AppFrame.Navigate(typeof(WeatherNow));
            }
        }

        private void Shell_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (AppFrame == null)
                return;

            // Navigate back if possible, and if the event has not 
            // already been handled .
            if (AppFrame.CanGoBack && (AppFrame.SourcePageType != typeof(WeatherNow)) && e.Handled == false)
            {
                e.Handled = true;
                AppFrame.GoBack();
            }
        }

        private void AppFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.SourcePageType != typeof(WeatherNow))
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            if (e.SourcePageType == typeof(WeatherNow))
            {
                WeatherButton.Background = new SolidColorBrush(App.AppColor);

                SettingsButton.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
                LocationsButton.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            }
            else if (e.SourcePageType == typeof(SettingsPage))
            {
                SettingsButton.Background = new SolidColorBrush(App.AppColor);

                WeatherButton.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
                LocationsButton.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            }
            else if (e.SourcePageType == typeof(LocationsPage))
            {
                LocationsButton.Background = new SolidColorBrush(App.AppColor);

                WeatherButton.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
                SettingsButton.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            }

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
                AppFrame.Navigate(typeof(SettingsPage), SettingsButton.Tag);
        }

        private void WeatherButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppFrame.SourcePageType != typeof(WeatherNow))
                AppFrame.Navigate(typeof(WeatherNow), WeatherButton.Tag);
        }

        private void LocationsButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppFrame.SourcePageType != typeof(LocationsPage))
                AppFrame.Navigate(typeof(LocationsPage), LocationsButton.Tag);
        }
    }
}