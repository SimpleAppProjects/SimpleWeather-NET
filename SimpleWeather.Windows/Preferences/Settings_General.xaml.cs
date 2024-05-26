using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SimpleWeather.Extras;
using SimpleWeather.NET.BackgroundTasks;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Helpers;
using SimpleWeather.NET.Main;
using SimpleWeather.NET.Radar;
using SimpleWeather.NET.Tiles;
using SimpleWeather.NET.Widgets;
using SimpleWeather.Preferences;
using SimpleWeather.RemoteConfig;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using System.Globalization;
using Windows.ApplicationModel;
using Windows.Devices.Geolocation;

namespace SimpleWeather.NET.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings_General : Page, IBackRequestedPage, ISnackbarManager, IFrameContentPage
    {
        private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();
        private readonly IRemoteConfigService RemoteConfigService = Ioc.Default.GetService<IRemoteConfigService>();

        private SnackbarManager SnackMgr;

        private readonly HashSet<String> ActionQueue;

        private readonly IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();

        private readonly IReadOnlyList<SimpleWeather.Controls.ComboBoxItem> RefreshOptions = new List<SimpleWeather.Controls.ComboBoxItem>
        {
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_60min"), "60"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_2hrs"), "120"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_3hrs"), "180"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_6hrs"), "360"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_12hrs"), "720"),
        };

        private readonly IReadOnlyList<SimpleWeather.Controls.ComboBoxItem> PremiumRefreshOptions = new List<SimpleWeather.Controls.ComboBoxItem>
        {
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_30min"), "30"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_60min"), "60"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_2hrs"), "120"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_3hrs"), "180"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_6hrs"), "360"),
            new SimpleWeather.Controls.ComboBoxItem(App.Current.ResLoader.GetString("refresh_12hrs"), "720"),
        };

        public Settings_General()
        {
            this.InitializeComponent();

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
            LanguageComboBox.SelectionChanged += LanguageComboBox_SelectionChanged;
            DailyNotifSwitch.Toggled += DailyNotifSwitch_Toggled;
            DailyNotifTimePicker.SelectedTimeChanged += DailyNotifTimePicker_SelectedTimeChanged;
            PoPChanceNotifSwitch.Toggled += PoPChanceNotifSwitch_Toggled;
            PoPChancePct.SelectionChanged += PoPChancePct_SelectionChanged;
            MinAlertSeverity.SelectionChanged += MinAlertSeverity_SelectionChanged;

            AnalyticsLogger.LogEvent("Settings_General");
        }

        public void InitSnackManager()
        {
            SnackMgr ??= new SnackbarManager(Content as Panel);
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
            FollowGPS.IsOn = SettingsManager.FollowGPS;

            // Weather Providers
            if (APIComboBox.ItemsSource == null || !WeatherAPI.APIs.SequenceEqual(APIComboBox.ItemsSource as IEnumerable<SimpleWeather.Controls.ProviderEntry>))
            {
                APIComboBox.ItemsSource = WeatherAPI.APIs.Where(it => RemoteConfigService.IsProviderEnabled(it.Value));
            }
            APIComboBox.DisplayMemberPath = "Display";
            APIComboBox.SelectedValuePath = "Value";

            wm.UpdateAPI();

            var selectedProvider = SettingsManager.API;
            APIComboBox.SelectedValue = selectedProvider ?? RemoteConfigService.GetDefaultWeatherProvider();

            // Refresh interval
            if (ExtrasService.IsEnabled())
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
                if (!string.IsNullOrWhiteSpace(SettingsManager.APIKey) && !SettingsManager.KeysVerified[selectedProvider])
                    SettingsManager.KeysVerified[selectedProvider] = true;

                PersonalKeySwitch.IsOn = SettingsManager.UsePersonalKeys[selectedProvider];

                if (string.IsNullOrWhiteSpace(wm.GetAPIKey()))
                {
                    PersonalKeySwitch.IsOn = SettingsManager.UsePersonalKeys[selectedProvider] = true;
                    PersonalKeySwitch.IsEnabled = false;
                    KeyEntry.Visibility = Visibility.Collapsed;
                }
                else
                {
                    PersonalKeySwitch.IsEnabled = true;
                }

                if (!SettingsManager.UsePersonalKeys[selectedProvider])
                {
                    // We're using our own (verified) keys
                    SettingsManager.KeysVerified[selectedProvider] = true;
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
                SettingsManager.KeysVerified[selectedProvider] = false;
                // Clear API KEY entry to avoid issues
                SettingsManager.APIKey = String.Empty;
                KeyPanel.Visibility = Visibility.Collapsed;
            }

            // Update Interval
            RefreshComboBox.SelectedValue = SettingsManager.RefreshInterval switch
            {
                30 => "30",
                60 => "60",
                120 => "120",
                180 => "180",
                360 => "360",
                720 => "720",
                _ => SettingsManager.DefaultInterval.ToInvariantString(),
            };
            KeyEntry.Text = SettingsManager.APIKey;
            UpdateKeyBorder();
            UpdateRegisterLink();

            // Alerts
            AlertSwitch.IsEnabled = wm.SupportsAlerts;
            AlertSwitch.IsOn = SettingsManager.ShowAlerts;
            MinAlertSeverity.SelectedValue = ((int)SettingsManager.MinimumAlertSeverity).ToInvariantString();

            // Daily Notification
            DailyNotifSwitch.IsOn = SettingsManager.DailyNotificationEnabled;
            DailyNotifTimePicker.SelectedTime = SettingsManager.DailyNotificationTime;

            // Precipitation
            PoPChanceNotifSwitch.IsOn = SettingsManager.PoPChanceNotificationEnabled;
            PoPChancePct.SelectedValue = SettingsManager.PoPChanceMinimumPercentage.ToInvariantString();

            // Radar
            RadarComboBox.ItemsSource = RadarProvider.GetRadarProviders();
            RadarComboBox.DisplayMemberPath = "Display";
            RadarComboBox.SelectedValuePath = "Value";
            RadarComboBox.SelectedValue = RadarProvider.GetRadarProvider();

            // Theme
            UserThemeMode userTheme = SettingsManager.UserTheme;
            SystemMode.IsChecked = userTheme == UserThemeMode.System;
            LightMode.IsChecked = userTheme == UserThemeMode.Light;
            DarkMode.IsChecked = userTheme == UserThemeMode.Dark;

            LanguageComboBox.SelectedValue = LocaleUtils.GetLocaleCode();
            LanguageComboBox.Items.OfType<SimpleWeather.Controls.ComboBoxItem>().ForEach(it =>
            {
                var code = it.Value.ToString();

                if (string.IsNullOrWhiteSpace(code))
                {
                    it.Display = App.Current.ResLoader.GetString("summary_default");
                }
                else
                {
                    var culture = CultureInfo.GetCultureInfo(code, true);
                    it.Display = culture.GetNativeDisplayName(culture);
                }
            });
        }

        public Task<bool> OnBackRequested()
        {
            var provider = APIComboBox.SelectedValue.ToString();

            if (SettingsManager.UsePersonalKeys[provider] && String.IsNullOrWhiteSpace(SettingsManager.APIKeys[provider]) &&
                wm.IsKeyRequired(provider))
            {
                KeyBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                ShowSnackbar(Snackbar.MakeWarning(App.Current.ResLoader.GetString("message_enter_apikey"), SnackbarDuration.Long));
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public void OnNavigatedToPage(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnalyticsLogger.LogEvent("Settings_General: OnNavigatedToPage");
            InitSnackManager();
            App.Current.UnregisterSettingsListener();
            SettingsManager.OnSettingsChanged += Settings_OnSettingsChanged;
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

            if (SettingsManager.UsePersonalKeys[provider] && String.IsNullOrWhiteSpace(SettingsManager.APIKeys[provider]) && wm.IsKeyRequired(provider))
            {
                e.Cancel = true;
            }
            else
            {
                // Unsubscribe from event
                App.Current.RegisterSettingsListener();
                SettingsManager.OnSettingsChanged -= Settings_OnSettingsChanged;

                ProcessQueue();
            }
        }

        private void OnSuspending(object _, SuspendingEventArgs __)
        {
            var provider = APIComboBox.SelectedValue.ToString();

            if (SettingsManager.UsePersonalKeys[provider] && String.IsNullOrWhiteSpace(SettingsManager.APIKeys[provider]) && wm.IsKeyRequired(provider))
            {
                // Fallback to supported weather provider
                string API = RemoteConfigService.GetDefaultWeatherProvider();
                APIComboBox.SelectedValue = API;
                SettingsManager.API = API;
                wm.UpdateAPI();

                // If key exists, go ahead
                SettingsManager.UsePersonalKeys[provider] = false;
                SettingsManager.KeysVerified[API] = true;
            }

            App.Current.UnregisterSettingsListener();
            App.Current.RegisterSettingsListener();

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
                case SettingsManager.KEY_API:
                    EnqueueAction(CommonActions.ACTION_SETTINGS_UPDATEAPI);
                    break;
                case SettingsManager.KEY_FOLLOWGPS:
                    EnqueueAction(CommonActions.ACTION_SETTINGS_UPDATEGPS);
                    break;
                case SettingsManager.KEY_REFRESHINTERVAL:
                    EnqueueAction(CommonActions.ACTION_SETTINGS_UPDATEREFRESH);
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
                        {
                            var api = SettingsManager.API;
                            // Log event
                            AnalyticsLogger.LogEvent("Update API", new Dictionary<string, string>()
                            {
                                { "API", api },
                                { "API_IsInternalKey", (!SettingsManager.UsePersonalKeys[api]).ToString() }
                            });
                            AnalyticsLogger.SetUserProperty(AnalyticsProps.WEATHER_PROVIDER, api);
                            AnalyticsLogger.SetUserProperty(AnalyticsProps.USING_PERSONAL_KEY, SettingsManager.UsePersonalKeys[api]);
                        }
                        Task.Run(WeatherUpdateBackgroundTask.RequestAppTrigger);
                        break;
                    case CommonActions.ACTION_SETTINGS_UPDATEREFRESH:
                        SharedModule.Instance.RequestAction(CommonActions.ACTION_SETTINGS_UPDATEREFRESH);
                        break;
                    case CommonActions.ACTION_SETTINGS_UPDATEGPS:
                        SharedModule.Instance.RequestAction(CommonActions.ACTION_SETTINGS_UPDATEGPS);
                        break;
                }
            }

            ActionQueue.Clear();
        }

        private void AlertSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch sw = sender as ToggleSwitch;
            SettingsManager.ShowAlerts = sw.IsOn;
        }

        private async void KeyEntry_Tapped(object _, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs __)
        {
            AnalyticsLogger.LogEvent("Settings_General: KeyEntry_Tapped");

            await ShowKeyEntryDialog();
        }

        private Task ShowKeyEntryDialog()
        {
            return DispatcherQueue.EnqueueAsync(async () =>
            {
                var keydialog = new KeyEntryDialog(APIComboBox.SelectedValue.ToString())
                {
                    RequestedTheme = Shell.Instance.AppFrame.RequestedTheme,
                    // NOTE: Required to avoid System.ArgumentException: This element is already associated with a XamlRoot...
                    // https://github.com/microsoft/microsoft-ui-xaml/issues/4990#issuecomment-1181390828
                    XamlRoot = this.XamlRoot
                };

                keydialog.PrimaryButtonClick += async (ContentDialog dialog, ContentDialogButtonClickEventArgs args) =>
                {
                    var diag = dialog as KeyEntryDialog;

                    string provider = diag.APIProvider;
                    string key = diag.Key;

                    try
                    {
                        if (await wm.IsKeyValid(key, provider))
                        {
                            await DispatcherQueue.EnqueueAsync(() =>
                            {
                                SettingsManager.APIKeys[provider] = key;
                                SettingsManager.API = provider;
                                SettingsManager.KeysVerified[provider] = true;
                                SettingsManager.UsePersonalKeys[provider] = true;

                                KeyEntry.Text = key;
                                UpdateKeyBorder();

                                AlertSwitch.IsEnabled = wm.SupportsAlerts;

                                diag.CanClose = true;
                                diag.Hide();
                            });
                        }
                        else
                        {
                            await DispatcherQueue.EnqueueAsync(() =>
                            {
                                diag.CanClose = false;
                            });
                        }
                    }
                    catch (WeatherException ex)
                    {
                        Logger.WriteLine(LoggerLevel.Error, ex, "Settings: KeyEntry: invalid key");
                        await DispatcherQueue.EnqueueAsync(() =>
                        {
                            ShowSnackbar(Snackbar.MakeWarning(ex.Message, SnackbarDuration.Short));
                        });
                    }
                };

                await keydialog.ShowAsync();
            });
        }

        private void UpdateKeyBorder()
        {
            var selectedProvider = APIComboBox.SelectedValue?.ToString();

            if (SettingsManager.KeysVerified[selectedProvider])
                KeyBorder.BorderBrush = new SolidColorBrush(Colors.Green);
            else
                KeyBorder.BorderBrush = new SolidColorBrush(Colors.DarkGray);
        }

        private void APIComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            string selectedProvider = box.SelectedValue?.ToString();

            if (selectedProvider == null) return;

            if (!ExtrasService.IsWeatherAPISupported(selectedProvider))
            {
                var prevItem = e.RemovedItems?.FirstOrDefault() as SimpleWeather.Controls.ProviderEntry;
                // Revert value
                box.SelectedValue = prevItem.Value;
                // show premium popup
                Frame.Navigate(typeof(Extras.Store.PremiumPage));
                return;
            }

            var selectedWProv = wm.GetWeatherProvider(selectedProvider);

            if (selectedWProv.KeyRequired)
            {
                if (String.IsNullOrWhiteSpace(selectedWProv.GetAPIKey()))
                {
                    PersonalKeySwitch.IsOn = SettingsManager.UsePersonalKeys[selectedProvider] = true;
                    PersonalKeySwitch.IsEnabled = Equals(selectedProvider, WeatherAPI.OpenWeatherMap);
                    KeyEntry.Visibility = Visibility.Collapsed;
                }
                else
                {
                    PersonalKeySwitch.IsEnabled = true;
                }

                if (!SettingsManager.UsePersonalKeys[selectedProvider])
                {
                    // We're using our own (verified) keys
                    SettingsManager.KeysVerified[selectedProvider] = true;
                    KeyEntry.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // User is using personal (unverified) keys
                    SettingsManager.KeysVerified[selectedProvider] = false;

                    // Show dialog to set key
                    DispatcherQueue.TryEnqueue(async () => await ShowKeyEntryDialog());

                    KeyEntry.Visibility = Visibility.Visible;
                }

                if (KeyPanel != null)
                    KeyPanel.Visibility = Visibility.Visible;

                if (SettingsManager.KeysVerified[selectedProvider])
                {
                    SettingsManager.API = selectedProvider;
                }
            }
            else
            {
                SettingsManager.KeysVerified[selectedProvider] = false;

                SettingsManager.API = selectedProvider;
                // Clear API KEY entry to avoid issues
                SettingsManager.APIKeys[selectedProvider] = KeyEntry.Text = String.Empty;

                if (KeyPanel != null)
                    KeyPanel.Visibility = Visibility.Collapsed;

                AnalyticsLogger.LogEvent("Update API", new Dictionary<string, string>()
                {
                    { "API", selectedProvider },
                    { "API_IsInternalKey", (!SettingsManager.UsePersonalKeys[selectedProvider]).ToString() }
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

            string API = APIComboBox.SelectedValue.ToString();
            SettingsManager.UsePersonalKeys[API] = sw.IsOn;

            if (!sw.IsOn)
            {
                var selectedWProv = wm.GetWeatherProvider(API);

                if (!selectedWProv.KeyRequired || !String.IsNullOrWhiteSpace(selectedWProv.GetAPIKey()))
                {
                    // We're using our own (verified) keys
                    SettingsManager.KeysVerified[API] = true;
                    SettingsManager.API = API;
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
            string RadarAPI = box?.SelectedValue?.ToString() ?? WeatherAPI.RainViewer;
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
                SettingsManager.RefreshInterval = interval;
            }
            else
            {
                SettingsManager.RefreshInterval = SettingsManager.DefaultInterval;
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
                        await DispatcherQueue.EnqueueAsync(() =>
                        {
                            var snackbar = Snackbar.MakeError(App.Current.ResLoader.GetString("Msg_LocDeniedSettings"), SnackbarDuration.Long);
                            snackbar.SetAction(App.Current.ResLoader.GetString("action_settings"), async () =>
                            {
                                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
                            });
                            ShowSnackbar(snackbar);
                            sw.IsOn = false;
                        });
                        break;

                    case GeolocationAccessStatus.Unspecified:
                        await DispatcherQueue.EnqueueAsync(() =>
                        {
                            ShowSnackbar(Snackbar.MakeError(App.Current.ResLoader.GetString("error_retrieve_location"), SnackbarDuration.Short));
                            sw.IsOn = false;
                        });
                        break;

                    default:
                        break;
                }
            }

            // Update ids when switching GPS feature
            if (await DispatcherQueue.EnqueueAsync(() => sw.IsOn))
            {
                var prevLoc = (await SettingsManager.GetFavorites()).FirstOrDefault();
                if (prevLoc?.query != null)
                {
                    if (SecondaryTileUtils.Exists(prevLoc.query))
                    {
                        var gpsLoc = await SettingsManager.GetLastGPSLocData();
                        if (gpsLoc?.query == null)
                            await SettingsManager.SaveLastGPSLocData(prevLoc);
                        else
                        {
                            SecondaryTileUtils.UpdateTileId(prevLoc.query, Constants.KEY_GPS);
                        }
                    }
                    if (WidgetUtils.Exists(prevLoc.query))
                    {
                        var gpsLoc = await SettingsManager.GetLastGPSLocData();
                        if (gpsLoc?.query == null)
                            await SettingsManager.SaveLastGPSLocData(prevLoc);
                        else
                        {
                            WidgetUtils.UpdateWidgetIds(prevLoc.query, Constants.KEY_GPS);
                        }
                    }
                }
            }
            else
            {
                if (SecondaryTileUtils.Exists(Constants.KEY_GPS))
                {
                    var favLoc = (await SettingsManager.GetFavorites()).FirstOrDefault();
                    if (favLoc?.IsValid() == true)
                        SecondaryTileUtils.UpdateTileId(Constants.KEY_GPS, favLoc.query);
                }
                if (WidgetUtils.Exists(Constants.KEY_GPS))
                {
                    var favLoc = (await SettingsManager.GetFavorites()).FirstOrDefault();
                    if (favLoc?.IsValid() == true)
                        WidgetUtils.UpdateWidgetIds(Constants.KEY_GPS, favLoc);
                }
            }

            await DispatcherQueue.EnqueueAsync((Action)(() =>
            {
                SettingsManager.FollowGPS = sw.IsOn;
            }));
        }

        private void SystemMode_Checked(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("Settings_General: SystemMode_Checked");

            SettingsManager.UserTheme = UserThemeMode.System;
            App.Current.UpdateAppTheme();
            Shell.Instance.UpdateAppTheme();
        }

        private void DarkMode_Checked(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("Settings_General: DarkMode_Checked");

            SettingsManager.UserTheme = UserThemeMode.Dark;
            App.Current.UpdateAppTheme();
            Shell.Instance.UpdateAppTheme();
        }

        private void LightMode_Checked(object sender, RoutedEventArgs e)
        {
            AnalyticsLogger.LogEvent("Settings_General: LightMode_Checked");

            SettingsManager.UserTheme = UserThemeMode.Light;
            App.Current.UpdateAppTheme();
            Shell.Instance.UpdateAppTheme();
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            string requestedLang = box.SelectedValue?.ToString();
            LocaleUtils.SetLocaleCode(requestedLang);

            DispatcherQueue.TryEnqueue(() =>
            {
                App.Current.RefreshAppShell();
            });
        }

        private async void DailyNotifSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch sw = sender as ToggleSwitch;

            if (sw.IsOn)
            {
                if (!await BackgroundTaskHelper.IsBackgroundAccessEnabled().ConfigureAwait(true))
                {
                    var snackbar = Snackbar.MakeError(App.Current.ResLoader.GetString("Msg_BGAccessDeniedSettings"), SnackbarDuration.Long);
                    snackbar.SetAction(App.Current.ResLoader.GetString("action_settings"), async () =>
                    {
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
                    });
                    ShowSnackbar(snackbar);
                    SettingsManager.DailyNotificationEnabled = sw.IsOn = false;
                    return;
                }
            }

            if (sw.IsOn && ExtrasService.IsEnabled())
            {
                SettingsManager.DailyNotificationEnabled = true;
                // Register task
                _ = Task.Run(() => DailyNotificationTask.RegisterBackgroundTask(true));
            }
            else
            {
                if (sw.IsOn && !ExtrasService.IsEnabled())
                {
                    // show premium popup
                    Frame.Navigate(typeof(Extras.Store.PremiumPage));
                }
                SettingsManager.DailyNotificationEnabled = sw.IsOn = false;
                // Unregister task
                _ = Task.Run(() => DailyNotificationTask.UnregisterBackgroundTask());
            }
        }

        private void DailyNotifTimePicker_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
        {
            SettingsManager.DailyNotificationTime = args.NewTime ?? SettingsManager.DEFAULT_DAILYNOTIFICATION_TIME;
            if (SettingsManager.DailyNotificationEnabled)
            {
                Task.Run(async () =>
                {
                    await DailyNotificationTask.RegisterBackgroundTask(true);
                    // Schedule notification anyway as task is not guaranteed
                    await DailyNotificationTask.ScheduleDailyNotification();
                });
            }
        }

        private async void PoPChanceNotifSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch sw = sender as ToggleSwitch;

            if (sw.IsOn)
            {
                if (!await BackgroundTaskHelper.IsBackgroundAccessEnabled().ConfigureAwait(true))
                {
                    var snackbar = Snackbar.MakeError(App.Current.ResLoader.GetString("Msg_BGAccessDeniedSettings"), SnackbarDuration.Long);
                    snackbar.SetAction(App.Current.ResLoader.GetString("action_settings"), async () =>
                    {
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
                    });
                    ShowSnackbar(snackbar);
                    SettingsManager.PoPChanceNotificationEnabled = sw.IsOn = false;
                    return;
                }
            }

            if (sw.IsOn && ExtrasService.IsEnabled())
            {
                SettingsManager.PoPChanceNotificationEnabled = true;
                // Re-register background task if needed
                _ = Task.Run(async () => await WeatherTileUpdaterTask.RegisterBackgroundTask(false));
                _ = Task.Run(async () => await WeatherUpdateBackgroundTask.RegisterBackgroundTask(false));
            }
            else
            {
                if (sw.IsOn && !ExtrasService.IsEnabled())
                {
                    // show premium popup
                    SettingsManager.PoPChanceNotificationEnabled = sw.IsOn = false;
                    Frame.Navigate(typeof(Extras.Store.PremiumPage));
                }
                else
                {
                    SettingsManager.PoPChanceNotificationEnabled = sw.IsOn = false;
                    _ = Task.Run(WeatherTileUpdaterTask.UnregisterBackgroundTask);
                }
            }
        }

        private void PoPChancePct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            string pctStr = box.SelectedValue?.ToString();
            if (int.TryParse(pctStr, out int pct))
            {
                SettingsManager.PoPChanceMinimumPercentage = pct;
            }
        }

        private void MinAlertSeverity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            string value = box.SelectedValue?.ToString();

            if (int.TryParse(value, out int val))
            {
                SettingsManager.MinimumAlertSeverity = (WeatherAlertSeverity)val;
            }
            else
            {
                SettingsManager.MinimumAlertSeverity = WeatherAlertSeverity.Unknown;
            }
        }
    }
}