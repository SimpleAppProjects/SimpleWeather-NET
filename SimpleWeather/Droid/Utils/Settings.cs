#if __ANDROID__
using Android.App;
using Android.Content;
using Android.Util;
using Java.IO;
#if __ANDROID_WEAR__
using SimpleWeather.Droid.Wear;
using SimpleWeather.Droid.Wear.Wearable;
#else
using SimpleWeather.Droid.App;
using SimpleWeather.Droid.App.Widgets;
#endif
using SQLite;
using System;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static partial class Settings
    {
        private static readonly String LOG_TAG = nameof(Settings);

        // Shared Settings
        private static ISharedPreferences preferences = App.Preferences;
        private static ISharedPreferencesEditor editor = preferences.Edit();
        private static ISharedPreferences wuSharedPrefs = Application.Context.GetSharedPreferences(WeatherData.WeatherAPI.WeatherUnderground, FileCreationMode.Private);

        // App data files
        private static File appDataFolder = Application.Context.FilesDir;

#if !__ANDROID_WEAR__
        // Android specific settings
        private const string KEY_ONGOINGNOTIFICATION = "key_ongoingnotification";
        private const string KEY_NOTIFICATIONICON = "key_notificationicon";

        public static bool OnGoingNotification { get { return ShowOngoingNotification(); } }
        public static string NotificationIcon { get { return GetNotificationIcon(); } }

        public const string TEMPERATURE_ICON = "0";
        public const string CONDITION_ICON = "1";
#endif

        // Shared Preferences listener
        internal class SettingsListener : Java.Lang.Object, ISharedPreferencesOnSharedPreferenceChangeListener
        {
            public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
            {
                if (String.IsNullOrWhiteSpace(key))
                    return;

                var context = Application.Context;

                switch (key)
                {
                    // Weather Provider changed
                    case KEY_API:
                        WeatherData.WeatherManager.GetInstance().UpdateAPI();
#if !__ANDROID_WEAR__
                        context.StartService(
                            new Intent(context, typeof(WearableDataListenerService))
                                .SetAction(WearableDataListenerService.ACTION_SENDSETTINGSUPDATE));
#endif
                        goto case KEY_USECELSIUS;
                    // FollowGPS changed
                    case KEY_FOLLOWGPS:
#if !__ANDROID_WEAR__
                        context.StartService(
                            new Intent(context, typeof(WearableDataListenerService))
                                .SetAction(WearableDataListenerService.ACTION_SENDSETTINGSUPDATE));
                        context.StartService(
                            new Intent(context, typeof(WearableDataListenerService))
                                .SetAction(WearableDataListenerService.ACTION_SENDLOCATIONUPDATE));
#endif
                        goto case KEY_USECELSIUS;
                    // Settings unit changed
                    case KEY_USECELSIUS:
#if __ANDROID_WEAR__
                        WeatherComplicationIntentService.EnqueueWork(context,
                            new Intent(context, typeof(WeatherComplicationIntentService))
                                .SetAction(WeatherComplicationIntentService.ACTION_UPDATECOMPLICATIONS)
                                .PutExtra(WeatherComplicationIntentService.EXTRA_FORCEUPDATE, true));
                        break;
#else
                        WeatherWidgetService.EnqueueWork(context, new Intent(context, typeof(WeatherWidgetService))
                            .SetAction(WeatherWidgetService.ACTION_UPDATEWEATHER));
                        break;
                    // Refresh interval changed
                    case KEY_REFRESHINTERVAL:
                        WeatherWidgetService.EnqueueWork(context, new Intent(context, typeof(WeatherWidgetService))
                            .SetAction(WeatherWidgetService.ACTION_UPDATEALARM));
                        break;
#endif
#if __ANDROID_WEAR__
                    // Data sync setting changed
                    case KEY_DATASYNC:
                        // Reset UpdateTime value to force a refresh
                        UpdateTime = DateTime.MinValue;
                        break;
#endif
                    default:
                        break;
                }
            }
        }

        // Initialize file
        private static void Init()
        {
#if !__ANDROID_WEAR__
            if (locationDB == null)
            {
                locationDB = new SQLiteAsyncConnection(
                   System.IO.Path.Combine(appDataFolder.Path, "locations.db"));
            }
#endif

            if (weatherDB == null)
            {
                weatherDB = new SQLiteAsyncConnection(
                   System.IO.Path.Combine(appDataFolder.Path, "weatherdata.db"));
            }

            preferences.RegisterOnSharedPreferenceChangeListener(App.SharedPreferenceListener);
        }

        private static string GetTempUnit()
        {
            if (!preferences.Contains(KEY_USECELSIUS))
            {
                return Fahrenheit;
            }
            else if (preferences.GetBoolean(KEY_USECELSIUS, false))
            {
                return Celsius;
            }

            return Fahrenheit;
        }

        private static void SetTempUnit(string value)
        {
            if (value == Celsius)
                editor.PutBoolean(KEY_USECELSIUS, true);
            else
                editor.PutBoolean(KEY_USECELSIUS, false);
        }

        private static bool IsWeatherLoaded()
        {
#if !__ANDROID_WEAR__
            if (!Task.Run(() => DBUtils.LocationDataExists(locationDB)).Result)
            {
#endif
                if (!Task.Run(() => DBUtils.WeatherDataExists(weatherDB)).Result)
                {
                    SetWeatherLoaded(false);
                    return false;
                }
#if !__ANDROID_WEAR__
            }
#endif

            if (preferences.Contains(KEY_WEATHERLOADED) && preferences.GetBoolean(KEY_WEATHERLOADED, false))
            {
                SetWeatherLoaded(true);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void SetWeatherLoaded(bool isLoaded)
        {
            editor.PutBoolean(KEY_WEATHERLOADED, isLoaded);
            editor.Commit();
        }

        private static string GetAPI()
        {
            if (!preferences.Contains(KEY_API))
            {
                SetAPI(WeatherData.WeatherAPI.WeatherUnderground);
                return WeatherData.WeatherAPI.WeatherUnderground;
            }
            else
            {
                return preferences.GetString(KEY_API, null);
            }
        }

        private static void SetAPI(string value)
        {
            editor.PutString(KEY_API, value);
            editor.Commit();
        }

        private static string GetAPIKEY()
        {
            if (!preferences.Contains(KEY_APIKEY))
            {
                String key;
                key = ReadAPIKEYfile();

                if (!String.IsNullOrWhiteSpace(key))
                    SetAPIKEY(key);

                return key;
            }
            else
            {
                return preferences.GetString(KEY_APIKEY, null);
            }
        }

        private static string ReadAPIKEYfile()
        {
            // Read key from file
            String key = String.Empty;
            BufferedReader reader = null;

            try
            {
                reader = new BufferedReader(
                            new InputStreamReader(App.Context.Assets.Open("API_KEY.txt")));

                key = reader.ReadLine();
            }
            catch (Exception e)
            {
                Log.WriteLine(LogPriority.Error, LOG_TAG, e.StackTrace);
            }
            finally
            {
                if (reader != null)
                {
                    try
                    {
                        reader.Close();
                    }
                    catch (Exception e)
                    {
                        Log.WriteLine(LogPriority.Error, LOG_TAG, e.StackTrace);
                    }
                }
            }

            return key;
        }

        private static void SetAPIKEY(string key)
        {
            editor.PutString(KEY_APIKEY, key);
            editor.Commit();
        }

        private static bool UseFollowGPS()
        {
            if (!preferences.Contains(KEY_FOLLOWGPS))
            {
                SetFollowGPS(false);
                return false;
            }
            else
                return preferences.GetBoolean(KEY_FOLLOWGPS, false);
        }

        private static void SetFollowGPS(bool value)
        {
            editor.PutBoolean(KEY_FOLLOWGPS, value);
            editor.Commit();
        }

        private static string GetLastGPSLocation()
        {
            return preferences.GetString(KEY_LASTGPSLOCATION, null);
        }

        private static void SetLastGPSLocation(string value)
        {
            editor.PutString(KEY_LASTGPSLOCATION, value);
            editor.Commit();
        }

        private static DateTime GetUpdateTime()
        {
            if (!preferences.Contains(KEY_UPDATETIME))
                return DateTime.MinValue;
            else
                return DateTime.Parse(preferences.GetString(KEY_UPDATETIME, DateTime.MinValue.ToString()));
        }

        private static void SetUpdateTime(DateTime value)
        {
            editor.PutString(KEY_UPDATETIME, value.ToString());
            editor.Commit();
        }

#if !__ANDROID_WEAR__
        private static int GetRefreshInterval()
        {
            return int.Parse(preferences.GetString(KEY_REFRESHINTERVAL, DEFAULT_UPDATE_INTERVAL));
        }

        private static void SetRefreshInterval(int value)
        {
            editor.PutString(KEY_REFRESHINTERVAL, value.ToString());
            editor.Commit();
        }

        private static bool ShowOngoingNotification()
        {
            if (!preferences.Contains(KEY_ONGOINGNOTIFICATION))
            {
                return false;
            }
            else
                return preferences.GetBoolean(KEY_ONGOINGNOTIFICATION, false);
        }

        private static string GetNotificationIcon()
        {
            if (!preferences.Contains(KEY_NOTIFICATIONICON))
            {
                return TEMPERATURE_ICON;
            }
            else
                return preferences.GetString(KEY_NOTIFICATIONICON, TEMPERATURE_ICON);
        }

        private static bool UseAlerts()
        {
            if (!preferences.Contains(KEY_USEALERTS))
            {
                SetAlerts(false);
                return false;
            }
            else
                return preferences.GetBoolean(KEY_USEALERTS, false);
        }

        private static void SetAlerts(bool value)
        {
            editor.PutBoolean(KEY_USEALERTS, value);
            editor.Commit();
        }
#endif

        private static int GetDBVersion()
        {
            return int.Parse(preferences.GetString(KEY_DBVERSION, "0"));
        }

        private static void SetDBVersion(int value)
        {
            editor.PutString(KEY_DBVERSION, value.ToString());
            editor.Commit();
        }

        private static bool IsKeyVerified()
        {
            if (!wuSharedPrefs.Contains(KEY_APIKEY_VERIFIED))
            {
                return false;
            }
            else
                return wuSharedPrefs.GetBoolean(KEY_APIKEY_VERIFIED, false);
        }

        private static void SetKeyVerified(bool value)
        {
            var wuEditor = wuSharedPrefs.Edit();
            editor.PutBoolean(KEY_APIKEY_VERIFIED, value);
            editor.Apply();

            if (!value)
                wuSharedPrefs.Edit().Remove(KEY_APIKEY_VERIFIED).Apply();
        }
    }
}
#endif