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
using System.Globalization;
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
using SharedExtras = SimpleWeather.Shared.Extras.Extras;

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
        private readonly WeatherManager wm;
        private SnackbarManager SnackMgr;

        private readonly HashSet<String> ActionQueue;

        private readonly IReadOnlyList<SimpleWeather.Controls.ComboBoxItem> RefreshOptions = new List<SimpleWeather.Controls.ComboBoxItem>
        {
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_60min"), "60"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_2hrs"), "120"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_3hrs"), "180"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_6hrs"), "360"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_12hrs"), "720"),
        };

        private readonly IReadOnlyList<SimpleWeather.Controls.ComboBoxItem> PremiumRefreshOptions = new List<SimpleWeather.Controls.ComboBoxItem>
        {
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_30min"), "30"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_60min"), "60"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_2hrs"), "120"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_3hrs"), "180"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_6hrs"), "360"),
            new SimpleWeather.Controls.ComboBoxItem(App.ResLoader.GetString("refresh_12hrs"), "720"),
        };

        public Settings_General()
        {
            this.InitializeComponent();

            wm = WeatherManager.GetInstance();
            ActionQueue = new HashSet<string>();

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
            DailyNotifSwitch.Toggled += DailyNotifSwitch_Toggled;
            DailyNotifTimePicker.SelectedTimeChanged += DailyNotifTimePicker_SelectedTimeChanged;
            PoPChanceNotifSwitch.Toggled += PoPChanceNotifSwitch_Toggled;

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
            if (APIComboBox.ItemsSource == null || !WeatherAPI.APIs.SequenceEqual(APIComboBox.ItemsSource as IEnumerable<SimpleWeather.Controls.ProviderEntry>))
            {
                APIComboBox.ItemsSource = WeatherAPI.APIs;
            }
            APIComboBox.DisplayMemberPath = "Display";
            APIComboBox.SelectedValuePath = "Value";

            wm.UpdateAPI();

            var selectedProvider = Settings.API;
            APIComboBox.SelectedValue = selectedProvider;

            // Refresh interval
            if (Extras.ExtrasLibrary.IsEnabled())
            {
                RefreshComboBox.ItemsSource = PremiumRefreshOptions;
            }
            else
            {
                RefreshComboBox.ItemsSource = RefreshOptions;
            }
            RefreshComboBox.DisplayMemberPath = "Display";
            RefreshComboBox.SelectedValuePath = "Value";

            if (wm.KeyRequired)
            {
                if (!string.IsNullOrWhiteSpace(Settings.APIKey) && !Settings.KeysVerified[selectedProvider])
                    Settings.KeysVerified[selectedProvider] = true;

                PersonalKeySwitch.IsOn = Settings.UsePersonalKey;

                if (string.IsNullOrWhiteSpace(wm.GetAPIKey()))
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
                    Settings.KeysVerified[selectedProvider] = true;
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
                Settings.KeysVerified[selectedProvider] = false;
                // Clear API KEY entry to avoid issues
                Settings.APIKey = String.Empty;
                KeyPanel.Visibility = Visibility.Collapsed;
            }

            // Update Interval
            switch (Settings.RefreshInterval)
            {
                case 30:
                    RefreshComboBox.SelectedValue = "30";
                    break;

                case 60:
                    RefreshComboBox.SelectedValue = "60";
                    break;

                case 120:
                    RefreshComboBox.SelectedValue = "120";
                    break;

                case 180:
                    RefreshComboBox.SelectedValue = "180";
                    break;

                case 360:
                    RefreshComboBox.SelectedValue = "360";
                    break;

                case 720:
                    RefreshComboBox.SelectedValue = "720";
                    break;

                default:
                    RefreshComboBox.SelectedValue = Settings.DefaultInterval.ToInvariantString();
                    break;
            }

            KeyEntry.Text = Settings.APIKey;
            UpdateKeyBorder();
            UpdateRegisterLink();

            // Alerts
            AlertSwitch.IsEnabled = wm.SupportsAlerts;
            AlertSwitch.IsOn = Settings.ShowAlerts;

            // Daily Notification
            DailyNotifSwitch.IsOn = Settings.DailyNotificationEnabled;
            DailyNotifTimePicker.SelectedTime = Settings.DailyNotificationTime;

            // Radar
            RadarComboBox.ItemsSource = RadarProvider.GetRadarProviders();
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
            var provider = APIComboBox.SelectedValue.ToString();

            if (Settings.UsePersonalKey && String.IsNullOrWhiteSpace(Settings.APIKeys[provider]) &&
                WeatherManager.IsKeyRequired(provider))
            {
                KeyBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                ShowSnackbar(Snackbar.MakeWarning(App.ResLoader.GetString("message_enter_apikey"), SnackbarDuration.Long));
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public void OnNavigatedToPage(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnalyticsLogger.LogEvent("Settings_General: OnNavigatedToPage");
            InitSnackManager();
            Application.Current.Suspending += OnSuspending;
            App.UnregisterSettingsListener();
            Settings.OnSettingsChanged += Settings_OnSettingsChanged;
            RestoreSettings();
        }

        public void OnNavigatedFromPage(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            UnloadSnackManager();
        }

        public void OnNavigatingFromPage(NavigatingCancelEventArgs e)
        {
            var provider = APIComboBox.SelectedValue.ToString();

            if (Settings.UsePersonalKey && String.IsNullOrWhiteSpace(Settings.APIKeys[provider]) && WeatherManager.IsKeyRequired(provider))
            {
                e.Cancel = true;
            }
            else
            {
                // Unsubscribe from event
                Application.Current.Suspending -= OnSuspending;
                App.RegisterSettingsListener();
                Settings.OnSettingsChanged -= Settings_OnSettingsChanged;

                ProcessQueue();
            }
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var provider = APIComboBox.SelectedValue.ToString();

            if (Settings.UsePersonalKey && String.IsNullOrWhiteSpace(Settings.APIKeys[provider]) && WeatherManager.IsKeyRequired(provider))
            {
                // Fallback to supported weather provider
                string API = RemoteConfig.RemoteConfig.GetDefaultWeatherProvider();
                APIComboBox.SelectedValue = API;
                Settings.API = API;
                wm.UpdateAPI();

                // If key exists, go ahead
                Settings.UsePersonalKey = false;
                Settings.KeysVerified[API] = true;
            }

            App.UnregisterSettingsListener();
            App.RegisterSettingsListener();

            ProcessQueue();
        }

        private bool EnqueueAction(string action)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                return false;
            }
            else
            {
                return ActionQueue.Add(action);
            }
        }

        private void Settings_OnSettingsChanged(SettingsChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Key)) return;

            switch (e.Key)
            {
                case Settings.KEY_API:
                    EnqueueAction(CommonActions.ACTION_SETTINGS_UPDATEAPI);
                    break;
                case Settings.KEY_FOLLOWGPS:
                    EnqueueAction(CommonActions.ACTION_WEATHER_UPDATE);
                    break;
                case Settings.KEY_REFRESHINTERVAL:
                    EnqueueAction(CommonActions.ACTION_WEATHER_REREGISTERTASK);
                    break;
            }
        }

        private void ProcessQueue()
        {
            foreach (var action in ActionQueue)
            {
                switch (action)
                {
                    case CommonActions.ACTION_SETTINGS_UPDATEAPI:
                        wm.UpdateAPI();
                        // Log event
                        AnalyticsLogger.LogEvent("Update API", new Dictionary<string, string>()
                        {
                            { "API", Settings.API },
                            { "API_IsInternalKey", (!Settings.UsePersonalKey).ToString() }
                        });
                        Task.Run(WeatherUpdateBackgroundTask.RequestAppTrigger);
                        break;
                    case CommonActions.ACTION_WEATHER_REREGISTERTASK:
                        Task.Run(() => WeatherUpdateBackgroundTask.RegisterBackgroundTask(true));
                        break;
                    case CommonActions.ACTION_WEATHER_UPDATE:
                        Task.Run(WeatherUpdateBackgroundTask.RequestAppTrigger);
                        break;
                }
            }

            ActionQueue.Clear();
        }

        private void AlertSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch sw = sender as ToggleSwitch;
            Settings.ShowAlerts = sw.IsOn;
        }

        private async void KeyEntry_Tapped(object _, Windows.UI.Xaml.Input.TappedRoutedEventArgs __)
        {
            AnalyticsLogger.LogEvent("Settings_General: KeyEntry_Tapped");

            await ShowKeyEntryDialog();
        }

        private async Task ShowKeyEntryDialog()
        {
            var keydialog = new KeyEntryDialog(APIComboBox.SelectedValue.ToString())
            {
                RequestedTheme = Shell.Instance.AppFrame.RequestedTheme
            };

            keydialog.PrimaryButtonClick += async (ContentDialog dialog, ContentDialogButtonClickEventArgs args) =>
            {
                var diag = dialog as KeyEntryDialog;

                string provider = diag.APIProvider;
                string key = diag.Key;

                try
                {
                    if (await WeatherManager.IsKeyValid(key, provider))
                    {
                        await Dispatcher.RunOnUIThread(() =>
                        {
                            Settings.APIKeys[provider] = key;
                            Settings.API = provider;
                            Settings.KeysVerified[provider] = true;

                            KeyEntry.Text = key;
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
                        ShowSnackbar(Snackbar.MakeWarning(ex.Message, SnackbarDuration.Short));
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
            var selectedProvider = APIComboBox.SelectedValue?.ToString();

            if (Settings.KeysVerified[selectedProvider])
                KeyBorder.BorderBrush = new SolidColorBrush(Colors.Green);
            else
                KeyBorder.BorderBrush = new SolidColorBrush(Colors.DarkGray);
        }

        private void APIComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            string selectedProvider = box.SelectedValue?.ToString();

            if (selectedProvider == null) return;

            if (!SharedExtras.IsWeatherAPISupported(selectedProvider))
            {
                var prevItem = e.RemovedItems?.FirstOrDefault() as SimpleWeather.Controls.ProviderEntry;
                // Revert value
                box.SelectedValue = prevItem.Value;
                // show premium popup
                Frame.Navigate(typeof(Extras.Store.PremiumPage));
                return;
            }

            var selectedWProv = WeatherManager.GetProvider(selectedProvider);

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
                    Settings.KeysVerified[selectedProvider] = true;
                    KeyEntry.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // User is using personal (unverified) keys
                    Settings.KeysVerified[selectedProvider] = false;

                    // Show dialog to set key
                    Dispatcher.LaunchOnUIThread(async () => await ShowKeyEntryDialog());

                    KeyEntry.Visibility = Visibility.Visible;
                }

                if (KeyPanel != null)
                    KeyPanel.Visibility = Visibility.Visible;

                if (Settings.KeysVerified[selectedProvider])
                {
                    Settings.API = selectedProvider;
                }
            }
            else
            {
                Settings.KeysVerified[selectedProvider] = false;

                Settings.API = selectedProvider;
                // Clear API KEY entry to avoid issues
                Settings.APIKeys[selectedProvider] = KeyEntry.Text = String.Empty;

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
                    Settings.KeysVerified[API] = true;
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
            object value = box.SelectedValue;

            if (int.TryParse(value?.ToString(), out int interval))
            {
                Settings.RefreshInterval = interval;
            }
            else
            {
                Settings.RefreshInterval = Settings.DefaultInterval;
            }

            // Re-register background task
            Task.Run(async () => await WeatherUpdateBackgroundTask.RegisterBackgroundTask());
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
                            var snackbar = Snackbar.MakeError(App.ResLoader.GetString("Msg_LocDeniedSettings"), SnackbarDuration.Long);
                            snackbar.SetAction(App.ResLoader.GetString("action_settings"), async () =>
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
                            ShowSnackbar(Snackbar.MakeError(App.ResLoader.GetString("error_retrieve_location"), SnackbarDuration.Short));
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
        }

        private void SystemMode_Checked(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("Settings_General: SystemMode_Checked");

            Settings.UserTheme = UserThemeMode.System;
            App.UpdateAppTheme();
            Shell.Instance.UpdateAppTheme();
        }

        private void DarkMode_Checked(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("Settings_General: DarkMode_Checked");

            Settings.UserTheme = UserThemeMode.Dark;
            App.UpdateAppTheme();
            Shell.Instance.UpdateAppTheme();
        }

        private void LightMode_Checked(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("Settings_General: LightMode_Checked");

            Settings.UserTheme = UserThemeMode.Light;
            App.UpdateAppTheme();
            Shell.Instance.UpdateAppTheme();
        }

        private async void DailyNotifSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch sw = sender as ToggleSwitch;

            if (sw.IsOn)
            {
                if (!await BackgroundTaskHelper.IsBackgroundAccessEnabled().ConfigureAwait(true))
                {
                    var snackbar = Snackbar.MakeError(App.ResLoader.GetString("Msg_BGAccessDeniedSettings"), SnackbarDuration.Long);
                    snackbar.SetAction(App.ResLoader.GetString("action_settings"), async () =>
                    {
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
                    });
                    ShowSnackbar(snackbar);
                    Settings.DailyNotificationEnabled = sw.IsOn = false;
                    return;
                }
            }

            if (sw.IsOn && Extras.ExtrasLibrary.IsEnabled())
            {
                Settings.DailyNotificationEnabled = true;
                // Register task
                _ = Task.Run(() => DailyNotificationTask.RegisterBackgroundTask(true));
            }
            else
            {
                if (sw.IsOn && !Extras.ExtrasLibrary.IsEnabled())
                {
                    // show premium popup
                    Frame.Navigate(typeof(Extras.Store.PremiumPage));
                }
                Settings.DailyNotificationEnabled = sw.IsOn = false;
                // Unregister task
                _ = Task.Run(() => DailyNotificationTask.UnregisterBackgroundTask());
            }
        }

        private void DailyNotifTimePicker_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
        {
            Settings.DailyNotificationTime = args.NewTime ?? Settings.DEFAULT_DAILYNOTIFICATION_TIME;
            if (Settings.DailyNotificationEnabled)
            {
                Task.Run(() => DailyNotificationTask.RegisterBackgroundTask(true));
            }
        }

        private async void PoPChanceNotifSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch sw = sender as ToggleSwitch;

            if (sw.IsOn)
            {
                if (!await BackgroundTaskHelper.IsBackgroundAccessEnabled().ConfigureAwait(true))
                {
                    var snackbar = Snackbar.MakeError(App.ResLoader.GetString("Msg_BGAccessDeniedSettings"), SnackbarDuration.Long);
                    snackbar.SetAction(App.ResLoader.GetString("action_settings"), async () =>
                    {
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
                    });
                    ShowSnackbar(snackbar);
                    Settings.PoPChanceNotificationEnabled = sw.IsOn = false;
                    return;
                }
            }

            if (sw.IsOn && Extras.ExtrasLibrary.IsEnabled())
            {
                Settings.PoPChanceNotificationEnabled = true;
                // Re-register background task if needed
                _ = Task.Run(async () => await WeatherTileUpdaterTask.RegisterBackgroundTask(false));
                _ = Task.Run(async () => await WeatherUpdateBackgroundTask.RegisterBackgroundTask(false));
            }
            else
            {
                if (sw.IsOn && !Extras.ExtrasLibrary.IsEnabled())
                {
                    // show premium popup
                    Frame.Navigate(typeof(Extras.Store.PremiumPage));
                }
                Settings.PoPChanceNotificationEnabled = sw.IsOn = false;
            }
        }
    }
}