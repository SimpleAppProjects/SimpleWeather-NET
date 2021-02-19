using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Main;
using SimpleWeather.UWP.Radar;
using SimpleWeather.UWP.Tiles;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

#if !DEBUG

using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;

#endif

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings_General : Page, IBackRequestedPage, ISnackbarManager, IFrameContentPage
    {
        private WeatherManager wm;

        private bool RequestAppTrigger = false;

        private SnackbarManager SnackMgr;

        public Settings_General()
        {
            this.InitializeComponent();

            wm = WeatherManager.GetInstance();
            RestoreSettings();

            // Event Listeners
            FollowGPS.Toggled += FollowGPS_Toggled;
            AlertSwitch.Toggled += AlertSwitch_Toggled;
            APIComboBox.SelectionChanged += APIComboBox_SelectionChanged;
            RefreshComboBox.SelectionChanged += RefreshComboBox_SelectionChanged;
            RadarComboBox.SelectionChanged += RadarComboBox_SelectionChanged;
            PersonalKeySwitch.Toggled += PersonalKeySwitch_Toggled;
            LightMode.Checked += LightMode_Checked;
            DarkMode.Checked += DarkMode_Checked;
            SystemMode.Checked += SystemMode_Checked;

            AnalyticsLogger.LogEvent("Settings_General");
        }

        public void InitSnackManager()
        {
            if (SnackMgr == null)
            {
                SnackMgr = new SnackbarManager(Content as Panel);
            }
        }

        public void ShowSnackbar(Snackbar snackbar)
        {
            SnackMgr?.Show(snackbar);
        }

        public void DismissAllSnackbars()
        {
            SnackMgr?.DismissAll();
        }

        public void UnloadSnackManager()
        {
            DismissAllSnackbars();
            SnackMgr = null;
        }

        private void RestoreSettings()
        {
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

            // Radar
            RadarComboBox.ItemsSource = RadarProvider.RadarAPIProviders;
            RadarComboBox.DisplayMemberPath = "Display";
            RadarComboBox.SelectedValuePath = "Value";
            RadarComboBox.SelectedValue = RadarProvider.GetRadarProvider();

            // Theme
            UserThemeMode userTheme = Settings.UserTheme;
            SystemMode.IsChecked = userTheme == UserThemeMode.System;
            LightMode.IsChecked = userTheme == UserThemeMode.Light;
            DarkMode.IsChecked = userTheme == UserThemeMode.Dark;
        }

        public Task<bool> OnBackRequested()
        {
            if (Settings.UsePersonalKey && String.IsNullOrWhiteSpace(Settings.API_KEY) && WeatherManager.IsKeyRequired(APIComboBox.SelectedValue.ToString()))
            {
                KeyBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                ShowSnackbar(Snackbar.Make(App.ResLoader.GetString("Msg_EnterAPIKey"), SnackbarDuration.Long));
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public void OnNavigatedToPage(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnalyticsLogger.LogEvent("Settings_General: OnNavigatedToPage");
            InitSnackManager();
            RequestAppTrigger = false;
            Application.Current.Suspending += OnSuspending;
            RestoreSettings();
        }

        public void OnNavigatedFromPage(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            UnloadSnackManager();
        }

        public void OnNavigatingFromPage(NavigatingCancelEventArgs e)
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

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            if (Settings.UsePersonalKey && String.IsNullOrWhiteSpace(Settings.API_KEY) && WeatherManager.IsKeyRequired(APIComboBox.SelectedValue.ToString()))
            {
                // Fallback to supported weather provider
                string API = RemoteConfig.RemoteConfig.GetDefaultWeatherProvider();
                APIComboBox.SelectedValue = API;
                Settings.API = API;
                wm.UpdateAPI();

                // If key exists, go ahead
                Settings.UsePersonalKey = false;
                Settings.KeyVerified = true;
            }
        }

        private void AlertSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch sw = sender as ToggleSwitch;
            Settings.ShowAlerts = sw.IsOn;
        }

        private async void KeyEntry_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("Settings_General: KeyEntry_Tapped");

            var keydialog = new KeyEntryDialog()
            {
                RequestedTheme = Shell.Instance.AppFrame.RequestedTheme
            };

            keydialog.PrimaryButtonClick += async (ContentDialog dialog, ContentDialogButtonClickEventArgs args) =>
            {
                var diag = dialog as KeyEntryDialog;

                string key = diag.Key;
                string API = APIComboBox.SelectedValue.ToString();

                try
                {
                    if (await WeatherManager.IsKeyValid(key, API))
                    {
                        await Dispatcher.RunOnUIThread(() =>
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
                        });
                    }
                    else
                    {
                        await Dispatcher.RunOnUIThread(() =>
                        {
                            diag.CanClose = false;
                        });
                    }
                }
                catch (WeatherException ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "Settings: KeyEntry: invalid key");
                    await Dispatcher.RunOnUIThread(() =>
                    {
                        ShowSnackbar(Snackbar.Make(ex.Message, SnackbarDuration.Short));
                    });
                }
            };
            await Dispatcher.RunOnUIThread(async () =>
            {
                await keydialog.ShowAsync();
            });
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

            if (API == WeatherAPI.Here && !Extras.ExtrasLibrary.IsEnabled())
            {
                var prevItem = e.RemovedItems?.FirstOrDefault() as SimpleWeather.Controls.ProviderEntry;
                // Revert value
                box.SelectedValue = prevItem.Value;
                // show premium popup
                Frame.Navigate(typeof(Extras.Store.PremiumPage));
                return;
            }

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
                    AnalyticsLogger.LogEvent("Update API", new Dictionary<string, string>()
                    {
                        { "API", Settings.API },
                        { "API_IsInternalKey", (!Settings.UsePersonalKey).ToString() }
                    });
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

                AnalyticsLogger.LogEvent("Update API", new Dictionary<string, string>()
                {
                    { "API", Settings.API },
                    { "API_IsInternalKey", (!Settings.UsePersonalKey).ToString() }
                });
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
            AnalyticsLogger.LogEvent("Settings_General: PersonalKeySwitch_Toggled");

            ToggleSwitch sw = sender as ToggleSwitch;
            Settings.UsePersonalKey = sw.IsOn;

            if (!sw.IsOn)
            {
                string API = APIComboBox.SelectedValue.ToString();
                var selectedWProv = WeatherManager.GetProvider(API);

                if (!selectedWProv.KeyRequired || !String.IsNullOrWhiteSpace(selectedWProv.GetAPIKey()))
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

        private void RadarComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            string RadarAPI = box.SelectedValue.ToString();
            var radarProviderValues = Enum.GetValues(typeof(RadarProvider.RadarProviders));
            RadarProvider.RadarAPIProvider = radarProviderValues
                .Cast<RadarProvider.RadarProviders>()
                .FirstOrDefault(@enum => Equals(@enum.GetStringValue(), RadarAPI));
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

        private async void FollowGPS_Toggled(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("Settings_General: FollowGPS_Toggled");

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

                switch (geoStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        // Reset home location data
                        //Settings.SaveLastGPSLocData(new WeatherData.LocationData());
                        break;

                    case GeolocationAccessStatus.Denied:
                        await Dispatcher.RunOnUIThread(() =>
                        {
                            var snackbar = Snackbar.Make(App.ResLoader.GetString("Msg_LocDeniedSettings"), SnackbarDuration.Long);
                            snackbar.SetAction(App.ResLoader.GetString("Label_Settings"), async () =>
                            {
                                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
                            });
                            ShowSnackbar(snackbar);
                            sw.IsOn = false;
                        });
                        break;

                    case GeolocationAccessStatus.Unspecified:
                        await Dispatcher.RunOnUIThread(() =>
                        {
                            ShowSnackbar(Snackbar.Make(App.ResLoader.GetString("Error_Location"), SnackbarDuration.Short));
                            sw.IsOn = false;
                        });
                        break;

                    default:
                        break;
                }
            }

            // Update ids when switching GPS feature
            if (await Dispatcher.RunOnUIThread(() => sw.IsOn))
            {
                var prevLoc = (await Settings.GetFavorites()).FirstOrDefault();
                if (prevLoc?.query != null && SecondaryTileUtils.Exists(prevLoc.query))
                {
                    var gpsLoc = await Settings.GetLastGPSLocData();
                    if (gpsLoc?.query == null)
                        Settings.SaveLastGPSLocData(prevLoc);
                    else
                        SecondaryTileUtils.UpdateTileId(prevLoc.query, Constants.KEY_GPS);
                }
            }
            else
            {
                if (SecondaryTileUtils.Exists(Constants.KEY_GPS))
                {
                    var favLoc = (await Settings.GetFavorites()).FirstOrDefault();
                    if (favLoc?.IsValid() == true)
                        SecondaryTileUtils.UpdateTileId(Constants.KEY_GPS, favLoc.query);
                }
            }

            await Dispatcher.RunOnUIThread(() =>
            {
                Settings.FollowGPS = sw.IsOn;
            });
            RequestAppTrigger = true;
        }

        private void SystemMode_Checked(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("Settings_General: SystemMode_Checked");

            Settings.UserTheme = UserThemeMode.System;
            Shell.Instance.UpdateAppTheme();
        }

        private void DarkMode_Checked(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("Settings_General: DarkMode_Checked");

            Settings.UserTheme = UserThemeMode.Dark;
            Shell.Instance.UpdateAppTheme();
        }

        private void LightMode_Checked(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("Settings_General: LightMode_Checked");

            Settings.UserTheme = UserThemeMode.Light;
            Shell.Instance.UpdateAppTheme();
        }
    }
}