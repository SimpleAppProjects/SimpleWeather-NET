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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings_General : Page, IBackRequestedPage
    {
        private WeatherManager wm;

        private bool RequestAppTrigger = false;

        public Settings_General()
        {
            this.InitializeComponent();

            wm = WeatherManager.GetInstance();
            RestoreSettings();

            // Event Listeners
            Fahrenheit.Checked += Fahrenheit_Checked;
            Celsius.Checked += Celsius_Checked;
            FollowGPS.Toggled += FollowGPS_Toggled;
            AlertSwitch.Toggled += AlertSwitch_Toggled;
            APIComboBox.SelectionChanged += APIComboBox_SelectionChanged;
            RefreshComboBox.SelectionChanged += RefreshComboBox_SelectionChanged;
            PersonalKeySwitch.Toggled += PersonalKeySwitch_Toggled;
            LightMode.Checked += LightMode_Checked;
            DarkMode.Checked += DarkMode_Checked;
            SystemMode.Checked += SystemMode_Checked;
        }

        private void RestoreSettings()
        {
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
                if (!String.IsNullOrWhiteSpace(Settings.API_KEY) && !Settings.KeyVerified)
                    Settings.KeyVerified = true;

                PersonalKeySwitch.IsOn = Settings.UsePersonalKey;

                if (String.IsNullOrWhiteSpace(wm.GetAPIKey()))
                {
                    PersonalKeySwitch.IsOn = Settings.UsePersonalKey = true;
                    PersonalKeySwitch.IsEnabled = false;
                    KeyEntry.Visibility = Visibility.Collapsed;
                }
                else
                {
                    PersonalKeySwitch.IsEnabled = true;
                }

                if (!Settings.UsePersonalKey)
                {
                    // We're using our own (verified) keys
                    Settings.KeyVerified = true;
                    KeyEntry.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // User is using personal (unverified) keys
                    //Settings.KeyVerified = false;
                    // Clear API KEY entry to avoid issues
                    //Settings.API_KEY = String.Empty;

                    KeyEntry.Visibility = Visibility.Visible;
                }

                KeyPanel.Visibility = Visibility.Visible;
            }
            else
            {
                Settings.KeyVerified = false;
                // Clear API KEY entry to avoid issues
                Settings.API_KEY = String.Empty;
                KeyPanel.Visibility = Visibility.Collapsed;
            }

            // Update Interval
            switch (Settings.RefreshInterval)
            {
                case 60:
                default:
                    RefreshComboBox.SelectedIndex = 0;
                    break;
                case 120:
                    RefreshComboBox.SelectedIndex = 1;
                    break;
                case 180:
                    RefreshComboBox.SelectedIndex = 2;
                    break;
                case 360:
                    RefreshComboBox.SelectedIndex = 3;
                    break;
                case 720:
                    RefreshComboBox.SelectedIndex = 4;
                    break;
            }

            KeyEntry.Text = Settings.API_KEY;
            UpdateKeyBorder();
            UpdateRegisterLink();

            // Alerts
            AlertSwitch.IsEnabled = wm.SupportsAlerts;
            AlertSwitch.IsOn = Settings.ShowAlerts;

            // Theme
            UserThemeMode userTheme = Settings.UserTheme;
            SystemMode.IsChecked = userTheme == UserThemeMode.System;
            LightMode.IsChecked = userTheme == UserThemeMode.Light;
            DarkMode.IsChecked = userTheme == UserThemeMode.Dark;
        }

        public async Task<bool> OnBackRequested()
        {
            if (Settings.UsePersonalKey && String.IsNullOrWhiteSpace(Settings.API_KEY) && WeatherManager.IsKeyRequired(APIComboBox.SelectedValue.ToString()))
            {
                KeyBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                await new MessageDialog(App.ResLoader.GetString("Msg_EnterAPIKey")).ShowAsync();
                return true;
            }

            return false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RequestAppTrigger = false;
            Application.Current.Suspending += OnSuspending;
            RestoreSettings();
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            if (Settings.UsePersonalKey && String.IsNullOrWhiteSpace(Settings.API_KEY) && WeatherManager.IsKeyRequired(APIComboBox.SelectedValue.ToString()))
            {
                // Fallback to supported weather provider
                APIComboBox.SelectedValue = WeatherAPI.Here;
                Settings.API = WeatherAPI.Here;
                wm.UpdateAPI();

                if (String.IsNullOrWhiteSpace(wm.GetAPIKey()))
                {
                    // If (internal) key doesn't exist, fallback to Yahoo
                    APIComboBox.SelectedValue = WeatherAPI.Yahoo;
                    Settings.API = WeatherAPI.Yahoo;
                    wm.UpdateAPI();
                    Settings.UsePersonalKey = true;
                    Settings.KeyVerified = false;
                }
                else
                {
                    // If key exists, go ahead
                    Settings.UsePersonalKey = false;
                    Settings.KeyVerified = true;
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (Settings.UsePersonalKey && String.IsNullOrWhiteSpace(Settings.API_KEY) && WeatherManager.IsKeyRequired(APIComboBox.SelectedValue.ToString()))
            {
                e.Cancel = true;
            }
            else
            {
                // Unsubscribe from event
                Application.Current.Suspending -= OnSuspending;

                // Trigger background task if necessary
                if (RequestAppTrigger)
                {
                    Task.Run(async () => await WeatherUpdateBackgroundTask.RequestAppTrigger());
                    RequestAppTrigger = false;
                }
            }
        }

        private void AlertSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch sw = sender as ToggleSwitch;
            Settings.ShowAlerts = sw.IsOn;
        }

        private async void KeyEntry_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var keydialog = new Controls.KeyEntryDialog(APIComboBox.SelectedValue.ToString());

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

                    RequestAppTrigger = true;

                    Settings.KeyVerified = true;
                    UpdateKeyBorder();

                    AlertSwitch.IsEnabled = wm.SupportsAlerts;

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
            if (Settings.KeyVerified)
                KeyBorder.BorderBrush = new SolidColorBrush(Colors.Green);
            else
                KeyBorder.BorderBrush = new SolidColorBrush(Colors.DarkGray);
        }

        private void APIComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            string API = box.SelectedValue.ToString();
            var selectedWProv = WeatherManager.GetProvider(API);

            if (selectedWProv.KeyRequired)
            {
                if (String.IsNullOrWhiteSpace(selectedWProv.GetAPIKey()))
                {
                    PersonalKeySwitch.IsOn = Settings.UsePersonalKey = true;
                    PersonalKeySwitch.IsEnabled = false;
                    KeyEntry.Visibility = Visibility.Collapsed;
                }
                else
                {
                    PersonalKeySwitch.IsEnabled = true;
                }

                if (!Settings.UsePersonalKey)
                {
                    // We're using our own (verified) keys
                    Settings.KeyVerified = true;
                    KeyEntry.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // User is using personal (unverified) keys
                    Settings.KeyVerified = false;
                    // Clear API KEY entry to avoid issues
                    Settings.API_KEY = String.Empty;

                    KeyEntry.Visibility = Visibility.Visible;
                }

                if (KeyPanel != null)
                    KeyPanel.Visibility = Visibility.Visible;

                if (Settings.KeyVerified)
                {
                    Settings.API = API;
                    RequestAppTrigger = true;
                }
            }
            else
            {
                Settings.KeyVerified = false;

                Settings.API = API;
                // Clear API KEY entry to avoid issues
                Settings.API_KEY = KeyEntry.Text = String.Empty;

                if (KeyPanel != null)
                    KeyPanel.Visibility = Visibility.Collapsed;
            }

            wm.UpdateAPI();
            UpdateKeyBorder();
            UpdateRegisterLink();

            AlertSwitch.IsEnabled = wm.SupportsAlerts;
            if (!wm.SupportsAlerts)
                AlertSwitch.IsOn = false;
        }

        private void PersonalKeySwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch sw = sender as ToggleSwitch;
            Settings.UsePersonalKey = sw.IsOn;

            if (!sw.IsOn)
            {
                string API = APIComboBox.SelectedValue.ToString();
                var selectedWProv = WeatherManager.GetProvider(API);

                if (!String.IsNullOrWhiteSpace(selectedWProv.GetAPIKey()))
                {
                    // We're using our own (verified) keys
                    Settings.KeyVerified = true;
                    Settings.API = API;
                }
            }

            KeyEntry.Visibility = sw.IsOn ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateRegisterLink()
        {
            string API = APIComboBox?.SelectedValue?.ToString();
            RegisterKeyButton.NavigateUri =
                new Uri(WeatherAPI.APIs.First(prov => prov.Value == API).APIRegisterURL);
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
                Settings.RefreshInterval = 60; // 1 hr
            else if (index == 1)
                Settings.RefreshInterval = 120; // 2 hr
            else if (index == 2)
                Settings.RefreshInterval = 180; // 3 hr
            else if (index == 3)
                Settings.RefreshInterval = 360; // 6 hr
            else if (index == 4)
                Settings.RefreshInterval = 720; // 12 hr

            // Re-register background task
            Task.Run(WeatherUpdateBackgroundTask.RegisterBackgroundTask);
        }

        private void Fahrenheit_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Unit = Settings.Fahrenheit;
            RequestAppTrigger = true;
        }

        private void Celsius_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Unit = Settings.Celsius;
            RequestAppTrigger = true;
        }

        private async void FollowGPS_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch sw = sender as ToggleSwitch;

            if (sw.IsOn)
            {
                var geoStatus = GeolocationAccessStatus.Unspecified;

                try
                {
                    // Catch error in case dialog is dismissed
                    geoStatus = await Geolocator.RequestAccessAsync();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "SettingsPage: error getting location permission");
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
                    default:
                        break;
                }
            }

            // Update ids when switching GPS feature
            if (sw.IsOn)
            {
                var prevLoc = (await Settings.GetFavorites()).FirstOrDefault();
                if (prevLoc?.query != null && SecondaryTileUtils.Exists(prevLoc.query))
                {
                    var gpsLoc = await Settings.GetLastGPSLocData();
                    if (gpsLoc?.query == null)
                        Settings.SaveLastGPSLocData(prevLoc);
                    else
                        await SecondaryTileUtils.UpdateTileId(prevLoc.query, gpsLoc.query);
                }
            }
            else
            {
                var prevLoc = await Settings.GetLastGPSLocData();
                if (prevLoc?.query != null && SecondaryTileUtils.Exists(prevLoc.query))
                {
                    var favLoc = (await Settings.GetFavorites()).FirstOrDefault();
                    if (favLoc?.query != null)
                        await SecondaryTileUtils.UpdateTileId(prevLoc.query, favLoc.query);
                }
            }

            Settings.FollowGPS = sw.IsOn;
            RequestAppTrigger = true;
        }

        private void SystemMode_Checked(object sender, RoutedEventArgs e)
        {
            Settings.UserTheme = UserThemeMode.System;
            Shell.Instance.UpdateAppTheme();
        }

        private void DarkMode_Checked(object sender, RoutedEventArgs e)
        {
            Settings.UserTheme = UserThemeMode.Dark;
            Shell.Instance.UpdateAppTheme();
        }

        private void LightMode_Checked(object sender, RoutedEventArgs e)
        {
            Settings.UserTheme = UserThemeMode.Light;
            Shell.Instance.UpdateAppTheme();
        }
    }
}
