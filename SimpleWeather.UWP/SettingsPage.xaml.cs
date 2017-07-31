using System;
using SimpleWeather.Utils;
using Windows.ApplicationModel;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            RestoreSettings();
        }

        private async void RestoreSettings()
        {
            // Temperature
            if (Settings.Unit == Settings.Fahrenheit)
            {
                Fahrenheit.IsChecked = true;
                Celsius.IsChecked = false;
            }
            else
            {
                Fahrenheit.IsChecked = false;
                Celsius.IsChecked = true;
            }

            // Location
            GeolocationAccessStatus geoStatus = await Geolocator.RequestAccessAsync();
            if (geoStatus != GeolocationAccessStatus.Allowed)
            {
                Settings.FollowGPS = FollowGPS.IsOn = false;
            }
            else
                FollowGPS.IsOn = Settings.FollowGPS;

            Version.Text = string.Format("v{0}.{1}.{2}",
                Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

            OSSLicenseWebview.NavigationStarting += OSSLicenseWebview_NavigationStarting;
        }

        private void OSSLicenseWebview_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            // Cancel navigation and open link in ext. browser
            args.Cancel = true;
            Windows.Foundation.IAsyncOperation<bool> b = Windows.System.Launcher.LaunchUriAsync(args.Uri);
        }

        private void Fahrenheit_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Unit = Settings.Fahrenheit;
        }

        private void Celsius_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Unit = Settings.Celsius;
        }

        private async void FollowGPS_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch sw = sender as ToggleSwitch;

            if (sw.IsOn)
            {
                GeolocationAccessStatus geoStatus = await Geolocator.RequestAccessAsync();
                // Setup error just in case
                MessageDialog error = null;

                switch (geoStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        if (CoreApplication.Properties.ContainsKey("HomeChanged"))
                            CoreApplication.Properties["HomeChanged"] = true;
                        else
                            CoreApplication.Properties.Add("HomeChanged", true);
                        break;
                    case GeolocationAccessStatus.Denied:
                        error = new MessageDialog("Access to location was denied. Please enable in Settings.", "Location access denied");
                        error.Commands.Add(new UICommand("Settings", async (command) =>
                        {
                            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
                        }, 0));
                        error.Commands.Add(new UICommand("Cancel", null, 1));
                        error.DefaultCommandIndex = 0;
                        error.CancelCommandIndex = 1;
                        await error.ShowAsync();
                        sw.IsOn = false;
                        break;
                    case GeolocationAccessStatus.Unspecified:
                        error = new MessageDialog("Unable to retrieve location status", "Location access error");
                        await error.ShowAsync();
                        sw.IsOn = false;
                        break;
                }
            }

            Settings.FollowGPS = sw.IsOn;
        }
    }
}
