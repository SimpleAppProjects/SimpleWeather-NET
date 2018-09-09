using System;

using Android.App;
using Android.Runtime;
using Android.Content;
using Android.Preferences;
using SimpleWeather.Utils;
using System.Threading;
using Android.OS;
using SimpleWeather.Droid.Helpers;

namespace SimpleWeather.Droid.Wear
{
    public enum AppState
    {
        Closed = 0,
        Foreground,
        Background,
    }

    //You can specify additional application information in this attribute
    [Application(Icon = "@mipmap/ic_launcher", RoundIcon = "@mipmap/ic_round_launcher",
        Label = "@string/app_name", SupportsRtl = true, Theme = "@style/WearAppTheme.Launcher",
#if DEBUG
        AllowBackup = false,
        Debuggable = true
#else
        AllowBackup = true,
        Debuggable = false
#endif
        )]
    [MetaData("com.google.android.wearable.standalone", Value = "true")]
    public class App : Application, Application.IActivityLifecycleCallbacks
    {
        public const int HomeIdx = 0;
        public static ISharedPreferences Preferences => PreferenceManager.GetDefaultSharedPreferences(Context);
        public static ISharedPreferencesOnSharedPreferenceChangeListener SharedPreferenceListener;

        public static AppState ApplicationState;
        private int mActivitiesStarted;

        public App(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            //A great place to initialize Xamarin.Insights and Dependency Services!
            RegisterActivityLifecycleCallbacks(this);
            ApplicationState = AppState.Closed;
            mActivitiesStarted = 0;

            Logger.WriteLine(LoggerLevel.Info, "Started logger...");

            AndroidEnvironment.UnhandledExceptionRaiser += App_UnhandledExceptionRaiser;
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;

            var oldHandler = Java.Lang.Thread.DefaultUncaughtExceptionHandler;

            Java.Lang.Thread.DefaultUncaughtExceptionHandler =
                new UncaughtExceptionHandler((thread, throwable) =>
                {
                    Logger.WriteLine(LoggerLevel.Error, throwable, "SimpleWeather: Unhandled Exception {0}", throwable?.Message);

                    if (oldHandler != null)
                    {
                        oldHandler.UncaughtException(thread, throwable);
                    }
                    else
                    {
                        Java.Lang.JavaSystem.Exit(2);
                    }
                });

            // Initialize preference listener
            SharedPreferenceListener = new Settings.SettingsListener();
        }

        private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.WriteLine(LoggerLevel.Error, (Exception)e.ExceptionObject, "SimpleWeather: Unhandled Exception");
        }

        private void App_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            Logger.WriteLine(LoggerLevel.Error, e.Exception, "SimpleWeather: Unhandled Exception {0}", e?.Exception?.Message);
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
            Logger.ForceShutdown();
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            if (activity.LocalClassName.Contains(nameof(LaunchActivity))
                || activity.LocalClassName.Contains(nameof(MainActivity)))
            {
                ApplicationState = AppState.Foreground;
            }
        }

        public void OnActivityStarted(Activity activity)
        {
            if (mActivitiesStarted == 0)
                ApplicationState = AppState.Foreground;

            mActivitiesStarted++;
        }

        public void OnActivityResumed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityStopped(Activity activity)
        {
            mActivitiesStarted--;

            if (mActivitiesStarted == 0)
                ApplicationState = AppState.Background;
        }

        public void OnActivityDestroyed(Activity activity)
        {
            if (activity.LocalClassName.Contains(nameof(MainActivity)))
            {
                ApplicationState = AppState.Closed;
            }
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }
    }
}