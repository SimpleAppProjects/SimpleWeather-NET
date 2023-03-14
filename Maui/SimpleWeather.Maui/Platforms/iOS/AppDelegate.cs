using Foundation;
using UIKit;

namespace SimpleWeather.Maui;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        var ret = base.FinishedLaunching(application, launchOptions);

        App.Current.UpdateAppTheme();

        return ret;
    }
}
