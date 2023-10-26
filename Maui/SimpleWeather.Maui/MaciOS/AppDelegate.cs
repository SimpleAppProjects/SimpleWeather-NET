#if __IOS__
using CoreLocation;
using Foundation;
using SimpleWeather.Common.Helpers;
using SimpleWeather.Utils;
using UIKit;

namespace SimpleWeather.Maui;

public partial class AppDelegate
{
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
}
#endif