using CoreLocation;
using Foundation;
using SimpleWeather.Common.Helpers;
using SimpleWeather.Utils;
using UIKit;

namespace SimpleWeather.Maui;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
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

        return ret;
    }

    private void LocationManager_AuthorizationStatusChanged(object sender, CLAuthorizationChangedEventArgs e)
    {
        Logger.WriteLine(LoggerLevel.Info, $"LocationManager_AuthorizationStatusChanged: {e.Status}");
    }
}
