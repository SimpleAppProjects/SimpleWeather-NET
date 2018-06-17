using SimpleWeather.Controls;
using SimpleWeather.Utils;
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Appwidget;
using System.Threading.Tasks;
using SimpleWeather.WeatherData;
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;
using Android.Locations;
using SimpleWeather.Droid.Utils;
using Android.Views;
using Android.Util;
using SimpleWeather.Droid.App.Notifications;
using Android.Text.Format;
using Android.Text;
using Android.Text.Style;
using Android.Support.V4.App;
using SimpleWeather.Droid.App.WeatherAlerts;
using SimpleWeather.Droid.Helpers;

namespace SimpleWeather.Droid.App.Widgets
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class WeatherWidgetBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            // Relay intent to WeatherWidgetService
            intent.SetClass(context, Java.Lang.Class.FromType(typeof(WeatherWidgetService)));
            WeatherWidgetService.EnqueueWork(context, intent);
        }
    }

    [Service(Exported = false, Permission = "android.permission.BIND_JOB_SERVICE")]
    public partial class WeatherWidgetService : JobIntentService
    {
        private static string TAG = nameof(WeatherWidgetService);

        public const string ACTION_REFRESHWIDGET = "SimpleWeather.Droid.action.REFRESH_WIDGET";
        public const string ACTION_RESIZEWIDGET = "SimpleWeather.Droid.action.RESIZE_WIDGET";
        public const string ACTION_UPDATECLOCK = "SimpleWeather.Droid.action.UPDATE_CLOCK";
        public const string ACTION_UPDATEDATE = "SimpleWeather.Droid.action.UPDATE_DATE";
        public const string ACTION_UPDATEWEATHER = "SimpleWeather.Droid.action.UPDATE_WEATHER";

        public const string ACTION_STARTALARM = "SimpleWeather.Droid.action.START_ALARM";
        public const string ACTION_CANCELALARM = "SimpleWeather.Droid.action.CANCEL_ALARM";
        public const string ACTION_UPDATEALARM = "SimpleWeather.Droid.action.UPDATE_ALARM";

        public const string ACTION_REFRESHNOTIFICATION = "SimpleWeather.Droid.action.REFRESH_NOTIFICATION";
        public const string ACTION_REMOVENOTIFICATION = "SimpleWeather.Droid.action.REMOVE_NOTIFICATION";

        public const string ACTION_STARTCLOCK = "SimpleWeather.Droid.action.START_CLOCKALARM";
        public const string ACTION_CANCELCLOCK = "SimpleWeather.Droid.action.CANCEL_CLOCKALARM";

        public const string ACTION_SHOWALERTS = "SimpleWeather.Droid.action.SHOW_ALERTS";

        private const int JOB_ID = 1000;

        private Context mContext;
        private AppWidgetManager mAppWidgetManager;
        private static bool alarmStarted = false;
        private static BroadcastReceiver mTickReceiver;

        private WeatherManager wm;

        // Weather Widget Providers
        private WeatherWidgetProvider1x1 mAppWidget1x1 =
            WeatherWidgetProvider1x1.GetInstance();
        private WeatherWidgetProvider2x2 mAppWidget2x2 =
            WeatherWidgetProvider2x2.GetInstance();
        private WeatherWidgetProvider4x1 mAppWidget4x1 = 
            WeatherWidgetProvider4x1.GetInstance();
        private WeatherWidgetProvider4x2 mAppWidget4x2 =
            WeatherWidgetProvider4x2.GetInstance();

        private const int FORECAST_LENGTH = 3; // 3-day
        private const int MEDIUM_FORECAST_LENGTH = 4; // 4-day
        private const int WIDE_FORECAST_LENGTH = 5; // 5-day

        public static void EnqueueWork(Context context, Intent work)
        {
            EnqueueWork(context,
                Java.Lang.Class.FromType(typeof(WeatherWidgetService)),
                JOB_ID, work);
        }

        public override void OnCreate()
        {
            base.OnCreate();

            mContext = ApplicationContext;
            mAppWidgetManager = AppWidgetManager.GetInstance(mContext);
            wm = WeatherManager.GetInstance();

            var oldHandler = Java.Lang.Thread.DefaultUncaughtExceptionHandler;

            Java.Lang.Thread.DefaultUncaughtExceptionHandler =
                new UncaughtExceptionHandler((thread, throwable) =>
                {
                    Logger.WriteLine(LoggerLevel.Error, throwable, "{0}: Unhandled Exception {1}", TAG, throwable?.Message);

                    if (oldHandler != null)
                    {
                        oldHandler.UncaughtException(thread, throwable);
                    }
                    else
                    {
                        Java.Lang.JavaSystem.Exit(2);
                    }
                });
        }

        protected override void OnHandleWork(Intent intent)
        {
            if (ACTION_REFRESHWIDGET.Equals(intent?.Action))
            {
                int[] appWidgetIds = intent.GetIntArrayExtra(WeatherWidgetProvider.EXTRA_WIDGET_IDS);
                WidgetType widgetType = (WidgetType)intent.GetIntExtra(WeatherWidgetProvider.EXTRA_WIDGET_TYPE, -1);

                switch (widgetType)
                {
                    case WidgetType.Widget1x1:
                        Task.Run(async () => await RefreshWidget(mAppWidget1x1, appWidgetIds));
                        break;
                    case WidgetType.Widget2x2:
                        Task.Run(async () => await RefreshWidget(mAppWidget2x2, appWidgetIds));
                        break;
                    case WidgetType.Widget4x1:
                        Task.Run(async () => await RefreshWidget(mAppWidget4x1, appWidgetIds));
                        break;
                    case WidgetType.Widget4x2:
                        Task.Run(async () => await RefreshWidget(mAppWidget4x2, appWidgetIds));
                        break;
                    // We don't know the widget type to update,
                    // so just update all
                    case WidgetType.Unknown:
                    default:
                        Task.Run(async () => await RefreshWidgets());
                        break;
                }
            }
            else if (ACTION_RESIZEWIDGET.Equals(intent?.Action))
            {
                int appWidgetId = intent.GetIntExtra(WeatherWidgetProvider.EXTRA_WIDGET_ID, -1);
                WidgetType widgetType = (WidgetType)intent.GetIntExtra(WeatherWidgetProvider.EXTRA_WIDGET_TYPE, -1);
                Bundle newOptions = intent.GetBundleExtra(WeatherWidgetProvider.EXTRA_WIDGET_OPTIONS);

                switch (widgetType)
                {
                    case WidgetType.Widget1x1:
                    default:
                        // Widget resizes itself; no need to adjust
                        break;
                    case WidgetType.Widget2x2:
                        Task.Run(async () => await ResizeWidget(mAppWidget2x2, appWidgetId, newOptions));
                        break;
                    case WidgetType.Widget4x1:
                        Task.Run(async () => await ResizeWidget(mAppWidget4x1, appWidgetId, newOptions));
                        break;
                    case WidgetType.Widget4x2:
                        Task.Run(async () => await ResizeWidget(mAppWidget4x2, appWidgetId, newOptions));
                        break;
                }
            }
            else if (ACTION_STARTALARM.Equals(intent?.Action))
            {
                // Start alarm if it hasn't started already
                StartAlarm(mContext);
            }
            else if (ACTION_CANCELALARM.Equals(intent?.Action))
            {
                // Cancel all alarms if no widgets exist
                CancelAlarms(mContext);
            }
            else if (ACTION_UPDATEALARM.Equals(intent?.Action))
            {
                // Refresh interval was changed
                // Update alarm
                UpdateAlarm(mContext);
            }
            else if (ACTION_STARTCLOCK.Equals(intent?.Action))
            {
                // Schedule clock updates
                StartTickReceiver(mContext);
            }
            else if (ACTION_CANCELCLOCK.Equals(intent?.Action))
            {
                // Cancel clock alarm
                CancelClockAlarm(mContext);
            }
            else if (ACTION_UPDATECLOCK.Equals(intent?.Action))
            {
                // Update clock widget instances
                int[] appWidgetIds = intent.GetIntArrayExtra(WeatherWidgetProvider.EXTRA_WIDGET_IDS);
                RefreshClock(appWidgetIds);
            }
            else if (ACTION_UPDATEDATE.Equals(intent?.Action))
            {
                // Update clock widget instances
                int[] appWidgetIds = intent.GetIntArrayExtra(WeatherWidgetProvider.EXTRA_WIDGET_IDS);
                RefreshDate(appWidgetIds);
            }
            else if (ACTION_REFRESHNOTIFICATION.Equals(intent?.Action))
            {
                if (Settings.WeatherLoaded)
                {
                    var weather = Task.Run(GetWeather).Result;

                    if (Settings.OnGoingNotification && weather != null)
                        WeatherNotificationBuilder.UpdateNotification(weather);
                }
            }
            else if (ACTION_REMOVENOTIFICATION.Equals(intent?.Action))
            {
                WeatherNotificationBuilder.RemoveNotification();
            }
            else if (ACTION_UPDATEWEATHER.Equals(intent?.Action))
            {
                if (Settings.WeatherLoaded)
                {
                    // Send broadcast to signal update
                    if (WidgetsExist(App.Context))
                        SendBroadcast(new Intent(WeatherWidgetProvider.ACTION_SHOWREFRESH));
                    // NOTE: Don't try to show refresh for pre-M devices
                    // If app gets killed, instance of notif is lost & view is reset
                    // and might get stuck
                    if (Settings.OnGoingNotification && Build.VERSION.SdkInt >= BuildVersionCodes.M)
                        WeatherNotificationBuilder.ShowRefresh();

                    if (WidgetsExist(App.Context))
                        Task.Run(async () => await RefreshWidgets());

                    // Update for home
                    var weather = Task.Run(GetWeather).Result;
                    if (weather != null)
                    {
                        if (Settings.OnGoingNotification)
                            WeatherNotificationBuilder.UpdateNotification(weather);
                        if (Settings.ShowAlerts && wm.SupportsAlerts && weather != null)
                            Task.Run(async () => await WeatherAlertHandler.PostAlerts(Settings.HomeData, weather.weather_alerts));
                    }
                }
            }

            Logger.WriteLine(LoggerLevel.Info, "{0}: Intent Action = {1}", TAG, intent?.Action);
        }

        private static PendingIntent GetAlarmIntent(Context context)
        {
            Intent intent = new Intent(context, typeof(WeatherWidgetBroadcastReceiver))
                .SetAction(ACTION_UPDATEWEATHER);

            return PendingIntent.GetBroadcast(context, 0, intent, 0);
        }

        private static void UpdateAlarm(Context context)
        {
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            int interval = Settings.RefreshInterval;

            bool startNow = !alarmStarted;
            long intervalMillis = (long)TimeSpan.FromMinutes(interval).TotalMilliseconds;
            long triggerAtTime = startNow ? SystemClock.ElapsedRealtime() : SystemClock.ElapsedRealtime() + intervalMillis;

            PendingIntent pendingIntent = GetAlarmIntent(context);
            am.Cancel(pendingIntent);
            am.SetInexactRepeating(AlarmType.ElapsedRealtime, triggerAtTime, intervalMillis, pendingIntent);
            alarmStarted = true;

            Logger.WriteLine(LoggerLevel.Info, "{0}: Updated alarm", TAG);
        }

        private void CancelAlarms(Context context)
        {
            // Cancel alarm if dependent features are turned off
            if (!WidgetsExist(context) && !Settings.OnGoingNotification && !Settings.ShowAlerts)
            {
                AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
                am.Cancel(GetAlarmIntent(context));
                alarmStarted = false;

                Logger.WriteLine(LoggerLevel.Info, "{0}: Canceled alarm", TAG);
            }
        }

        private void StartAlarm(Context context)
        {
            // Start alarm if dependent features are enabled
            if (!alarmStarted && (WidgetsExist(context) || Settings.OnGoingNotification || Settings.ShowAlerts))
            {
                UpdateAlarm(context);
                alarmStarted = true;
            }
        }

        private static PendingIntent GetClockRefreshIntent(Context context)
        {
            Intent intent = new Intent(context, typeof(WeatherWidgetBroadcastReceiver))
                .SetAction(ACTION_UPDATECLOCK);

            return PendingIntent.GetBroadcast(context, 0, intent, 0);
        }

        private static void StartTickReceiver(Context context)
        {
            StopTickReceiver(context);

            mTickReceiver = new TickReceiver();
            context.RegisterReceiver(mTickReceiver, new IntentFilter(Intent.ActionTimeTick));

            Logger.WriteLine(LoggerLevel.Info, "{0}: Started tick receiver", TAG);
        }

        private static void StopTickReceiver(Context context)
        {
            if (mTickReceiver != null)
            {
                context.UnregisterReceiver(mTickReceiver);
                mTickReceiver = null;

                Logger.WriteLine(LoggerLevel.Info, "{0}: Unregistered tick receiver", TAG);
            }
        }

        private static void CancelClockAlarm(Context context)
        {
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            am.Cancel(GetClockRefreshIntent(context));

            Logger.WriteLine(LoggerLevel.Info, "{0}: Canceled clock alarm", TAG);
        }

        internal class TickReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                if (Intent.ActionTimeTick.Equals(intent?.Action))
                {
                    AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
                    PendingIntent pendingIntent = GetClockRefreshIntent(context);
                    am.Cancel(pendingIntent);
                    am.SetRepeating(AlarmType.Rtc, Java.Lang.JavaSystem.CurrentTimeMillis(), 60000, pendingIntent);

                    StopTickReceiver(context);

                    Logger.WriteLine(LoggerLevel.Info, "{0}: Receieved tick in receiver", TAG);
                }
            }
        }

        private bool WidgetsExist(Context context)
        {
            return mAppWidget1x1.HasInstances(context) || mAppWidget2x2.HasInstances(context) || mAppWidget4x1.HasInstances(context) || mAppWidget4x2.HasInstances(context);
        }

        private async Task ResizeWidget(WeatherWidgetProvider provider, int appWidgetId, Bundle newOptions)
        {
            if (Settings.WeatherLoaded)
                await RebuildForecast(provider, appWidgetId, newOptions);
        }

        private async Task RefreshWidget(WeatherWidgetProvider provider, int[] appWidgetIds)
        {
            if (appWidgetIds == null || appWidgetIds.Length == 0)
                appWidgetIds = mAppWidgetManager.GetAppWidgetIds(provider.ComponentName);

            if (Settings.WeatherLoaded)
            {
                foreach (int id in appWidgetIds)
                {
                    var locData = WidgetUtils.GetLocationData(id);
                    Weather weather = null;

                    if (locData != null)
                    {
                        if (locData.Equals(Settings.HomeData))
                        {
                            weather = await GetWeather();
                        }
                        else
                        {
                            var wloader = new WeatherDataLoader(locData);
                            await wloader.LoadWeatherData(false);
                            weather = wloader.GetWeather();
                        }

                        if (weather != null)
                        {
                            // Save weather data
                            WidgetUtils.SaveWeatherData(id, weather);

                            // Build the widget update for provider
                            var views = BuildUpdate(mContext, provider, id, locData, weather);
                            // Push update for this widget to the home screen
                            mAppWidgetManager.UpdateAppWidget(id, views);
                            BuildForecast(provider, weather, id);
                        }
                    }
                }
            }

            if (provider.WidgetType == WidgetType.Widget4x2)
            {
                RefreshClock(appWidgetIds);
                RefreshDate(appWidgetIds);
            }
        }

        private async Task RefreshWidgets()
        {
            if (Settings.WeatherLoaded)
            {
                // Build the widget update for available providers
                // Add widget providers here
                if (mAppWidget1x1.HasInstances(this))
                {
                    int[] appWidgetIds = mAppWidgetManager.GetAppWidgetIds(mAppWidget1x1.ComponentName);

                    foreach (int id in appWidgetIds)
                    {
                        var locData = WidgetUtils.GetLocationData(id);
                        Weather weather = null;

                        if (locData != null)
                        {
                            if (locData.Equals(Settings.HomeData))
                            {
                                weather = await GetWeather();
                            }
                            else
                            {
                                var wloader = new WeatherDataLoader(locData);
                                await wloader.LoadWeatherData(false);
                                weather = wloader.GetWeather();
                            }

                            if (weather != null)
                            {
                                // Save weather data
                                WidgetUtils.SaveWeatherData(id, weather);

                                // Build the widget update for provider
                                var views = BuildUpdate(mContext, mAppWidget1x1, id, locData, weather);
                                // Push update for this widget to the home screen
                                mAppWidgetManager.UpdateAppWidget(id, views);
                                BuildForecast(mAppWidget1x1, weather, id);
                            }
                        }
                    }
                }

                if (mAppWidget2x2.HasInstances(this))
                {
                    int[] appWidgetIds = mAppWidgetManager.GetAppWidgetIds(mAppWidget2x2.ComponentName);

                    foreach (int id in appWidgetIds)
                    {
                        var locData = WidgetUtils.GetLocationData(id);
                        Weather weather = null;

                        if (locData != null)
                        {
                            if (locData.Equals(Settings.HomeData))
                            {
                                weather = await GetWeather();
                            }
                            else
                            {
                                var wloader = new WeatherDataLoader(locData);
                                await wloader.LoadWeatherData(false);
                                weather = wloader.GetWeather();
                            }

                            if (weather != null)
                            {
                                // Save weather data
                                WidgetUtils.SaveWeatherData(id, weather);

                                // Build the widget update for provider
                                var views = BuildUpdate(mContext, mAppWidget2x2, id, locData, weather);
                                // Push update for this widget to the home screen
                                mAppWidgetManager.UpdateAppWidget(id, views);
                                BuildForecast(mAppWidget2x2, weather, id);
                            }
                        }
                    }
                }

                if (mAppWidget4x1.HasInstances(this))
                {
                    int[] appWidgetIds = mAppWidgetManager.GetAppWidgetIds(mAppWidget4x1.ComponentName);

                    foreach (int id in appWidgetIds)
                    {
                        var locData = WidgetUtils.GetLocationData(id);
                        Weather weather = null;

                        if (locData != null)
                        {
                            if (locData.Equals(Settings.HomeData))
                            {
                                weather = await GetWeather();
                            }
                            else
                            {
                                var wloader = new WeatherDataLoader(locData);
                                await wloader.LoadWeatherData(false);
                                weather = wloader.GetWeather();
                            }

                            if (weather != null)
                            {
                                // Save weather data
                                WidgetUtils.SaveWeatherData(id, weather);

                                // Build the widget update for provider
                                var views = BuildUpdate(mContext, mAppWidget4x1, id, locData, weather);
                                // Push update for this widget to the home screen
                                mAppWidgetManager.UpdateAppWidget(id, views);
                                BuildForecast(mAppWidget4x1, weather, id);
                            }
                        }
                    }
                }

                if (mAppWidget4x2.HasInstances(this))
                {
                    int[] appWidgetIds = mAppWidgetManager.GetAppWidgetIds(mAppWidget4x2.ComponentName);

                    foreach (int id in appWidgetIds)
                    {
                        var locData = WidgetUtils.GetLocationData(id);
                        Weather weather = null;

                        if (locData != null)
                        {
                            if (locData.Equals(Settings.HomeData))
                            {
                                weather = await GetWeather();
                            }
                            else
                            {
                                var wloader = new WeatherDataLoader(locData);
                                await wloader.LoadWeatherData(false);
                                weather = wloader.GetWeather();
                            }

                            if (weather != null)
                            {
                                // Save weather data
                                WidgetUtils.SaveWeatherData(id, weather);

                                // Build the widget update for provider
                                var views = BuildUpdate(mContext, mAppWidget4x2, id, locData, weather);
                                // Push update for this widget to the home screen
                                mAppWidgetManager.UpdateAppWidget(id, views);
                                BuildForecast(mAppWidget4x2, weather, id);
                            }
                        }
                    }

                    RefreshClock(appWidgetIds);
                    RefreshDate(appWidgetIds);
                }
            }
        }

        private void RefreshClock(int[] appWidgetIds)
        {
            if (appWidgetIds == null || appWidgetIds.Length == 0)
                appWidgetIds = mAppWidgetManager.GetAppWidgetIds(mAppWidget4x2.ComponentName);

            // Update 4x2 clock widgets
            RemoteViews views = new RemoteViews(mContext.PackageName, mAppWidget4x2.WidgetLayoutId);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
            {
                // TextClock
                views.SetCharSequence(Resource.Id.clock_panel, "setFormat12Hour",
                    mContext.GetTextFormatted(Resource.String.main_widget_12_hours_format));
                views.SetCharSequence(Resource.Id.clock_panel, "setFormat24Hour",
                    mContext.GetTextFormatted(Resource.String.clock_24_hours_format));
            }
            else
            {
                // TextView
                var now = DateTime.Now;

                SpannableString timeStr;
                string timeformat = now.ToString("h:mmtt");
                int end = timeformat.Length - 2;

                if (DateFormat.Is24HourFormat(App.Context))
                {
                    timeformat = now.ToString("HH:mm");
                    end = timeformat.Length - 1;
                    timeStr = new SpannableString(timeformat);
                }
                else
                {
                    timeStr = new SpannableString(timeformat);
                    timeStr.SetSpan(new TextAppearanceSpan("sans-serif", Android.Graphics.TypefaceStyle.Bold, 16,
                        ContextCompat.GetColorStateList(mContext, Android.Resource.Color.White),
                        ContextCompat.GetColorStateList(mContext, Android.Resource.Color.White)),
                        end, timeformat.Length, SpanTypes.ExclusiveExclusive);
                }

                views.SetTextViewText(Resource.Id.clock_panel, timeStr);
            }

            mAppWidgetManager.PartiallyUpdateAppWidget(appWidgetIds, views);

            Logger.WriteLine(LoggerLevel.Info, "{0}: Refreshed clock", TAG);
        }

        private void RefreshDate(int[] appWidgetIds)
        {
            if (appWidgetIds == null || appWidgetIds.Length == 0)
                appWidgetIds = mAppWidgetManager.GetAppWidgetIds(mAppWidget4x2.ComponentName);

            // Update 4x2 clock widgets
            RemoteViews views = new RemoteViews(mContext.PackageName, mAppWidget4x2.WidgetLayoutId);
            views.SetTextViewText(Resource.Id.date_panel, DateTime.Now.ToString("ddd, MMM dd"));
            mAppWidgetManager.PartiallyUpdateAppWidget(appWidgetIds, views);

            Logger.WriteLine(LoggerLevel.Info, "{0}: Refreshed date", TAG);
        }

        private static PendingIntent GetDefaultCalendarIntent(Context context)
        {
            Intent onClickIntent = Intent.MakeMainSelectorActivity(Intent.ActionMain, Intent.CategoryAppCalendar);
            return PendingIntent.GetActivity(context, 0, onClickIntent, 0);
        }

        private static PendingIntent GetDefaultClockIntent(Context context)
        {
            string clockAction = Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat ?
                Android.Provider.AlarmClock.ActionShowAlarms : Android.Provider.AlarmClock.ActionSetAlarm;
            Intent onClickIntent = new Intent(clockAction);
            return PendingIntent.GetActivity(context, 0, onClickIntent, 0);
        }

        private RemoteViews BuildUpdate(Context context, WeatherWidgetProvider provider, int appWidgetId, LocationData location, Weather weather)
        {
            // Build an update that holds the updated widget contents
            RemoteViews updateViews = new RemoteViews(context.PackageName, provider.WidgetLayoutId);

            // Progress bar
            updateViews.SetViewVisibility(Resource.Id.refresh_button, ViewStates.Visible);
            updateViews.SetViewVisibility(Resource.Id.refresh_progress, ViewStates.Gone);

            // Set on click refresh intent
            SetOnRefreshIntent(context, provider, appWidgetId, updateViews);

            // Temperature
            string temp = Settings.IsFahrenheit ?
                Math.Round(weather.condition.temp_f) + "\uf045" : Math.Round(weather.condition.temp_c) + "\uf03c";
            int tempTextSize = 72;
            if (provider.WidgetType == WidgetType.Widget2x2 || provider.WidgetType == WidgetType.Widget4x2)
                tempTextSize = 96;
            updateViews.SetImageViewBitmap(Resource.Id.condition_temp,
                ImageUtils.WeatherIconToBitmap(context, temp, tempTextSize));

            // Location Name
            updateViews.SetTextViewText(Resource.Id.location_name, weather.location.name);
            // Update Time
            string updatetext = GetUpdateTimeText(Settings.UpdateTime, false);
            updateViews.SetTextViewText(Resource.Id.update_time, updatetext);

            // Background
            var color = wm.GetWeatherBackgroundColor(weather);
            updateViews.SetInt(Resource.Id.widgetBackground, "setBackgroundColor", color);

            // WeatherIcon
            updateViews.SetImageViewResource(Resource.Id.weather_icon,
                wm.GetWeatherIconResource(weather.condition.icon));

            // Set data for larger widgets
            if (provider.WidgetType != WidgetType.Widget1x1)
            {
                // Condition text
                updateViews.SetTextViewText(Resource.Id.condition_weather, weather.condition.weather);

                // Details
                if (provider.WidgetType == WidgetType.Widget2x2 || provider.WidgetType == WidgetType.Widget4x2)
                {
                    // Feels like temp
                    updateViews.SetTextViewText(Resource.Id.condition_feelslike,
                        (Settings.IsFahrenheit ?
                            Math.Round(weather.condition.feelslike_f) : Math.Round(weather.condition.feelslike_c)) + "º");

                    // Wind
                    updateViews.SetTextViewText(Resource.Id.condition_wind,
                        (Settings.IsFahrenheit ?
                             weather.condition.wind_mph.ToString() + " mph" : weather.condition.wind_kph.ToString() + " kph"));

                    // Show precipitation % if available
                    if (weather.precipitation != null)
                    {
                        updateViews.SetViewVisibility(Resource.Id.condition_pop_panel, ViewStates.Visible);
                        updateViews.SetTextViewText(Resource.Id.condition_pop, weather.precipitation.pop + "%");

                        if (Settings.API.Equals(WeatherAPI.OpenWeatherMap) || Settings.API.Equals(WeatherAPI.MetNo))
                            updateViews.SetImageViewResource(Resource.Id.condition_pop_label, Resource.Drawable.ic_cloudy);
                        else
                            updateViews.SetImageViewResource(Resource.Id.condition_pop_label, Resource.Drawable.ic_raindrop);
                    }
                    else
                    {
                        updateViews.SetViewVisibility(Resource.Id.condition_pop_panel, ViewStates.Gone);
                    }

                    // Open default clock/calendar app
                    if (provider.WidgetType == WidgetType.Widget4x2)
                    {
                        updateViews.SetOnClickPendingIntent(Resource.Id.date_panel, GetDefaultCalendarIntent(context));
                        updateViews.SetOnClickPendingIntent(Resource.Id.clock_panel, GetDefaultClockIntent(context));
                    }
                }
            }

            SetOnClickIntent(context, location, updateViews);

            return updateViews;
        }

        private static void SetOnRefreshIntent(Context context, WeatherWidgetProvider provider, int appWidgetId, RemoteViews updateViews)
        {
            Intent refreshIntent = new Intent(context, typeof(WeatherWidgetBroadcastReceiver))
                .SetAction(ACTION_REFRESHWIDGET)
                .PutExtra(WeatherWidgetProvider.EXTRA_WIDGET_IDS, new int[] { appWidgetId })
                .PutExtra(WeatherWidgetProvider.EXTRA_WIDGET_TYPE, (int)provider.WidgetType);
            PendingIntent refreshPendingIntent =
                PendingIntent.GetBroadcast(context, appWidgetId, refreshIntent, PendingIntentFlags.UpdateCurrent);
            updateViews?.SetOnClickPendingIntent(Resource.Id.refresh_button, refreshPendingIntent);
        }

        private static void SetOnClickIntent(Context context, LocationData location, RemoteViews updateViews)
        {
            // When user clicks on widget, launch to WeatherNow page
            Intent onClickIntent = new Intent(context.ApplicationContext, typeof(MainActivity))
                .SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);

            if (!Settings.HomeData.Equals(location))
                onClickIntent.PutExtra("shortcut-data", location?.ToJson());

            PendingIntent clickPendingIntent =
                PendingIntent.GetActivity(context, location.GetHashCode(), onClickIntent, PendingIntentFlags.UpdateCurrent);
            updateViews?.SetOnClickPendingIntent(Resource.Id.widgetBackground, clickPendingIntent);
        }

        // TODO: Merge into function below
        private async Task RebuildForecast(WeatherWidgetProvider provider, int appWidgetId, Bundle newOptions)
        {
            var weather = WidgetUtils.GetWeatherData(appWidgetId);

            if (weather == null)
            {
                var locData = WidgetUtils.GetLocationData(appWidgetId);
                if (locData != null)
                {
                    var wloader = new WeatherDataLoader(locData);
                    await wloader.LoadWeatherData(false);
                    weather = wloader.GetWeather();
                    if (weather != null)
                        WidgetUtils.SaveWeatherData(appWidgetId, weather);
                    else
                        return;
                }
            }

            RemoteViews updateViews = new RemoteViews(mContext.PackageName, provider.WidgetLayoutId);

            // Widget dimensions
            int minHeight = newOptions.GetInt(AppWidgetManager.OptionAppwidgetMinHeight);
            int minWidth = newOptions.GetInt(AppWidgetManager.OptionAppwidgetMinWidth);
            int maxHeight = newOptions.GetInt(AppWidgetManager.OptionAppwidgetMaxHeight);
            int maxWidth = newOptions.GetInt(AppWidgetManager.OptionAppwidgetMaxWidth);
            int maxCellHeight = GetCellsForSize(maxHeight);
            int maxCellWidth = GetCellsForSize(maxWidth);
            int cellHeight = GetCellsForSize(minHeight);
            int cellWidth = GetCellsForSize(minWidth);

            // Determine forecast size
            int forecastLength = GetForecastLength(provider.WidgetType, cellWidth);
            if (weather.forecast.Length < forecastLength)
                forecastLength = weather.forecast.Length;

            if (provider.WidgetType == WidgetType.Widget4x1)
            {
                if (cellWidth > 3)
                    updateViews.SetViewVisibility(Resource.Id.condition_weather, ViewStates.Visible);
                else
                    updateViews.SetViewVisibility(Resource.Id.condition_weather, ViewStates.Gone);
            }
            else if (provider.WidgetType == WidgetType.Widget4x2)
            {
                float clockSize = mContext.Resources.GetDimension(Resource.Dimension.clock_text_size);
                float dateSize = mContext.Resources.GetDimension(Resource.Dimension.date_text_size);
                float scale = (cellHeight != maxCellHeight) ? 1.25f : 1f;

                updateViews.SetTextViewTextSize(Resource.Id.clock_panel, (int)ComplexUnitType.Px, clockSize * scale);
                updateViews.SetTextViewTextSize(Resource.Id.date_panel, (int)ComplexUnitType.Px, dateSize * scale);
            }

            updateViews.RemoveAllViews(Resource.Id.forecast_layout);
            BuildForecastPanel(updateViews, provider, weather, forecastLength, cellHeight == maxCellHeight);
            mAppWidgetManager.PartiallyUpdateAppWidget(appWidgetId, updateViews);
        }

        private void BuildForecast(WeatherWidgetProvider provider, Weather weather)
        {
            int[] appWidgetIds = mAppWidgetManager.GetAppWidgetIds(provider.ComponentName);
            BuildForecast(provider, weather, appWidgetIds);
        }

        private void BuildForecast(WeatherWidgetProvider provider, Weather weather, int appWidgetId)
        {
            BuildForecast(provider, weather, new int[] { appWidgetId });
        }

        private void BuildForecast(WeatherWidgetProvider provider, Weather weather, int[] appWidgetIds)
        {
            if (weather == null)
                return;

            for (int i = 0; i < appWidgetIds.Length; i++)
            {
                RemoteViews updateViews = new RemoteViews(mContext.PackageName, provider.WidgetLayoutId);
                updateViews.RemoveAllViews(Resource.Id.forecast_layout);

                Bundle newOptions = mAppWidgetManager.GetAppWidgetOptions(appWidgetIds[i]);

                // Widget dimensions
                int minHeight = newOptions.GetInt(AppWidgetManager.OptionAppwidgetMinHeight);
                int minWidth = newOptions.GetInt(AppWidgetManager.OptionAppwidgetMinWidth);
                int maxHeight = newOptions.GetInt(AppWidgetManager.OptionAppwidgetMaxHeight);
                int maxWidth = newOptions.GetInt(AppWidgetManager.OptionAppwidgetMaxWidth);
                int maxCellHeight = GetCellsForSize(maxHeight);
                int maxCellWidth = GetCellsForSize(maxWidth);
                int cellHeight = GetCellsForSize(minHeight);
                int cellWidth = GetCellsForSize(minWidth);

                // Determine forecast size
                int forecastLength = GetForecastLength(provider.WidgetType, cellWidth);
                if (weather.forecast.Length < forecastLength)
                    forecastLength = weather.forecast.Length;

                if (provider.WidgetType == WidgetType.Widget4x1)
                {
                    if (cellWidth > 3)
                        updateViews.SetViewVisibility(Resource.Id.condition_weather, ViewStates.Visible);
                    else
                        updateViews.SetViewVisibility(Resource.Id.condition_weather, ViewStates.Gone);
                }
                else if (provider.WidgetType == WidgetType.Widget4x2)
                {
                    float clockSize = mContext.Resources.GetDimension(Resource.Dimension.clock_text_size);
                    float dateSize = mContext.Resources.GetDimension(Resource.Dimension.date_text_size);
                    float scale = (cellHeight != maxCellHeight) ? 1.25f : 1f;

                    updateViews.SetTextViewTextSize(Resource.Id.clock_panel, (int)ComplexUnitType.Px, clockSize * scale);
                    updateViews.SetTextViewTextSize(Resource.Id.date_panel, (int)ComplexUnitType.Px, dateSize * scale);
                }

                BuildForecastPanel(updateViews, provider, weather, forecastLength, cellHeight == maxCellHeight);
                mAppWidgetManager.PartiallyUpdateAppWidget(appWidgetIds[i], updateViews);
            }
        }

        private void BuildForecastPanel(
            RemoteViews updateViews, WeatherWidgetProvider provider, Weather weather,
            int forecastLength, bool forceSmall)
        {
            if (weather == null)
                return;

            for (int i = 0; i < forecastLength; i++)
            {
                var forecast = weather.forecast[i];

                RemoteViews forecastPanel = null;
                if (provider.WidgetType == WidgetType.Widget4x1)
                    forecastPanel = new RemoteViews(mContext.PackageName, Resource.Layout.app_widget_forecast_panel);
                else if (provider.WidgetType == WidgetType.Widget2x2 || forceSmall)
                    forecastPanel = new RemoteViews(mContext.PackageName, Resource.Layout.app_widget_forecast_panel_small);
                else
                    forecastPanel = new RemoteViews(mContext.PackageName, Resource.Layout.app_widget_forecast_panel_medium);

                forecastPanel.SetTextViewText(Resource.Id.forecast_date, forecast.date.ToString("ddd"));
                forecastPanel.SetImageViewResource(Resource.Id.forecast_icon,
                    wm.GetWeatherIconResource(forecast.icon));
                forecastPanel.SetTextViewText(Resource.Id.forecast_hi,
                    (Settings.IsFahrenheit ? forecast.high_f : forecast.high_c) + "º");
                forecastPanel.SetTextViewText(Resource.Id.forecast_lo,
                    (Settings.IsFahrenheit ? forecast.low_f : forecast.low_c) + "º");

                updateViews.AddView(Resource.Id.forecast_layout, forecastPanel);
            }
        }

        /**
         * Returns number of cells needed for given size of the widget.
         *
         * @param size Widget size in dp.
         * @return Size in number of cells.
         */
        private static int GetCellsForSize(int size)
        {
            // The hardwired sizes in this function come from the hardwired formula found in
            // Android's UI guidelines for widget design:
            // http://developer.android.com/guide/practices/ui_guidelines/widget_design.html
            return (size + 30) / 70;
        }

        private static int GetForecastLength(WidgetType widgetType, int cellWidth)
        {
            int forecastLength = (widgetType == WidgetType.Widget4x2) ? WIDE_FORECAST_LENGTH : FORECAST_LENGTH;

            if (cellWidth < 2)
            {
                if (widgetType == WidgetType.Widget4x1)
                    forecastLength = 0;
            }
            else if (cellWidth == 2)
            {
                if (widgetType == WidgetType.Widget4x1)
                    forecastLength = 1;
            }
            else if (cellWidth == 3)
            {
                if (widgetType == WidgetType.Widget4x1)
                    forecastLength = 2;
                else if (widgetType == WidgetType.Widget2x2)
                    forecastLength = MEDIUM_FORECAST_LENGTH;
            }
            else if (cellWidth == 4)
            {
                if (widgetType == WidgetType.Widget2x2 || widgetType == WidgetType.Widget4x2)
                    forecastLength = WIDE_FORECAST_LENGTH;
            }
            else if (cellWidth > 4)
            {
                if (widgetType == WidgetType.Widget4x1)
                    forecastLength = MEDIUM_FORECAST_LENGTH;
                else if (widgetType == WidgetType.Widget2x2 || widgetType == WidgetType.Widget4x2)
                    forecastLength = WIDE_FORECAST_LENGTH;
            }

            return forecastLength;
        }

        private string GetUpdateTimeText(DateTime now, bool shortFormat)
        {
            string timeformat = now.ToString("h:mm tt").ToLower();

            if (DateFormat.Is24HourFormat(App.Context))
                timeformat = now.ToString("HH:mm").ToLower();

            string updatetime = string.Format("{0} {1}", now.ToString("ddd"), timeformat);

            if (shortFormat)
                return updatetime;
            else
                return string.Format("{0} {1}", mContext.GetString(Resource.String.update_prefix).ToLower(), updatetime);
        }

        private async Task<Weather> GetWeather()
        {
            Weather weather = null;

            try
            {
                if (Settings.FollowGPS)
                    await UpdateLocation();

                var wloader = new WeatherDataLoader(Settings.HomeData);
                await wloader.LoadWeatherData(false);

                weather = wloader.GetWeather();

                if (weather != null)
                {
                    // Re-schedule alarm at selected interval from now
                    UpdateAlarm(App.Context);
                    Settings.UpdateTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, ex.Message);
            }

            return weather;
        }

        private async Task<bool> UpdateLocation()
        {
            bool locationChanged = false;

            if (Settings.FollowGPS)
            {
                if (ContextCompat.CheckSelfPermission(App.Context, Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                    ContextCompat.CheckSelfPermission(App.Context, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
                {
                    return false;
                }

                LocationManager locMan = (LocationManager)App.Context.GetSystemService(Context.LocationService);
                bool isGPSEnabled = locMan.IsProviderEnabled(LocationManager.GpsProvider);
                bool isNetEnabled = locMan.IsProviderEnabled(LocationManager.NetworkProvider);

                Android.Locations.Location location = null;

                if (isGPSEnabled || isNetEnabled)
                {
                    Criteria locCriteria = new Criteria() { Accuracy = Accuracy.Coarse, CostAllowed = false, PowerRequirement = Power.Low };
                    string provider = locMan.GetBestProvider(locCriteria, true);
                    location = locMan.GetLastKnownLocation(provider);

                    if (location != null)
                    {
                        LocationData lastGPSLocData = await Settings.GetLastGPSLocData();

                        // Check previous location difference
                        if (lastGPSLocData.query != null &&
                            Math.Abs(ConversionMethods.CalculateHaversine(lastGPSLocData.latitude, lastGPSLocData.longitude,
                            location.Latitude, location.Longitude)) < 1600)
                        {
                            return false;
                        }

                        LocationQueryViewModel query_vm = null;

                        await Task.Run(async () =>
                        {
                            query_vm = await wm.GetLocation(location);

                            if (String.IsNullOrEmpty(query_vm.LocationQuery))
                                query_vm = new LocationQueryViewModel();
                        });

                        if (String.IsNullOrWhiteSpace(query_vm.LocationQuery))
                        {
                            // Stop since there is no valid query
                            return false;
                        }

                        // Save oldkey
                        string oldkey = lastGPSLocData.query;

                        // Save location as last known
                        lastGPSLocData.SetData(query_vm, location);
                        Settings.SaveLastGPSLocData(lastGPSLocData);

                        App.Context.StartService(
                            new Intent(App.Context, typeof(WearableDataListenerService))
                                .SetAction(WearableDataListenerService.ACTION_SENDLOCATIONUPDATE));

                        // Update widget ids for location
                        if (oldkey != null && WidgetUtils.Exists(oldkey))
                        {
                            WidgetUtils.UpdateWidgetIds(oldkey, lastGPSLocData);
                        }

                        locationChanged = true;
                    }
                }
            }

            return locationChanged;
        }
    }
}