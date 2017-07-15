using System;

using Android.App;
using Android.Runtime;
using Android.Content;
using Android.Preferences;
using Com.Nostra13.Universalimageloader.Core;
using Com.Nostra13.Universalimageloader.Cache.Disc.Impl;
using SimpleWeather.Droid.Utils;
using SimpleWeather.Utils;
using System.Threading.Tasks;

namespace SimpleWeather.Droid
{
    //You can specify additional application information in this attribute
    [Application(AllowBackup = true, Icon = "@mipmap/ic_launcher", RoundIcon = "@mipmap/ic_round_launcher",
        Label = "@string/app_name", SupportsRtl = true, Theme = "@style/AppTheme.Launcher",
#if DEBUG
        Debuggable = true
#else
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
            Task.Run(() => Settings.LoadIfNeeded());

            // ImageLoader
            // Use default options
            ImageLoaderConfiguration default_config = new ImageLoaderConfiguration.Builder(ApplicationContext)
                .DiskCache(new UnlimitedDiskCache(ApplicationContext.CacheDir))
                .DefaultDisplayImageOptions(ImageUtils.DefaultDisplayConfig())
                .DiskCacheSize(50 * 1024 * 1024)
                .Build();
            // Initialize ImageLoader with configuration.
            ImageLoader.Instance.Init(default_config);
        }
    }
}