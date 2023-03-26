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

        // Update theme
        App.Current.UpdateAppTheme();
        this.InitThemeListener();

        // Register background tasks
        this.RegisterBGTasks();

        return ret;
    }
}
