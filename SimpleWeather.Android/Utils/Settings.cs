using Android.Content;
using Java.IO;
using SimpleWeather.Droid;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static partial class Settings
    {
        // Shared Settings
        private static ISharedPreferences preferences = App.Preferences;
        private static ISharedPreferencesEditor editor = preferences.Edit();

        // App data files
        private static File appDataFolder = App.Context.FilesDir;
        private static File dataFile;

        private static void init()
        {
            if (dataFile == null)
                dataFile = new File(appDataFolder, "data.json");

            if (!dataFile.Exists())
                dataFile.CreateNewFile();
        }

        private static string getTempUnit()
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

        private static void setTempUnit(string value)
        {
            if (value == Celsius)
                editor.PutBoolean(KEY_USECELSIUS, true);
            else
                editor.PutBoolean(KEY_USECELSIUS, false);
        }

        private static bool isWeatherLoaded()
        {
            System.IO.FileInfo fileinfo = new System.IO.FileInfo(dataFile.Path);

            if (!fileinfo.Exists || (fileinfo.Exists && fileinfo.Length == 0))
            {
                setWeatherLoaded(false);
                return false;
            }

            return preferences.Contains(KEY_WEATHERLOADED) && preferences.GetBoolean(KEY_WEATHERLOADED, false);
        }

        private static void setWeatherLoaded(bool isLoaded)
        {
            editor.PutBoolean(KEY_WEATHERLOADED, isLoaded);
            editor.Commit();
        }

        private static string getAPI()
        {
            if (!preferences.Contains(KEY_API))
            {
                setAPI("WUnderground");
                return "WUnderground";
            }
            else
                return preferences.GetString(KEY_API, null);
        }

        private static void setAPI(string value)
        {
            editor.PutString(KEY_API, value);
            editor.Commit();
        }

        #region WeatherUnderground
        private static string getAPIKEY()
        {
            if (!preferences.Contains(KEY_APIKEY))
            {
                String key;
                key = readAPIKEYfile();

                if (!String.IsNullOrWhiteSpace(key))
                    setAPIKEY(key);

                return key;
            }
            else
                return preferences.GetString(KEY_APIKEY, null);
        }

        private static string readAPIKEYfile()
        {
            // Read key from file
            String key = "";
            BufferedReader reader = null;

            try
            {
                reader = new BufferedReader(
                        new InputStreamReader(App.Context.Assets.Open("API_KEY.txt")));

                key = reader.ReadLine();
            }
            catch (Exception e)
            {
                Android.Util.Log.WriteLine(Android.Util.LogPriority.Error, "Settings", e.StackTrace);
            }
            finally
            {
                if (reader != null)
                    try
                    {
                        reader.Close();
                    }
                    catch (Exception e)
                    {
                        Android.Util.Log.WriteLine(Android.Util.LogPriority.Error, "Settings", e.StackTrace);
                    }
            }

            return key;
        }

        private static void setAPIKEY(string key)
        {
            if (!String.IsNullOrWhiteSpace(key))
                editor.PutString(KEY_APIKEY, key);

            editor.Commit();
        }
        #endregion
    }
}
