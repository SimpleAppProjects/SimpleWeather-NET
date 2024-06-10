using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
#if __IOS__
#endif
using SimpleWeather.Common.Helpers;
using SimpleWeather.Extras;
using SimpleWeather.Maui.BackgroundTasks;
using SimpleWeather.Maui.Controls;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Extras.Store;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Preferences;

public partial class Settings_WeatherAlerts : ContentPage, ISnackbarManager, IRecipient<SettingsChangedMessage>
{
    private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    private SnackbarManager SnackMgr;

    private readonly IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();

    public Settings_WeatherAlerts()
    {
        InitializeComponent();

        RestoreSettings();

        App.Current.Resources.TryGetValue("LightPrimary", out var LightPrimary);
        App.Current.Resources.TryGetValue("DarkPrimary", out var DarkPrimary);
        SettingsTable.UpdateCellColors(
            Colors.Black, Colors.White, Color.Parse("#767676"), Color.Parse("#a2a2a2"),
            LightPrimary as Color, DarkPrimary as Color);

        // Event Listeners
        this.SettingsTable.Model.ItemSelected += Model_ItemSelected;
        AlertPref.OnChanged += AlertPref_OnChanged;
        PoPChancePref.OnChanged += PoPChancePref_OnChanged;

        WeakReferenceMessenger.Default.Register(this);
    }

    private void RestoreSettings()
    {
        // Daily Notification
        AlertPref.IsEnabled = wm.SupportsAlerts;
        AlertPref.On = SettingsManager.ShowAlerts;

        MinAlertSeverityPref.SelectedItem = ((int)SettingsManager.MinimumAlertSeverity).ToInvariantString();
        MinAlertSeverityPref.Detail = MinAlertSeverityPref.Items.OfType<PreferenceListItem>().First(it => Equals(it.Value, MinAlertSeverityPref.SelectedItem)).Display;

        PoPChancePref.On = SettingsManager.PoPChanceNotificationEnabled;
        PoPChancePctPref.SelectedItem = SettingsManager.PoPChanceMinimumPercentage.ToInvariantString();
        PoPChancePctPref.Detail = PoPChancePctPref.Items.OfType<PreferenceListItem>().First(it => Equals(it.Value, PoPChancePctPref.SelectedItem)).Display;
    }

    private async void Model_ItemSelected(object sender, Microsoft.Maui.Controls.Internals.EventArg<object> e)
    {
        if (e.Data is ListViewCell listPref)
        {
            await Navigation.PushAsync(new PreferenceListDialogPage(listPref));
        }
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

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        InitSnackManager();
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        UnloadSnackManager();
    }

    public void Receive(SettingsChangedMessage message)
    {
        switch (message.Value.PreferenceKey)
        {
            case SettingsManager.KEY_POPCHANCEPCT:
                {
                    if (int.TryParse(message.Value.NewValue.ToString(), out int pct))
                    {
                        SettingsManager.PoPChanceMinimumPercentage = pct;
                    }
                }
                break;
            case SettingsManager.KEY_MINALERTSEVERITY:
                {
                    if (int.TryParse(message.Value.NewValue.ToString(), out int val))
                    {
                        SettingsManager.MinimumAlertSeverity = (WeatherAlertSeverity)val;
                    }
                }
                break;
        }

        RestoreSettings();
    }

    private async void AlertPref_OnChanged(object sender, ToggledEventArgs e)
    {
        SwitchCell sw = sender as SwitchCell;

        if (sw.On)
        {
            if (!await NotificationPermissionRequestHelper.NotificationPermissionEnabled(false))
            {
                sw.On = false;
                var result = await NotificationPermissionRequestHelper.RequestNotificationPermission(false);

                if (!result)
                {
                    return;
                }

                sw.On = result;
            }

#if __IOS__
            UpdaterTaskUtils.StartTasks();
#endif
        }
        else
        {
#if __IOS__
            UpdaterTaskUtils.CancelTasks();
#endif
        }

        SettingsManager.ShowAlerts = sw.On;
    }

    private async void PoPChancePref_OnChanged(object sender, ToggledEventArgs e)
    {
        SwitchCell sw = sender as SwitchCell;

        if (sw.On)
        {
#if __IOS__
            if (UIKit.UIApplication.SharedApplication.BackgroundRefreshStatus == UIKit.UIBackgroundRefreshStatus.Denied)
            {
                var snackbar = Snackbar.MakeError(ResStrings.Msg_BGAccessDeniedSettings, SnackbarDuration.Long);
                snackbar.SetAction(ResStrings.action_settings, () =>
                {
                    AppInfo.ShowSettingsUI();
                });
                ShowSnackbar(snackbar);
                SettingsManager.DailyNotificationEnabled = sw.On = false;
                return;
            }
#endif
        }

        if (sw.On && !ExtrasService.IsEnabled())
        {
            SettingsManager.PoPChanceNotificationEnabled = sw.On = false;
            await this.Navigation.PushAsync(new PremiumPage());
            return;
        }

        if (sw.On)
        {
#if __IOS__
            if (!await NotificationPermissionRequestHelper.NotificationPermissionEnabled(false))
            {
                sw.On = false;
                var result = await NotificationPermissionRequestHelper.RequestNotificationPermission(false);

                if (!result)
                {
                    return;
                }

                sw.On = result;
            }
#endif

            SettingsManager.PoPChanceNotificationEnabled = true;
            UpdaterTaskUtils.StartTasks();
        }
        else
        {
            SettingsManager.PoPChanceNotificationEnabled = false;
            UpdaterTaskUtils.CancelTasks();
        }
    }
}
