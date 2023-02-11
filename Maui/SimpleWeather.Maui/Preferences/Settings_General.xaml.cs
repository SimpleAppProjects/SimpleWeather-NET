using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using SimpleWeather.Extras;
using SimpleWeather.Maui.Controls;
using SimpleWeather.NET.Controls;
using SimpleWeather.Preferences;
using SimpleWeather.RemoteConfig;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Preferences;

public partial class Settings_General : ContentPage, ISnackbarManager, IRecipient<SettingsChangedMessage>
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

        // Event Listeners
        this.SettingsTable.Model.ItemSelected += Model_ItemSelected;
        FollowGPS.OnChanged += FollowGPS_OnChanged;

        AnalyticsLogger.LogEvent("Settings_General");
        WeakReferenceMessenger.Default.Register(this);
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
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

        ThemePref.SelectedItem = SettingsManager.UserTheme;
        ThemePref.Detail = ThemePref.Items.OfType<PreferenceListItem>().First(it => Equals(it.Value, ThemePref.SelectedItem)).Display;
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
                            App.Current.UserAppTheme = AppTheme.Unspecified;
                            break;
                    }
                }
                break;
        }

        RestoreSettings();
    }

    private async void FollowGPS_OnChanged(object sender, ToggledEventArgs e)
    {
        AnalyticsLogger.LogEvent("Settings_General: FollowGPS_Toggled");

        SwitchCell sw = sender as SwitchCell;

        if (sw.On)
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
            else if (pageType == typeof(Settings_Icons)) // notif
            {
                await Navigation.PushAsync(new Settings_Icons());
            }
            else if (pageType == typeof(Settings_Icons)) // alerts
            {
                await Navigation.PushAsync(new Settings_Icons());
            }
            else if (pageType == typeof(Settings_About))
            {
                await Navigation.PushAsync(new Settings_About());
            }
        }
    }
}