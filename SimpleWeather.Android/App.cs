using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
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
    [Application(Name= "SimpleWeather.Droid.App", AllowBackup = true, Icon = "@mipmap/ic_launcher", RoundIcon = "@mipmap/ic_round_launcher",
        Label = "@string/app_name", SupportsRtl = true, Theme = "@style/AppTheme.Launcher")]
    public class App : Application, Application.IActivityLifecycleCallbacks
    {
        public const int HomeIdx = 0;

        public App(IntPtr handle, JniHandleOwnership transer)
          :base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
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

        public static ISharedPreferences Preferences => PreferenceManager.GetDefaultSharedPreferences(Context);

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
        }
    }
}