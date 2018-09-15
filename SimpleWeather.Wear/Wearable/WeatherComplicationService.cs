using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Graphics.Drawables;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.Wearable.Complications;
using Android.Views;
using Android.Widget;
using SimpleWeather.Controls;
using SimpleWeather.Droid.Helpers;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Droid.Wear.Wearable
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class WeatherComplicationBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            // Relay intent to WeatherComplicationIntentService
            intent.SetClass(context, Java.Lang.Class.FromType(typeof(WeatherComplicationIntentService)));
            WeatherComplicationIntentService.EnqueueWork(context, intent);
        }
    }

    [Service(Exported = false, Enabled = true, Permission = "android.permission.BIND_JOB_SERVICE")]
    public class WeatherComplicationIntentService : JobIntentService
    {
        private const string TAG = nameof(WeatherComplicationIntentService);

        public const string ACTION_UPDATECOMPLICATIONS = "SimpleWeather.Droid.Wear.action.UPDATE_COMPLICATIONS";
        public const string ACTION_STARTALARM = "SimpleWeather.Droid.Wear.action.START_ALARM";
        public const string ACTION_CANCELALARM = "SimpleWeather.Droid.Wear.action.CANCEL_ALARM";

        public const string EXTRA_FORCEUPDATE = "SimpleWeather.Droid.Wear.extra.FORCE_UPDATE";

        private Context mContext;
        private ProviderUpdateRequester updateRequester;
        private static bool alarmStarted = false;

        private const int JOB_ID = 1000;

        public static void EnqueueWork(Context context, Intent work)
        {
            EnqueueWork(context,
                Java.Lang.Class.FromType(typeof(WeatherComplicationIntentService)),
                JOB_ID, work);
        }

        private bool ComplicationsExist => WeatherComplicationService.ComplicationsExist();

        public override void OnCreate()
        {
            base.OnCreate();

            mContext = ApplicationContext;
            updateRequester = new ProviderUpdateRequester(mContext,
                new ComponentName(mContext, Java.Lang.Class.FromType(typeof(WeatherComplicationService))));

            var oldHandler = Java.Lang.Thread.DefaultUncaughtExceptionHandler;

            Java.Lang.Thread.DefaultUncaughtExceptionHandler =
                new UncaughtExceptionHandler((thread, throwable) =>
                {
                    Logger.WriteLine(LoggerLevel.Error, throwable, "SimpleWeather: {0}: UncaughtException", TAG);

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
            if (ACTION_UPDATECOMPLICATIONS.Equals(intent?.Action))
            {
                bool force = intent.GetBooleanExtra(EXTRA_FORCEUPDATE, false);

                if ((DateTime.Now - WeatherComplicationService.UpdateTime).TotalMinutes > Settings.DefaultInterval)
                    force = true;

                if (force)
                {
                    // Request updates
                    updateRequester.RequestUpdateAll();
                    UpdateAlarm(App.Context);
                }
            }
            else if (ACTION_STARTALARM.Equals(intent?.Action))
            {
                StartAlarm(App.Context);
            }
            else if (ACTION_CANCELALARM.Equals(intent?.Action))
            {
                CancelAlarms(App.Context);
            }
        }

        private PendingIntent GetAlarmIntent(Context context)
        {
            Intent intent = new Intent(context, typeof(WeatherComplicationBroadcastReceiver))
                .SetAction(ACTION_UPDATECOMPLICATIONS);

            return PendingIntent.GetBroadcast(context, 0, intent, 0);
        }

        private void UpdateAlarm(Context context)
        {
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            int interval = Settings.DefaultInterval;

            bool startNow = !alarmStarted;
            long intervalMillis = (long)TimeSpan.FromMinutes(interval).TotalMilliseconds;
            long triggerAtTime = SystemClock.ElapsedRealtime() + intervalMillis;

            if (startNow)
            {
                EnqueueWork(context, new Intent(context, typeof(WeatherComplicationIntentService))
                    .SetAction(ACTION_UPDATECOMPLICATIONS));
            }

            PendingIntent pendingIntent = GetAlarmIntent(context);
            am.Cancel(pendingIntent);
            am.SetInexactRepeating(AlarmType.ElapsedRealtime, triggerAtTime, intervalMillis, pendingIntent);
            alarmStarted = true;

            Logger.WriteLine(LoggerLevel.Info, "{0}: Updated alarm", TAG);
        }

        private void CancelAlarms(Context context)
        {
            // Cancel alarm if dependent features are turned off
            if (!ComplicationsExist)
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
            if (!alarmStarted && ComplicationsExist)
            {
                UpdateAlarm(context);
                alarmStarted = true;
            }
        }
    }

    [Service(Icon = "@drawable/day_sunny", Label = "Weather",
             Permission = "com.google.android.wearable.permission.BIND_COMPLICATION_PROVIDER")]
    [IntentFilter(new string[]
    {
        ActionComplicationUpdateRequest
    })]
    [MetaData(MetadataKeySupportedTypes, Value = "SHORT_TEXT,LONG_TEXT")]
    [MetaData(MetadataKeyUpdatePeriodSeconds, Value = "0")]
    public class WeatherComplicationService : ComplicationProviderService
    {
        private const string TAG = nameof(WeatherComplicationService);

        private Context mContext;

        private WeatherManager wm;
        private static Weather weather;
        private static LocationData locData;

        private static List<int> complicationIds;

        internal static DateTime UpdateTime = DateTime.MinValue;

        static WeatherComplicationService()
        {
            if (complicationIds == null)
                complicationIds = new List<int>();
        }

        public override void OnCreate()
        {
            base.OnCreate();

            mContext = ApplicationContext;
            wm = WeatherManager.GetInstance();

            if (complicationIds == null)
                complicationIds = new List<int>();

            var oldHandler = Java.Lang.Thread.DefaultUncaughtExceptionHandler;

            Java.Lang.Thread.DefaultUncaughtExceptionHandler =
                new UncaughtExceptionHandler((thread, throwable) =>
                {
                    Logger.WriteLine(LoggerLevel.Error, throwable, "SimpleWeather: {0}: UncaughtException", TAG);

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

        internal static int[] GetComplicationIds()
        {
            if (complicationIds == null)
                return new int[0];
            else
                return complicationIds.ToArray();
        }

        private void StartAlarm(Context context)
        {
            // Tell service to start alarm
            WeatherComplicationIntentService.EnqueueWork(context,
                new Intent(context, typeof(WeatherComplicationIntentService))
                    .SetAction(WeatherComplicationIntentService.ACTION_STARTALARM));
        }

        private void CancelAlarm(Context context)
        {
            // Tell service to stop alarms
            WeatherComplicationIntentService.EnqueueWork(context,
                new Intent(context, typeof(WeatherComplicationIntentService))
                    .SetAction(WeatherComplicationIntentService.ACTION_CANCELALARM));
        }

        public override void OnComplicationActivated(int complicationId, int type, ComplicationManager manager)
        {
            base.OnComplicationActivated(complicationId, type, manager);
            complicationIds.Add(complicationId);

            StartAlarm(App.Context);
        }

        public override void OnComplicationDeactivated(int complicationId)
        {
            base.OnComplicationDeactivated(complicationId);
            complicationIds.Remove(complicationId);

            CancelAlarm(App.Context);
        }

        internal static bool ComplicationsExist()
        {
            if (complicationIds == null)
                return false;
            else
                return complicationIds.Count > 0;
        }

        public override void OnComplicationUpdate(int complicationId, int type, ComplicationManager manager)
        {
            Task.Run(async () =>
            {
                ComplicationData complicationData = null;

                if (Settings.WeatherLoaded)
                {
                    weather = await GetWeather();
                    complicationData = BuildUpdate(type, weather);
                }

                if (complicationData != null)
                {
                    manager.UpdateComplicationData(complicationId, complicationData);
                    UpdateTime = DateTime.Now;
                }
                else
                {
                    // If no data is sent, we still need to inform the ComplicationManager, so
                    // the update job can finish and the wake lock isn't held any longer.
                    manager.NoUpdateRequired(complicationId);
                }

                // Add id to list in case it wasn't before
                if (complicationIds.Count == 0)
                {
                    complicationIds.Add(complicationId);
                    StartAlarm(App.Context);
                }
            });
        }

        private PendingIntent GetTapIntent(Context context)
        {
            Intent onClickIntent = new Intent(context.ApplicationContext, typeof(LaunchActivity))
                .SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
            return PendingIntent.GetActivity(context, 0, onClickIntent, 0);
        }

        private ComplicationData BuildUpdate(int dataType, Weather weather)
        {
            if (weather?.IsValid() == false || (dataType != ComplicationData.TypeShortText && dataType != ComplicationData.TypeLongText))
            {
                return null;
            }
            else
            {
                // Temperature
                string temp = Settings.IsFahrenheit ?
                    Math.Round(weather.condition.temp_f) + "º" : Math.Round(weather.condition.temp_c) + "º";
                // Weather Icon
                int weatherIcon = wm.GetWeatherIconResource(weather.condition.icon);
                // Condition text
                string condition = weather.condition.weather;

                var builder = new ComplicationData.Builder(dataType);
                if (dataType == ComplicationData.TypeShortText)
                {
                    builder.SetShortText(ComplicationText.PlainText(temp));
                }
                else if (dataType == ComplicationData.TypeLongText)
                {
                    builder.SetLongText(ComplicationText.PlainText(String.Format("{0}: {1}", condition, temp)));
                }

                builder.SetIcon(Icon.CreateWithResource(this, weatherIcon))
                       .SetTapAction(GetTapIntent(this));

                return builder.Build();
            }
        }

        private async Task<Weather> GetWeather()
        {
            Weather weather = null;

            try
            {
                if (Settings.DataSync == WearableDataSync.Off && Settings.FollowGPS)
                    await UpdateLocation();

                var wloader = new WeatherDataLoader(Settings.HomeData);

                if (Settings.DataSync == WearableDataSync.Off)
                    await wloader.LoadWeatherData(false);
                else
                    await wloader.ForceLoadSavedWeatherData();

                locData = Settings.HomeData;
                weather = wloader.GetWeather();
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: {0}: GetWeather: error", TAG);
            }

            return weather;
        }

        private async Task<bool> UpdateLocation()
        {
            bool locationChanged = false;

            if (Settings.FollowGPS)
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    return false;
                }

                Android.Locations.Location location = null;

                if (WearableHelper.IsGooglePlayServicesInstalled && !WearableHelper.HasGPS)
                {
                    var mFusedLocationClient = new FusedLocationProviderClient(this);
                    location = await mFusedLocationClient.GetLastLocationAsync();
                }
                else
                {
                    LocationManager locMan = GetSystemService(Context.LocationService) as LocationManager;
                    bool isGPSEnabled = (bool)locMan?.IsProviderEnabled(LocationManager.GpsProvider);
                    bool isNetEnabled = (bool)locMan?.IsProviderEnabled(LocationManager.NetworkProvider);

                    if (isGPSEnabled || isNetEnabled)
                    {
                        Criteria locCriteria = new Criteria() { Accuracy = Accuracy.Coarse, CostAllowed = false, PowerRequirement = Power.Low };
                        string provider = locMan.GetBestProvider(locCriteria, true);
                        location = locMan.GetLastKnownLocation(provider);
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.error_retrieve_location, ToastLength.Short).Show();
                    }
                }

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

                    LocationQueryViewModel view = null;

                    await Task.Run(async () =>
                    {
                        view = await wm.GetLocation(location);

                        if (String.IsNullOrEmpty(view.LocationQuery))
                            view = new LocationQueryViewModel();
                    });

                    if (String.IsNullOrWhiteSpace(view.LocationQuery))
                    {
                        // Stop since there is no valid query
                        return false;
                    }

                    // Save location as last known
                    lastGPSLocData.SetData(view, location);
                    Settings.SaveHomeData(lastGPSLocData);

                    locationChanged = true;
                }
            }

            return locationChanged;
        }
    }
}