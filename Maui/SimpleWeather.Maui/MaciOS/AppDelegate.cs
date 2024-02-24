#if __IOS__
using CoreLocation;
using Foundation;
using SimpleWeather.Common.Helpers;
using SimpleWeather.Extras.BackgroundTasks;
using SimpleWeather.Maui.BackgroundTasks;
using SimpleWeather.Maui.Widget;
using SimpleWeather.Utils;
using UIKit;

namespace SimpleWeather.Maui;

public partial class AppDelegate
{
#if DEBUG
    public const string GROUP_IDENTIFIER = "group.com.thewizrd.simpleweather.debug";
#else
    public const string GROUP_IDENTIFIER = "group.com.thewizrd.simpleweather";
#endif

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    private CLLocationManager cLLocationManager;

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        var ret = base.FinishedLaunching(application, launchOptions);

        // Update theme
        App.Current.UpdateAppTheme();
        this.InitThemeListener();

        cLLocationManager = LocationPermissionRequestHelper.GetLocationManager();

        var @delegate = new LocationManagerDelegate();
        @delegate.AuthorizationStatusChanged += LocationManager_AuthorizationStatusChanged;

        cLLocationManager.Delegate = @delegate;

#if DEBUG
        switch (UIApplication.SharedApplication.BackgroundRefreshStatus)
        {
            case UIBackgroundRefreshStatus.Restricted:
                Logger.WriteLine(LoggerLevel.Info, $"{nameof(AppDelegate)}: BackgroundRefreshStatus - restricted");
                break;
            case UIBackgroundRefreshStatus.Denied:
                Logger.WriteLine(LoggerLevel.Info, $"{nameof(AppDelegate)}: BackgroundRefreshStatus - denied");
                break;
            case UIBackgroundRefreshStatus.Available:
                Logger.WriteLine(LoggerLevel.Info, $"{nameof(AppDelegate)}: BackgroundRefreshStatus - available");
                break;
        }

#if false
#pragma warning disable CA1422 // Validate platform compatibility
        UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(TimeSpan.FromHours(1).TotalSeconds);
#pragma warning restore CA1422 // Validate platform compatibility
#endif

        Logger.WriteLine(LoggerLevel.Info, $"{nameof(AppDelegate)}: BackgroundTimeRemaining - {UIApplication.SharedApplication.BackgroundTimeRemaining}");
#endif

        return ret;
    }

    private void LocationManager_AuthorizationStatusChanged(object sender, CLAuthorizationChangedEventArgs e)
    {
        Logger.WriteLine(LoggerLevel.Info, $"LocationManager_AuthorizationStatusChanged: {e.Status}");
    }

    public override void DidEnterBackground(UIApplication application)
    {
        base.DidEnterBackground(application);
        Logger.WriteLine(LoggerLevel.Info, "AppDelegate: DidEnterBackground");

        BGTaskRegistrar.ScheduleBGTasks();
    }

#if false
    public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
    {
        base.PerformFetch(application, completionHandler);
        Logger.WriteLine(LoggerLevel.Info, "AppDelegate: PerformFetch");

        Task.Run(async () =>
        {
            var now = DateTime.UtcNow;

            if (TaskShouldRun(WeatherUpdaterTask.TASK_ID, WeatherUpdaterTask.INTERVAL, now))
            {
                WeatherUpdaterTask.UpdateWeather();
                SetLastTaskRunTime(WeatherUpdaterTask.TASK_ID, now);
            }
            else if (await WeatherWidgetUpdater.WidgetsExist())
            {
                WidgetUpdaterTask.UpdateWidgets();
            }

            // Re-schedule daily notification
            DailyNotificationTask.ScheduleDailyNotification();

            if (TaskShouldRun(RemoteConfigUpdateTask.TASK_ID, RemoteConfigUpdateTask.INTERVAL, now))
            {
                RemoteConfigUpdateTask.CheckConfig();
                SetLastTaskRunTime(RemoteConfigUpdateTask.TASK_ID, now);
            }

            if (TaskShouldRun(PremiumStatusTask.TASK_ID, PremiumStatusTask.INTERVAL, now))
            {
                PremiumStatusTask.CheckPremiumStatus();
                SetLastTaskRunTime(PremiumStatusTask.TASK_ID, now);
            }

            if (TaskShouldRun(AppUpdaterTask.TASK_ID, AppUpdaterTask.INTERVAL, now))
            {
                AppUpdaterTask.CheckForAppUpdates();
                SetLastTaskRunTime(AppUpdaterTask.TASK_ID, now);
            }

            completionHandler.Invoke(UIBackgroundFetchResult.NewData);
        });
    }

    private static bool TaskShouldRun(string taskID, TimeSpan interval, DateTime now)
    {
        var lastTime = GetLastTaskRunTime(taskID);

        return (now - lastTime) >= interval;
    }

    private static DateTime GetLastTaskRunTime(string taskID)
    {
        return Microsoft.Maui.Storage.Preferences.Get($"{taskID}_lastruntime", DateTime.MinValue);
    }

    private static void SetLastTaskRunTime(string taskID, DateTime value)
    {
        Microsoft.Maui.Storage.Preferences.Set($"{taskID}_lastruntime", value);
    }
#endif
}
#endif