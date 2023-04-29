using System.Globalization;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
#if IOS || MACCATALYST
using CoreFoundation;
#endif
using SimpleWeather.Controls;
using SimpleWeather.Extras;
using SimpleWeather.Maui.BackgroundTasks;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Extras.Store;
using SimpleWeather.NET.Radar;
using SimpleWeather.Preferences;
using SimpleWeather.RemoteConfig;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using ResStrings = SimpleWeather.Resources.Strings.Resources;
using ResExtras = SimpleWeather.Extras.Resources.Strings.Extras;
using System.Collections.Immutable;

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

    private Cell PremiumPref = null;

    private bool SkipRestore = false;

    public Settings_General()
    {
        InitializeComponent();

        ActionQueue = new HashSet<string>();

        if (ExtrasService.AreSubscriptionsSupported)
        {
            PremiumPref = CreatePremiumPreference();
        }

        RestoreSettings();

        App.Current.Resources.TryGetValue("LightPrimary", out var LightPrimary);
        App.Current.Resources.TryGetValue("DarkPrimary", out var DarkPrimary);
        SettingsTable.UpdateCellColors(
            Colors.Black, Colors.White, Colors.DimGray, Colors.LightGray,
            LightPrimary as Color, DarkPrimary as Color);

        // Event Listeners
        this.SettingsTable.Model.ItemSelected += Model_ItemSelected;
        FollowGPS.OnChanged += FollowGPS_OnChanged;
        PersonalKeyPref.OnChanged += PersonalKeyPref_OnChanged;

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

    private async void RestoreSettings()
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

        await Dispatcher.DispatchAsync(() =>
        {
            if (!ExtrasService.AreSubscriptionsSupported)
            {
                PremiumPref?.Let(AboutSection.Remove);
            }
            else if (PremiumPref != null && !AboutSection.Contains(PremiumPref))
            {
                AboutSection.Add(PremiumPref);
            }
        });
    }

    private void RestoreAPISettings()
    {
        RestoreAPISettings(SettingsManager.API);
    }

    private async void RestoreAPISettings(string selectedProvider)
    {
        // Weather Providers
        APIPref.Items = WeatherAPI.APIs.Select(entry => new PreferenceListItem(entry.Display, entry.Value));
        APIPref.SelectedItem = selectedProvider;
        APIPref.Detail = APIPref.Items.OfType<PreferenceListItem>().First(it => Equals(it.Value, APIPref.SelectedItem)).Display;

        var selectedWProv = wm.GetWeatherProvider(selectedProvider);

        if (selectedWProv.KeyRequired)
        {
            KeyEntryPref.IsEnabled = true;

            if (!string.IsNullOrWhiteSpace(SettingsManager.APIKey) && !SettingsManager.KeysVerified[selectedProvider])
                SettingsManager.KeysVerified[selectedProvider] = true;

            if (string.IsNullOrWhiteSpace(selectedWProv.GetAPIKey()))
            {
                PersonalKeyPref.On = SettingsManager.UsePersonalKeys[selectedProvider] = true;
                PersonalKeyPref.IsEnabled = false;
                KeyEntryPref.IsEnabled = false;
                await Dispatcher.DispatchAsync(() =>
                {
                    ProviderSection.Remove(KeyEntryPref);
                    ProviderSection.Remove(APIRegisterPref);
                });
            }
            else
            {
                PersonalKeyPref.IsEnabled = true;
                PersonalKeyPref.On = SettingsManager.UsePersonalKeys[selectedProvider];
            }

            if (!SettingsManager.UsePersonalKeys[selectedProvider])
            {
                // We're using our own (verified) keys
                SettingsManager.KeysVerified[selectedProvider] = true;
                KeyEntryPref.IsEnabled = false;
                await Dispatcher.DispatchAsync(() =>
                {
                    ProviderSection.Remove(KeyEntryPref);
                    ProviderSection.Remove(APIRegisterPref);
                });
            }
            else
            {
                KeyEntryPref.IsEnabled = true;

                await Dispatcher.DispatchAsync(() =>
                {
                    if (!ProviderSection.Contains(APIRegisterPref))
                        ProviderSection.Add(APIRegisterPref);
                    if (!ProviderSection.Contains(KeyEntryPref))
                        ProviderSection.Add(KeyEntryPref);
                });
            }
        }
        else
        {
            KeyEntryPref.IsEnabled = false;
            PersonalKeyPref.IsEnabled = false;
            await Dispatcher.DispatchAsync(() =>
            {
                ProviderSection.Remove(PersonalKeyPref);
                ProviderSection.Remove(KeyEntryPref);
                ProviderSection.Remove(APIRegisterPref);
            });
            SettingsManager.KeysVerified[selectedProvider] = false;
            // Clear API KEY entry to avoid issues
            SettingsManager.APIKey = string.Empty;
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

        if (!SkipRestore)
        {
            RestoreSettings();
        }
        SkipRestore = false;
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
        AnalyticsLogger.LogEvent("Settings_General: OnNavigatedFrom");
        UnloadSnackManager();

        var currentLocation = Shell.Current.CurrentState.Location?.ToString();

        if (currentLocation?.ToLowerInvariant()?.Contains("dialog") != true && IsProviderAndKeyInvalid())
        {
            var API = RemoteConfigService.GetDefaultWeatherProvider();
            APIPref.SelectedItem = API;
            RestoreAPISettings(API);
        }
    }

    private bool IsProviderAndKeyInvalid()
    {
        var provider = APIPref.SelectedItem.ToString();
        return SettingsManager.UsePersonalKeys[provider] &&
            String.IsNullOrWhiteSpace(SettingsManager.APIKeys[provider]) &&
            wm.IsKeyRequired(provider);
    }

    public Task<bool> OnBackRequested()
    {
        if (IsProviderAndKeyInvalid())
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
                    var api = SettingsManager.API;
                    // Log event
                    AnalyticsLogger.LogEvent("Update API", new Dictionary<string, string>()
                    {
                        { "API", api },
                        { "API_IsInternalKey", (!SettingsManager.UsePersonalKeys[api]).ToString() }
                    });
#if __IOS__
                    WeatherUpdaterTask.UpdateWeather();
#endif
                    break;
                case CommonActions.ACTION_WEATHER_REREGISTERTASK:
#if __IOS__
                    UpdaterTaskUtils.UpdateTasks();
#endif
                    break;
                case CommonActions.ACTION_WEATHER_UPDATE:
#if __IOS__
                    WeatherUpdaterTask.UpdateWeather();
#endif
                    break;
            }
        }

        ActionQueue.Clear();
    }

    private void UpdateKeySummary()
    {
        UpdateKeySummary(APIPref.Detail?.ToString());
    }

    private void UpdateKeySummary(string providerAPI)
    {
        var value = APIPref.SelectedItem?.ToString();
        if (!string.IsNullOrWhiteSpace(SettingsManager.APIKeys[value]))
        {
            var keyVerified = SettingsManager.KeysVerified[value];
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
        SkipRestore = false;

        switch (message.Value.PreferenceKey)
        {
            case SettingsManager.KEY_REFRESHINTERVAL:
                SettingsManager.RefreshInterval = int.Parse(message.Value.NewValue.ToString());
#if __IOS__
                WeatherUpdaterTask.ScheduleTask();
#endif
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
                    SkipRestore = true;
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
    }

    private async void OnWeatherProviderChanged(object newValue)
    {
        string selectedProvider = newValue?.ToString();

        if (selectedProvider == null) return;

        if (!ExtrasService.IsWeatherAPISupported(selectedProvider))
        {
            await this.Navigation.PushAsync(new PremiumPage());
            return;
        }

        APIPref.SelectedItem = selectedProvider;
        APIPref.Detail = APIPref.Items.OfType<PreferenceListItem>().First(it => Equals(it.Value, APIPref.SelectedItem)).Display;

        var selectedWProv = wm.GetWeatherProvider(selectedProvider);

        if (selectedWProv.KeyRequired)
        {
            if (string.IsNullOrWhiteSpace(selectedWProv.GetAPIKey()))
            {
                PersonalKeyPref.On = SettingsManager.UsePersonalKeys[selectedProvider] = true;
                PersonalKeyPref.IsEnabled = Equals(selectedProvider, WeatherAPI.OpenWeatherMap);
                KeyEntryPref.IsEnabled = false;
                await Dispatcher.DispatchAsync(() =>
                {
                    ProviderSection.Remove(KeyEntryPref);
                    ProviderSection.Remove(APIRegisterPref);
                });
            }
            else
            {
                PersonalKeyPref.IsEnabled = true;
                PersonalKeyPref.On = SettingsManager.UsePersonalKeys[selectedProvider];
            }

            if (!SettingsManager.UsePersonalKeys[selectedProvider])
            {
                // We're using our own (verified) keys
                SettingsManager.KeysVerified[selectedProvider] = true;
                KeyEntryPref.IsEnabled = false;
                await Dispatcher.DispatchAsync(() =>
                {
                    ProviderSection.Remove(KeyEntryPref);
                    ProviderSection.Remove(APIRegisterPref);
                });
            }
            else
            {
                // User is using personal (unverified) keys
                SettingsManager.KeysVerified[selectedProvider] = false;

                // Show dialog to set key
                if (string.IsNullOrWhiteSpace(selectedWProv.GetAPIKey()))
                    ShowKeyEntryPopup();

                KeyEntryPref.IsEnabled = true;

                await Dispatcher.DispatchAsync(() =>
                {
                    if (!ProviderSection.Contains(APIRegisterPref))
                        ProviderSection.Add(APIRegisterPref);
                    if (!ProviderSection.Contains(KeyEntryPref))
                        ProviderSection.Add(KeyEntryPref);
                });
            }

            await Dispatcher.DispatchAsync(() =>
            {
                if (!ProviderSection.Contains(PersonalKeyPref))
                {
                    ProviderSection.Insert(1, PersonalKeyPref);
                }
            });

            if (SettingsManager.KeysVerified[selectedProvider])
            {
                SettingsManager.API = selectedProvider;
            }
        }
        else
        {
            SettingsManager.KeysVerified[selectedProvider] = false;
            KeyEntryPref.IsEnabled = false;
            PersonalKeyPref.IsEnabled = false;

            SettingsManager.API = selectedProvider;
            // Clear API KEY entry to avoid issues
            SettingsManager.APIKeys[selectedProvider] = KeyEntryPref.Detail = String.Empty;

            await Dispatcher.DispatchAsync(() =>
            {
                ProviderSection.Remove(PersonalKeyPref);
                ProviderSection.Remove(KeyEntryPref);
                ProviderSection.Remove(APIRegisterPref);
            });

            // Log event
            AnalyticsLogger.LogEvent("Update API", new Dictionary<string, string>()
            {
                { "API", selectedProvider },
                { "API_IsInternalKey", (!SettingsManager.UsePersonalKeys[selectedProvider]).ToString() }
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
                        ShowSnackbar(snackbar);
                    }
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        sw.On = true;
                    });
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

        // Update any widgets

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            SettingsManager.FollowGPS = sw.On;
        });
    }

    private async void PersonalKeyPref_OnChanged(object sender, ToggledEventArgs e)
    {
        AnalyticsLogger.LogEvent("Settings_General: PersonalKeySwitch_Toggled");

        SwitchCell sw = sender as SwitchCell;

        string API = APIPref.SelectedItem?.ToString();
        SettingsManager.UsePersonalKeys[API] = e.Value;

        if (e.Value)
        {
            await Dispatcher.DispatchAsync(() =>
            {
                if (!ProviderSection.Contains(APIRegisterPref))
                    ProviderSection.Add(APIRegisterPref);
                if (!ProviderSection.Contains(KeyEntryPref))
                    ProviderSection.Add(KeyEntryPref);
            });

            KeyEntryPref.IsEnabled = true;
        }
        else
        {
            var selectedWProv = wm.GetWeatherProvider(API);

            if (!selectedWProv.KeyRequired || !String.IsNullOrWhiteSpace(selectedWProv.GetAPIKey()))
            {
                // We're using our own (verified) keys
                SettingsManager.KeysVerified[API] = true;
                SettingsManager.API = API;
            }

            KeyEntryPref.IsEnabled = false;
            await Dispatcher.DispatchAsync(() =>
            {
                ProviderSection.Remove(KeyEntryPref);
                ProviderSection.Remove(APIRegisterPref);
            });
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
            else if (pageType == typeof(PremiumPage))
            {
                if (ExtrasService.AreSubscriptionsSupported)
                {
                    await Navigation.PushAsync(new PremiumPage());
                }
                else
                {
                    ShowSnackbar(Snackbar.Make(ResExtras.message_premium_required, SnackbarDuration.Short));
                }
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
            keyPopup.Close(false);
        };

        this.ShowPopup(keyPopup);
    }

    private Cell CreatePremiumPreference()
    {
        return new TextCell()
        {
            CommandParameter = typeof(PremiumPage),
            Text = ResExtras.pref_title_premium,
            Detail = ResExtras.message_premium_prompt
        }.Apply(it =>
        {
            it.Tapped += TextCell_Tapped;
        });
    }
}