using System;

using Android.App;
using Android.Runtime;
using Android.Content;
using Android.Preferences;
using SimpleWeather.Droid.Utils;
using SimpleWeather.Utils;
using System.Threading.Tasks;

namespace SimpleWeather.Droid
{
    //You can specify additional application information in this attribute
    [Application(Icon = "@mipmap/ic_launcher", RoundIcon = "@mipmap/ic_round_launcher",
        Label = "@string/app_name", SupportsRtl = true, Theme = "@style/AppTheme.Launcher",
#if DEBUG
        AllowBackup = false,
        Debuggable = true
#else
        AllowBackup = true,
        Debuggable = false
#endif
        )]
    public class App : Application
    {
        public const int HomeIdx = 0;
        public static ISharedPreferences Preferences => PreferenceManager.GetDefaultSharedPreferences(Context);

        public App(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            //A great place to initialize Xamarin.Insights and Dependency Services!
            // Load data if needed
            Task.Factory.StartNew(() => Settings.LoadIfNeeded());
        }
    }
}