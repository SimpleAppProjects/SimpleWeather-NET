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
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using System.Threading.Tasks;
using SimpleWeather.WeatherData;

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

        private WeatherManager wm;

        private bool IsKeyVerfied()
        {
            if (localSettings.Containers.ContainsKey(WeatherAPI.WeatherUnderground))
            {
                if (localSettings.Containers[WeatherAPI.WeatherUnderground].Values.TryGetValue(KEY_APIKEY_VERIFIED, out object value))
                    return (bool)value;
            }

            return false;
        }

        private void SetKeyVerified(bool value)
        {
            localSettings.Containers[WeatherAPI.WeatherUnderground].Values[KEY_APIKEY_VERIFIED] = value;
        }

        public SettingsPage()
        {
            this.InitializeComponent();

            wm = WeatherManager.GetInstance();
            RestoreSettings();
        }

        private void RestoreSettings()
        {
            localSettings.CreateContainer(WeatherAPI.WeatherUnderground, ApplicationDataCreateDisposition.Always);

            // Temperature
            Fahrenheit.IsChecked = Settings.IsFahrenheit;
            Celsius.IsChecked = !Settings.IsFahrenheit;

            // Location
            FollowGPS.IsOn = Settings.FollowGPS;

            // Weather Providers
            APIComboBox.ItemsSource = WeatherAPI.APIs;
            APIComboBox.DisplayMemberPath = "Display";
            APIComboBox.SelectedValuePath = "Value";

            wm.UpdateAPI();

            APIComboBox.SelectedValue = Settings.API;

            if (wm.KeyRequired)
            {
                if (!String.IsNullOrWhiteSpace(Settings.API_KEY) && !keyVerified)
                    keyVerified = true;

                KeyPanel.Visibility = Visibility.Visible;
            }
            else
            {
                keyVerified = false;
                localSettings.Containers[WeatherAPI.WeatherUnderground].Values.Remove(KEY_APIKEY_VERIFIED);

                KeyPanel.Visibility = Visibility.Collapsed;
            }

            // Update Interval
            switch (Settings.RefreshInterval)
            {
                case 15:
                    RefreshComboBox.SelectedIndex = 0;
                    break;
                case 30:
                default:
                    RefreshComboBox.SelectedIndex = 1;
                    break;
                case 60:
                    RefreshComboBox.SelectedIndex = 2;
                    break;
                case 180:
                    RefreshComboBox.SelectedIndex = 3;
                    break;
                case 360:
                    RefreshComboBox.SelectedIndex = 4;
                    break;
            }

            KeyEntry.Text = Settings.API_KEY;
            UpdateKeyBorder();

            Version.Text = string.Format("v{0}.{1}.{2}",
                Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

            // Event Listeners
            SystemNavigationManager.GetForCurrentView().BackRequested += SettingsPage_BackRequested;
            Fahrenheit.Checked += Fahrenheit_Checked;
            Celsius.Checked += Celsius_Checked;
            FollowGPS.Toggled += FollowGPS_Toggled;
            APIComboBox.SelectionChanged += APIComboBox_SelectionChanged;
            RefreshComboBox.SelectionChanged += RefreshComboBox_SelectionChanged;
            if (OSSLicenseWebview != null)
                OSSLicenseWebview.NavigationStarting += OSSLicenseWebview_NavigationStarting;
        }

        private async void SettingsPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Settings.API_KEY) && WeatherManager.IsKeyRequired(APIComboBox.SelectedValue.ToString()))
            {
                KeyBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                await new MessageDialog(App.ResLoader.GetString("Msg_EnterAPIKey")).ShowAsync();
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Settings.API_KEY) && WeatherManager.IsKeyRequired(APIComboBox.SelectedValue.ToString()))
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
                string API = APIComboBox.SelectedValue.ToString();

                if (await WeatherManager.IsKeyValid(key, API))
                {
                    KeyEntry.Text = Settings.API_KEY = key;
                    Settings.API = API;
                    wm.UpdateAPI();

                    // TODO: try not to trigger right away
                    // Do so when navigating away
                    await App.BGTaskHandler.RequestAppTrigger();

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
            string API = box.SelectedValue.ToString();

            if (WeatherManager.IsKeyRequired(API))
            {
                if (KeyPanel != null)
                    KeyPanel.Visibility = Visibility.Visible;

                //if (!String.IsNullOrWhiteSpace(Settings.API_KEY) && !keyVerified)
                //    keyVerified = true;

                if (keyVerified)
                {
                    Settings.API = API;
                    Task.Run(async () => await App.BGTaskHandler.RequestAppTrigger());
                }
            }
            else
            {
                if (KeyPanel != null)
                    KeyPanel.Visibility = Visibility.Collapsed;
                Settings.API = API;
                // Clear API KEY entry to avoid issues
                Settings.API_KEY = String.Empty;

                keyVerified = false;
                localSettings.Containers[WeatherAPI.WeatherUnderground].Values.Remove(KEY_APIKEY_VERIFIED);
            }

            wm.UpdateAPI();
            UpdateKeyBorder();
        }

        private void KeyEntry_GotFocus(object sender, RoutedEventArgs e)
        {
            KeyBorder.BorderBrush = new SolidColorBrush(Colors.DarkGray);
        }

        private void RefreshComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            int index = box.SelectedIndex;

            if (index == 0)
                Settings.RefreshInterval = 15; // 15 min
            else if (index == 1)
                Settings.RefreshInterval = 30; // 30 min
            else if (index == 2)
                Settings.RefreshInterval = 60; // 1 hr
            else if (index == 3)
                Settings.RefreshInterval = 180; // 3 hr
            else if (index == 4)
                Settings.RefreshInterval = 360; // 6 hr

            // Re-register background task
            Task.Run(() => App.BGTaskHandler.RegisterBackgroundTask());
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

            Task.Run(async () => await App.BGTaskHandler.RequestAppTrigger());
        }

        private void Celsius_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Unit = Settings.Celsius;

            Task.Run(async () => await App.BGTaskHandler.RequestAppTrigger());
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
                        // Reset home location data
                        //Settings.SaveLastGPSLocData(new WeatherData.LocationData());
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
            await App.BGTaskHandler.RequestAppTrigger();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pivot = sender as Pivot;

            if (pivot.SelectedIndex == 2)
            {
                // Load webview
                var webview = (FrameworkElement)FindName("OSSLicenseWebview");
                webview.Visibility = Visibility.Visible;

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
