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

        // App data files
        private static File appDataFolder = App.Context.FilesDir;
        private static File dataFile;

        // Initialize file
        private static void Init()
        {
            if (dataFile == null)
                dataFile = new File(appDataFolder, "data.json");

            if (!dataFile.Exists())
                dataFile.CreateNewFile();
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
            System.IO.FileInfo fileinfo = new System.IO.FileInfo(dataFile.Path);

            if (!fileinfo.Exists || (fileinfo.Exists && fileinfo.Length == 0))
            {
                SetWeatherLoaded(false);
                return false;
            }

            return preferences.Contains(KEY_WEATHERLOADED) && preferences.GetBoolean(KEY_WEATHERLOADED, false);
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

        private static string GetHomeLocation()
        {
            return preferences.GetString(KEY_HOMELOCATION, null);
        }

        private static void SetHomeLocation(string value)
        {
            editor.PutString(KEY_HOMELOCATION, value);
            editor.Commit();
        }
    }
}
