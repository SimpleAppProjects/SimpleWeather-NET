using Android.Content;
using Android.Util;
using Java.IO;
using SimpleWeather.Droid;
using System;

namespace SimpleWeather.Utils
{
    public static partial class Settings
    {
        private static String LOG_TAG = "Settings";

        // Shared Settings
        private static ISharedPreferences preferences = App.Preferences;
        private static ISharedPreferencesEditor editor = preferences.Edit();
        private static ISharedPreferencesOnSharedPreferenceChangeListener listener = new SettingsListener();

        // App data files
        private static File appDataFolder = App.Context.FilesDir;
        private static File locDataFile;
        private static File dataFile;

        // Android specific settings
        public static bool OnGoingNotification { get { return ShowOngoingNotification(); } }
        private const string KEY_ONGOINGNOTIFICATION = "key_ongoingnotification";

        // Shared Preferences listener
        internal class SettingsListener : Java.Lang.Object, ISharedPreferencesOnSharedPreferenceChangeListener
        {
            public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
            {
                if (String.IsNullOrWhiteSpace(key))
                    return;

                Context context = App.Context;

                switch (key)
                {
                    // FollowGPS changed
                    case KEY_FOLLOWGPS:
                    // Weather Provider changed
                    case KEY_API:
                    // Settings unit changed
                    case KEY_USECELSIUS:
                        context.StartService(new Intent(context, typeof(Droid.Widgets.WeatherWidgetService))
                            .SetAction(Droid.Widgets.WeatherWidgetService.ACTION_UPDATEWEATHER));
                        break;
                    // Refresh interval changed
                    case KEY_REFRESHINTERVAL:
                        context.StartService(new Intent(context, typeof(Droid.Widgets.WeatherWidgetService))
                            .SetAction(Droid.Widgets.WeatherWidgetService.ACTION_UPDATEALARM));
                        break;
                    default:
                        break;
                }
            }
        }

        // Initialize file
        private static void Init()
        {
            if (dataFile == null)
                dataFile = new File(appDataFolder, "data.json");

            if (!dataFile.Exists())
                dataFile.CreateNewFile();

            if (locDataFile == null)
                locDataFile = new File(appDataFolder, "locations.json");

            if (!locDataFile.Exists())
                locDataFile.CreateNewFile();

            preferences.RegisterOnSharedPreferenceChangeListener(listener);
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
            System.IO.FileInfo dataFileinfo = new System.IO.FileInfo(dataFile.Path);
            System.IO.FileInfo locFileinfo = new System.IO.FileInfo(locDataFile.Path);

            if (!dataFileinfo.Exists || (dataFileinfo.Exists && dataFileinfo.Length == 0))
            {
                if (!locFileinfo.Exists || (locFileinfo.Exists && locFileinfo.Length == 0))
                {
                    SetWeatherLoaded(false);
                    locationData.Clear();
                    weatherData.Clear();
                    return false;
                }
                else if (locationData.Count > 0)
                {
                    SetWeatherLoaded(true);
                    return true;
                }
            }

            if (weatherData.Count > 0 || locationData.Count > 0)
            {
                SetWeatherLoaded(true);
                return true;
            }
            else if (preferences.Contains(KEY_WEATHERLOADED) && preferences.GetBoolean(KEY_WEATHERLOADED, false))
            {
                SetWeatherLoaded(true);
                return true;
            }
            else
            {
                locationData.Clear();
                weatherData.Clear();
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
                SetAPI(API_WUnderground);
                return API_WUnderground;
            }
            else
                return preferences.GetString(KEY_API, null);
        }

        private static void SetAPI(string value)
        {
            editor.PutString(KEY_API, value);
            editor.Commit();
        }

        #region WeatherUnderground
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
                return preferences.GetString(KEY_APIKEY, null);
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
            if (!String.IsNullOrWhiteSpace(key))
                editor.PutString(KEY_APIKEY, key);

            editor.Commit();
        }
        #endregion

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

        private static int GetRefreshInterval()
        {
            return int.Parse(preferences.GetString(KEY_REFRESHINTERVAL, DEFAULT_UPDATE_INTERVAL));
        }

        private static void SetRefreshInterval(int value)
        {
            editor.PutString(KEY_REFRESHINTERVAL, value.ToString());
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

        private static bool ShowOngoingNotification()
        {
            if (!preferences.Contains(KEY_ONGOINGNOTIFICATION))
            {
                return false;
            }
            else
                return preferences.GetBoolean(KEY_ONGOINGNOTIFICATION, false);
        }
    }
}
