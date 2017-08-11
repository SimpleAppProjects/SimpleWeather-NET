using System;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static string KEY_APIKEY_VERIFIED = "API_KEY_VERIFIED";
        private bool keyVerified { get { return IsKeyVerfied(); } set { SetKeyVerified(value); } }

        private bool IsKeyVerfied()
        {
            if (localSettings.Containers.ContainsKey(Settings.API_WUnderground))
            {
                if (localSettings.Containers[Settings.API_WUnderground].Values.TryGetValue(KEY_APIKEY_VERIFIED, out object value))
                    return (bool)value;
            }

            return false;
        }

        private void SetKeyVerified(bool value)
        {
            localSettings.Containers[Settings.API_WUnderground].Values[KEY_APIKEY_VERIFIED] = value;
        }

        public SettingsPage()
        {
            this.InitializeComponent();

            RestoreSettings();
        }

        private void RestoreSettings()
        {
            localSettings.CreateContainer(Settings.API_WUnderground, ApplicationDataCreateDisposition.Always);

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
            FollowGPS.IsOn = Settings.FollowGPS;

            // Weather Providers
            if (Settings.API == Settings.API_WUnderground)
            {
                if (!String.IsNullOrWhiteSpace(Settings.API_KEY) && !keyVerified)
                    keyVerified = true;

                APIComboBox.SelectedIndex = 0;
                KeyPanel.Visibility = Visibility.Visible;
            }
            else if (Settings.API == Settings.API_Yahoo)
            {
                keyVerified = false;
                localSettings.Containers[Settings.API_WUnderground].Values.Remove(KEY_APIKEY_VERIFIED);

                APIComboBox.SelectedIndex = 1;
                KeyPanel.Visibility = Visibility.Collapsed;
            }

            KeyEntry.Text = Settings.API_KEY;
            UpdateKeyBorder();

            Version.Text = string.Format("v{0}.{1}.{2}",
                Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

            // Event Listeners
            SystemNavigationManager.GetForCurrentView().BackRequested += SettingsPage_BackRequested;
            APIComboBox.SelectionChanged += APIComboBox_SelectionChanged;
            OSSLicenseWebview.NavigationStarting += OSSLicenseWebview_NavigationStarting;
        }

        private async void SettingsPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Settings.API_KEY) && APIComboBox.SelectedIndex == 0)
            {
                KeyBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                await new MessageDialog(App.ResLoader.GetString("Msg_EnterAPIKey")).ShowAsync();
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Settings.API_KEY) && APIComboBox.SelectedIndex == 0)
            {
                e.Cancel = true;
            }
        }

        private async void KeyEntry_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var keydialog = new Controls.KeyEntryDialog();
            keydialog.PrimaryButtonClick += async (ContentDialog dialog, ContentDialogButtonClickEventArgs args) =>
            {
                var diag = dialog as Controls.KeyEntryDialog;

                string key = diag.Key;
                if (await WeatherUnderground.KeyCheckQuery.IsValid(key))
                {
                    KeyEntry.Text = Settings.API_KEY = key;
                    Settings.API = Settings.API_WUnderground;

                    keyVerified = true;
                    UpdateKeyBorder();

                    diag.CanClose = true;
                    diag.Hide();
                }
                else
                {
                    diag.CanClose = false;
                }
            };
            await keydialog.ShowAsync();
        }

        private void UpdateKeyBorder()
        {
            if (keyVerified)
                KeyBorder.BorderBrush = new SolidColorBrush(Colors.Green);
            else
                KeyBorder.BorderBrush = new SolidColorBrush(Colors.DarkGray);
        }

        private void APIComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            int index = box.SelectedIndex;

            if (index == 0)
            {
                // WeatherUnderground
                if (KeyPanel != null)
                    KeyPanel.Visibility = Visibility.Visible;

                if (!String.IsNullOrWhiteSpace(Settings.API_KEY) && !keyVerified)
                    keyVerified = true;

                if (keyVerified)
                    Settings.API = Settings.API_WUnderground;
            }
            else if (index == 1)
            {
                // Yahoo Weather
                if (KeyPanel != null)
                    KeyPanel.Visibility = Visibility.Collapsed;
                Settings.API = Settings.API_Yahoo;

                keyVerified = false;
                localSettings.Containers[Settings.API_WUnderground].Values.Remove(KEY_APIKEY_VERIFIED);
            }

            UpdateKeyBorder();
        }

        private void KeyEntry_GotFocus(object sender, RoutedEventArgs e)
        {
            KeyBorder.BorderBrush = new SolidColorBrush(Colors.DarkGray);
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
                GeolocationAccessStatus geoStatus = GeolocationAccessStatus.Unspecified;

                try
                {
                    // Catch error in case dialog is dismissed
                    geoStatus = await Geolocator.RequestAccessAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }

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
                        error = new MessageDialog(App.ResLoader.GetString("Msg_LocDeniedSettings"), App.ResLoader.GetString("Label_ErrLocationDenied"));
                        error.Commands.Add(new UICommand(App.ResLoader.GetString("Label_Settings"), async (command) =>
                        {
                            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
                        }, 0));
                        error.Commands.Add(new UICommand(App.ResLoader.GetString("Label_Cancel"), null, 1));
                        error.DefaultCommandIndex = 0;
                        error.CancelCommandIndex = 1;
                        await error.ShowAsync();
                        sw.IsOn = false;
                        break;
                    case GeolocationAccessStatus.Unspecified:
                        error = new MessageDialog(App.ResLoader.GetString("Error_Location"), App.ResLoader.GetString("Label_ErrorLocation"));
                        await error.ShowAsync();
                        sw.IsOn = false;
                        break;
                }
            }

            Settings.FollowGPS = sw.IsOn;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pivot = sender as Pivot;

            if (pivot.SelectedIndex == 2)
            {
                if (OSSLicenseWebview.Source == null)
                {
                    OSSLicenseWebview.NavigationStarting -= OSSLicenseWebview_NavigationStarting;
                    OSSLicenseWebview.Navigate(new Uri("ms-appx-web:///Assets/Credits/licenses.html"));
                    OSSLicenseWebview.NavigationStarting += OSSLicenseWebview_NavigationStarting;
                }
            }
        }
    }
}
