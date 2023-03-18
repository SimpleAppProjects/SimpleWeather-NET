using System.Diagnostics;
using System.Globalization;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
#if IOS || MACCATALYST
using CoreFoundation;
#endif
using SimpleWeather.Controls;
using SimpleWeather.Extras;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Radar;
using SimpleWeather.NET.Radar.RainViewer;
using SimpleWeather.Preferences;
using SimpleWeather.RemoteConfig;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Preferences;

public partial class Settings_General : ContentPage, IBackRequestedPage, ISnackbarManager, IRecipient<SettingsChangedMessage>
{
    private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();
    private readonly IRemoteConfigService RemoteConfigService = Ioc.Default.GetService<IRemoteConfigService>();

    private SnackbarManager SnackMgr;

    private readonly HashSet<String> ActionQueue;

    private readonly IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();

    private readonly IEnumerable<PreferenceListItem> RefreshOptions = new List<PreferenceListItem>
    {
        new PreferenceListItem(ResStrings.refresh_60min, "60"),
        new PreferenceListItem(ResStrings.refresh_2hrs, "120"),
        new PreferenceListItem(ResStrings.refresh_3hrs, "180"),
        new PreferenceListItem(ResStrings.refresh_6hrs, "360"),
        new PreferenceListItem(ResStrings.refresh_12hrs, "720"),
    };

    private readonly IEnumerable<PreferenceListItem> PremiumRefreshOptions = new List<PreferenceListItem>
    {
        new PreferenceListItem(ResStrings.refresh_30min, "30"),
        new PreferenceListItem(ResStrings.refresh_60min, "60"),
        new PreferenceListItem(ResStrings.refresh_2hrs, "120"),
        new PreferenceListItem(ResStrings.refresh_3hrs, "180"),
        new PreferenceListItem(ResStrings.refresh_6hrs, "360"),
        new PreferenceListItem(ResStrings.refresh_12hrs, "720"),
    };

    public Settings_General()
    {
        InitializeComponent();

        ActionQueue = new HashSet<string>();

        RestoreSettings();

        App.Current.Resources.TryGetValue("LightPrimary", out var LightPrimary);
        App.Current.Resources.TryGetValue("DarkPrimary", out var DarkPrimary);
        SettingsTable.UpdateCellColors(
            Colors.Black, Colors.White, Colors.DimGray, Colors.LightGray,
            LightPrimary as Color, DarkPrimary as Color);

        // Event Listeners
        this.SettingsTable.Model.ItemSelected += Model_ItemSelected;
        FollowGPS.OnChanged += FollowGPS_OnChanged;

        AnalyticsLogger.LogEvent("Settings_General");
        WeakReferenceMessenger.Default.Register(this);
    }

    public void InitSnackManager()
    {
        if (SnackMgr == null)
        {
            SnackMgr = new SnackbarManager(Content);
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
        FollowGPS.On = SettingsManager.FollowGPS;

        // Weather Providers
        RestoreAPISettings();

        // Refresh interval
        if (ExtrasService.IsEnabled())
        {
            IntervalPref.Items = PremiumRefreshOptions;
        }
        else
        {
            IntervalPref.Items = RefreshOptions;
        }

        // Update Interval
        switch (SettingsManager.RefreshInterval)
        {
            case 30:
                IntervalPref.SelectedItem = "30";
                break;

            case 60:
                IntervalPref.SelectedItem = "60";
                break;

            case 120:
                IntervalPref.SelectedItem = "120";
                break;

            case 180:
                IntervalPref.SelectedItem = "180";
                break;

            case 360:
                IntervalPref.SelectedItem = "360";
                break;

            case 720:
                IntervalPref.SelectedItem = "720";
                break;

            default:
                IntervalPref.SelectedItem = SettingsManager.DefaultInterval.ToInvariantString();
                break;
        }
        IntervalPref.Detail = IntervalPref.Items.OfType<PreferenceListItem>().First(it => Equals(it.Value, IntervalPref.SelectedItem)).Display;

        // Radar
        RadarPref.Items = RadarProvider.GetRadarProviders().Select(entry => new PreferenceListItem(entry.Display, entry.Value));
        RadarPref.SelectedItem = RadarProvider.GetRadarProvider();
        RadarPref.Detail = RadarPref.Items.OfType<PreferenceListItem>().First(it => Equals(it.Value, RadarPref.SelectedItem)).Display;

        ThemePref.SelectedItem = SettingsManager.UserTheme;
        ThemePref.Detail = ThemePref.Items.OfType<PreferenceListItem>().First(it => Equals(it.Value, ThemePref.SelectedItem)).Display;

        // Language
        LanguagePref.Items.OfType<PreferenceListItem>().ForEach(it =>
        {
            var code = it.Value.ToString();

            if (string.IsNullOrWhiteSpace(code))
            {
                it.Display = ResStrings.summary_default;
            }
            else
            {
                var culture = CultureInfo.GetCultureInfo(code, true);
                it.Display = culture.GetNativeDisplayName(culture);
                it.Detail = culture.GetNativeDisplayName();
            }
        });
        LanguagePref.SelectedItem = LocaleUtils.GetLocaleCode();
        LanguagePref.Detail = string.IsNullOrWhiteSpace(LanguagePref.SelectedItem?.ToString()) ? ResStrings.summary_default : LocaleUtils.GetLocaleDisplayName();
    }

    private void RestoreAPISettings()
    {
        // Weather Providers
        APIPref.Items = WeatherAPI.APIs.Select(entry => new PreferenceListItem(entry.Display, entry.Value));
        var selectedProvider = SettingsManager.API;
        APIPref.SelectedItem = selectedProvider;
        APIPref.Detail = APIPref.Items.OfType<PreferenceListItem>().First(it => Equals(it.Value, APIPref.SelectedItem)).Display;

        var selectedWProv = wm.GetWeatherProvider(selectedProvider);

        if (selectedWProv.KeyRequired)
        {
            if (!string.IsNullOrWhiteSpace(SettingsManager.APIKey) && !SettingsManager.KeysVerified[selectedProvider])
                SettingsManager.KeysVerified[selectedProvider] = true;

            PersonalKeyPref.On = SettingsManager.UsePersonalKey;

            ShowKeyPreferences(true);

            if (string.IsNullOrWhiteSpace(selectedWProv.GetAPIKey()))
            {
                PersonalKeyPref.On = SettingsManager.UsePersonalKey = true;
                PersonalKeyPref.IsEnabled = false;
                KeyEntryPref.IsEnabled = false;
            }
            else
            {
                PersonalKeyPref.IsEnabled = true;
            }

            if (!SettingsManager.UsePersonalKey)
            {
                // We're using our own (verified) keys
                SettingsManager.KeysVerified[selectedProvider] = true;
                KeyEntryPref.IsEnabled = false;
            }
            else
            {
                KeyEntryPref.IsEnabled = true;
            }
        }
        else
        {
            SettingsManager.KeysVerified[selectedProvider] = false;
            // Clear API KEY entry to avoid issues
            SettingsManager.APIKey = string.Empty;
            ShowKeyPreferences(false);
        }

        KeyEntryPref.Detail = SettingsManager.APIKey;
        UpdateKeySummary();
        UpdateRegisterLink();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        AnalyticsLogger.LogEvent("Settings_General: OnNavigatedToPage");
        InitSnackManager();
        App.Current.UnregisterSettingsListener();
        SettingsManager.OnSettingsChanged += Settings_OnSettingsChanged;
        RestoreSettings();
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
        // Unsubscribe from event
        App.Current.RegisterSettingsListener();
        SettingsManager.OnSettingsChanged -= Settings_OnSettingsChanged;

        ProcessQueue();
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        UnloadSnackManager();
    }

    public Task<bool> OnBackRequested()
    {
        var provider = APIPref.SelectedItem.ToString();

        if (SettingsManager.UsePersonalKey && String.IsNullOrWhiteSpace(SettingsManager.APIKeys[provider]) &&
            wm.IsKeyRequired(provider))
        {
            UpdateKeySummary();
            ShowSnackbar(Snackbar.MakeWarning(ResStrings.message_enter_apikey, SnackbarDuration.Long));
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
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
                EnqueueAction(CommonActions.ACTION_WEATHER_UPDATE);
                break;
            case SettingsManager.KEY_REFRESHINTERVAL:
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
                            { "API", SettingsManager.API },
                            { "API_IsInternalKey", (!SettingsManager.UsePersonalKey).ToString() }
                        });
                    // TODO: trigger task
                    break;
                case CommonActions.ACTION_WEATHER_REREGISTERTASK:
                    // TODO: trigger task
                    break;
                case CommonActions.ACTION_WEATHER_UPDATE:
                    // TODO: trigger task
                    break;
            }
        }

        ActionQueue.Clear();
    }

    private void ShowKeyPreferences(bool show)
    {
        PersonalKeyPref.IsEnabled = APIRegisterPref.IsEnabled = KeyEntryPref.IsEnabled = show;
        if (!show) PersonalKeyPref.On = false;
    }

    private void UpdateKeySummary()
    {
        UpdateKeySummary(APIPref.SelectedItem?.ToString());
    }

    private void UpdateKeySummary(string providerAPI)
    {
        if (!string.IsNullOrWhiteSpace(SettingsManager.APIKeys[providerAPI]))
        {
            var keyVerified = SettingsManager.KeysVerified[providerAPI];
            var color = keyVerified ? Colors.Green : Colors.Red;
            var summary = keyVerified ? "Key Verified" : "Key Invalid";
            KeyEntryPref.Detail = summary;
            KeyEntryPref.DetailColor = color;
        }
        else
        {
            KeyEntryPref.Detail = ResStrings.pref_summary_apikey.ReplaceFirst("%1$s", providerAPI);
            KeyEntryPref.SetAppThemeColor(TextCell.DetailColorProperty, Colors.DarkGray, Colors.White);
        }
    }

    private void UpdateRegisterLink()
    {
        UpdateRegisterLink(APIPref.SelectedItem?.ToString());
    }

    private void UpdateRegisterLink(string providerAPI)
    {
        ProviderEntry prov = null;
        foreach (var provider in WeatherAPI.APIs)
        {
            if (Equals(provider.Value, providerAPI))
            {
                prov = provider;
                break;
            }
        }

        if (!string.IsNullOrWhiteSpace(prov?.APIRegisterURL))
        {
            APIRegisterPref.CommandParameter = new Uri(prov.APIRegisterURL);
        }
        else
        {
            APIRegisterPref.CommandParameter = null;
        }
    }

    private async void Model_ItemSelected(object sender, Microsoft.Maui.Controls.Internals.EventArg<object> e)
    {
        if (e.Data is ListViewCell listPref)
        {
            await Navigation.PushAsync(new PreferenceListDialogPage(listPref));
        }
    }

    public void Receive(SettingsChangedMessage message)
    {
        switch (message.Value.PreferenceKey)
        {
            case SettingsManager.KEY_REFRESHINTERVAL:
                SettingsManager.RefreshInterval = int.Parse(message.Value.NewValue.ToString());
                break;
            case SettingsManager.KEY_USERTHEME:
                {
                    var userTheme = (UserThemeMode)message.Value.NewValue;
                    SettingsManager.UserTheme = (UserThemeMode)message.Value.NewValue;

                    switch (userTheme)
                    {
                        case UserThemeMode.Light:
                            App.Current.UserAppTheme = AppTheme.Light;
                            break;
                        case UserThemeMode.Dark:
                            App.Current.UserAppTheme = AppTheme.Dark;
                            break;
                        case UserThemeMode.System:
                            {
#if __IOS__
                                App.Current.UserAppTheme = App.Current.IsSystemDarkTheme ? AppTheme.Dark : AppTheme.Light;
#else
                                App.Current.UserAppTheme = AppTheme.Unspecified;
#endif
                                App.Current.UpdateAppTheme();
                            }
                            break;
                    }

                    App.Current.UpdateAppTheme();
                }
                break;
            case SettingsManager.KEY_API:
                {
                    OnWeatherProviderChanged(message.Value.NewValue);
                }
                break;
            case SettingsManager.KEY_APIKEY:
                break;
            case RadarProvider.KEY_RADARPROVIDER:
                {
                    string RadarAPI = message.Value.NewValue.ToString();
                    var radarProviderValues = Enum.GetValues(typeof(RadarProvider.RadarProviders));
                    RadarProvider.RadarAPIProvider = radarProviderValues
                        .Cast<RadarProvider.RadarProviders>()
                        .FirstOrDefault(@enum => Equals(@enum.GetStringValue(), RadarAPI));
                }
                break;
            case LocaleUtils.KEY_LANGUAGE:
                {
                    // Copy navigation stack and skip last page (dialog)
                    var navigationStack = App.Current.Navigation.NavigationStack.Where(p => p != null).SkipLast(1).ToList();

                    var requestedLang = message.Value.NewValue.ToString();
                    LocaleUtils.SetLocaleCode(requestedLang);

                    Dispatcher.Dispatch(async () =>
                    {
                        // Note: Workaround: destroy MapControl before replacing shell
                        MapControlCreator.Instance?.RemoveMapControl();

                        // Reset shell + navigation stack
                        App.Current.MainPage = new Main.AppShell();

                        foreach (var page in navigationStack)
                        {
                            var instance = ActivatorUtilities.CreateInstance(Ioc.Default, page.GetType()) as Page;

                            await App.Current.Navigation.PushAsync(instance);
                        }
                    });
                }
                break;
        }

        RestoreSettings();
    }

    private void OnWeatherProviderChanged(object newValue)
    {
        string selectedProvider = newValue?.ToString();

        if (selectedProvider == null) return;

        if (!ExtrasService.IsWeatherAPISupported(selectedProvider))
        {
            // TODO: show premium popup
            return;
        }

        APIPref.SelectedItem = selectedProvider;
        APIPref.Detail = APIPref.Items.OfType<PreferenceListItem>().First(it => Equals(it.Value, APIPref.SelectedItem)).Display;

        var selectedWProv = wm.GetWeatherProvider(selectedProvider);

        if (selectedWProv.KeyRequired)
        {
            ShowKeyPreferences(true);

            if (string.IsNullOrWhiteSpace(selectedWProv.GetAPIKey()))
            {
                PersonalKeyPref.On = SettingsManager.UsePersonalKey = true;
                PersonalKeyPref.IsEnabled = Equals(selectedProvider, WeatherAPI.OpenWeatherMap);
                KeyEntryPref.IsEnabled = false;
            }
            else
            {
                PersonalKeyPref.IsEnabled = true;
            }

            if (!SettingsManager.UsePersonalKey)
            {
                // We're using our own (verified) keys
                SettingsManager.KeysVerified[selectedProvider] = true;
                KeyEntryPref.IsEnabled = false;
            }
            else
            {
                // User is using personal (unverified) keys
                SettingsManager.KeysVerified[selectedProvider] = false;

                // Show dialog to set key
                ShowKeyEntryPopup();

                KeyEntryPref.IsEnabled = true;
            }

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
            SettingsManager.APIKeys[selectedProvider] = KeyEntryPref.Detail = String.Empty;

            ShowKeyPreferences(false);

            AnalyticsLogger.LogEvent("Update API", new Dictionary<string, string>()
            {
                { "API", SettingsManager.API },
                { "API_IsInternalKey", (!SettingsManager.UsePersonalKey).ToString() }
            });
        }

        wm.UpdateAPI();
        UpdateKeySummary();
        UpdateRegisterLink();
    }

    private async void FollowGPS_OnChanged(object sender, ToggledEventArgs e)
    {
        AnalyticsLogger.LogEvent("Settings_General: FollowGPS_Toggled");

        SwitchCell sw = sender as SwitchCell;

        if (e.Value)
        {
            var locationStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            switch (locationStatus)
            {
                case PermissionStatus.Granted:
                    if (await Permissions.CheckStatusAsync<Permissions.LocationAlways>() != PermissionStatus.Granted)
                    {
                        var snackbar = Snackbar.Make(ResStrings.Msg_BGAccessDeniedSettings, SnackbarDuration.Long);
                        snackbar.SetAction(ResStrings.ConfirmDialog_PrimaryButtonText, async () =>
                        {
                            await Permissions.RequestAsync<Permissions.LocationAlways>();
                        });
                    }
                    break;
                case PermissionStatus.Denied:
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        var snackbar = Snackbar.MakeError(ResStrings.Msg_LocDeniedSettings, SnackbarDuration.Long);
                        snackbar.SetAction(ResStrings.action_settings, () =>
                        {
                            AppInfo.ShowSettingsUI();
                        });
                        ShowSnackbar(snackbar);
                        sw.On = false;
                    });
                    break;
                default:
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        ShowSnackbar(Snackbar.MakeError(ResStrings.error_retrieve_location, SnackbarDuration.Short));
                        sw.On = false;
                    });
                    break;
            }
        }
    }

    private async void TextCell_Tapped(object sender, EventArgs e)
    {
        var cell = sender as TextCell;
        if (cell.CommandParameter is Type pageType)
        {
            if (pageType == typeof(Settings_Units))
            {
                await Navigation.PushAsync(new Settings_Units());
            }
            else if (pageType == typeof(Settings_Features))
            {
                await Navigation.PushAsync(new Settings_Features());
            }
            else if (pageType == typeof(Settings_Icons))
            {
                await Navigation.PushAsync(new Settings_Icons());
            }
            else if (pageType == typeof(Settings_WeatherNotifications))
            {
                await Navigation.PushAsync(new Settings_WeatherNotifications());
            }
            else if (pageType == typeof(Settings_WeatherAlerts))
            {
                await Navigation.PushAsync(new Settings_WeatherAlerts());
            }
            else if (pageType == typeof(Settings_About))
            {
                await Navigation.PushAsync(new Settings_About());
            }
        }
        else if (cell.CommandParameter is Uri uri)
        {
            this.RunCatching(async () =>
            {
                return await Browser.Default.OpenAsync(uri);
            });
        }
        else if (cell is DialogCell dialogCell && Equals(SettingsManager.KEY_APIKEY, dialogCell.PreferenceKey))
        {
            ShowKeyEntryPopup();
        }
    }

    private void ShowKeyEntryPopup()
    {
        var keyPopup = new KeyEntryPopup(APIPref.SelectedItem?.ToString());

        var deviceDisplay = DeviceDisplay.Current;
        keyPopup.Size = new Size(
            width: Math.Min(420, 0.7 * (deviceDisplay.MainDisplayInfo.Width / deviceDisplay.MainDisplayInfo.Density)),
            height: Math.Min(360, deviceDisplay.MainDisplayInfo.Height / deviceDisplay.MainDisplayInfo.Density)
        );

        keyPopup.PrimaryButtonClick += async (s, e) =>
        {
            var popup = s as KeyEntryPopup;

            string provider = popup.APIProvider;
            string key = popup.Key;

            try
            {
                if (await wm.IsKeyValid(key, provider))
                {
                    await Dispatcher.DispatchAsync(() =>
                    {
                        SettingsManager.APIKeys[provider] = key;
                        SettingsManager.API = provider;
                        SettingsManager.KeysVerified[provider] = true;

                        RestoreAPISettings();

                        popup.Close();
                    });
                }
                else
                {
                    ShowSnackbar(Snackbar.MakeWarning(ResStrings.werror_invalidkey, SnackbarDuration.Short));
                }
            }
            catch (WeatherException ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "Settings: KeyEntry: invalid key");
                await Dispatcher.DispatchAsync(() =>
                {
                    ShowSnackbar(Snackbar.MakeWarning(ex.Message, SnackbarDuration.Short));
                });
            }
        };

        keyPopup.SecondaryButtonClick += (s, e) =>
        {
            // Restore state from settings
            RestoreAPISettings();
            keyPopup.Close(false);
        };

        this.ShowPopup(keyPopup);
    }
}