using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
#if __IOS__
using Foundation;
using SimpleWeather.Common.Helpers;
#endif
using SimpleWeather.Extras;
using SimpleWeather.Maui.BackgroundTasks;
using SimpleWeather.Maui.Controls;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Extras.Store;
using SimpleWeather.Preferences;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Preferences;

public partial class Settings_WeatherNotifications : ContentPage, ISnackbarManager, IRecipient<SettingsChangedMessage>
{
    private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    private SnackbarManager SnackMgr;

    private readonly IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();

    public Settings_WeatherNotifications()
    {
        InitializeComponent();

        RestoreSettings();

        App.Current.Resources.TryGetValue("LightPrimary", out var LightPrimary);
        App.Current.Resources.TryGetValue("DarkPrimary", out var DarkPrimary);
        SettingsTable.UpdateCellColors(
            Colors.Black, Colors.White, Colors.DimGray, Colors.LightGray,
            LightPrimary as Color, DarkPrimary as Color);

        // Event Listeners
        this.SettingsTable.Model.ItemSelected += Model_ItemSelected;
        DailyNotifPref.OnChanged += DailyNotifPref_OnChanged;
        DailyNotifTimePref.PropertyChanged += DailyNotifTimePref_PropertyChanged;
    }

    private void RestoreSettings()
    {
        // Daily Notification
        DailyNotifPref.On = SettingsManager.DailyNotificationEnabled;
        DailyNotifTimePref.Time = SettingsManager.DailyNotificationTime;
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
        }

        RestoreSettings();
    }

    private async void DailyNotifPref_OnChanged(object sender, ToggledEventArgs e)
    {
        SwitchCell sw = sender as SwitchCell;

        if (sw.On)
        {
#if __IOS__
            if (UIKit.UIApplication.SharedApplication.BackgroundRefreshStatus == UIKit.UIBackgroundRefreshStatus.Denied)
            {
                var snackbar = Snackbar.MakeError(ResStrings.Msg_BGAccessDeniedSettings, SnackbarDuration.Long);
                snackbar.SetAction(ResStrings.action_settings, async () =>
                {
                    await UIKit.UIApplication.SharedApplication.OpenUrlAsync(NSUrl.FromString(UIKit.UIApplication.OpenSettingsUrlString), new UIKit.UIApplicationOpenUrlOptions());
                });
                ShowSnackbar(snackbar);
                SettingsManager.DailyNotificationEnabled = sw.On = false;
                return;
            }
#endif
        }

        if (sw.On && !ExtrasService.IsEnabled())
        {
            SettingsManager.DailyNotificationEnabled = sw.On = false;
            await this.Navigation.PushAsync(new PremiumPage());
            return;
        }

        if (sw.On)
        {
#if __IOS__
            if (!await NotificationPermissionRequestHelper.NotificationPermissionEnabled())
            {
                sw.On = false;
                await NotificationPermissionRequestHelper.RequestNotificationPermission();
                return;
            }
#endif

            SettingsManager.DailyNotificationEnabled = true;
#if __IOS__
            DailyNotificationTask.CancelPendingTasks();
            DailyNotificationTask.ScheduleTask();
#endif
        }
        else
        {
#if __IOS__
            // Unregister task
            DailyNotificationTask.CancelPendingTasks();
#endif
        }
    }

    private void DailyNotifTimePref_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DailyNotifTimePref.Time))
        {
            SettingsManager.DailyNotificationTime = DailyNotifTimePref.Time;

            if (SettingsManager.DailyNotificationEnabled)
            {
#if __IOS__
                DailyNotificationTask.CancelPendingTasks();
                DailyNotificationTask.ScheduleTask();
#endif
            }
        }
    }
}
