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
        private static File dataFile = null;

        private static string getTempUnit()
        {
            if (!preferences.Contains("key_usecelsius"))
            {
                return Fahrenheit;
            }
            else if (preferences.GetBoolean("key_usecelsius", false))
            {
                return Celsius;
            }

            return Fahrenheit;
        }

        private static bool isWeatherLoaded()
        {
            if (dataFile == null)
                dataFile = new File(appDataFolder, "data.json");

            System.IO.FileInfo fileinfo = new System.IO.FileInfo(dataFile.Path);

            if (!fileinfo.Exists || (fileinfo.Exists && fileinfo.Length == 0))
            {
                setWeatherLoaded(false);
                return false;
            }

            return preferences.Contains("weatherLoaded") && preferences.GetBoolean("weatherLoaded", false);
        }

        private static void setWeatherLoaded(bool isLoaded)
        {
            if (isLoaded)
                editor.PutBoolean("weatherLoaded", true);
            else
                editor.PutBoolean("weatherLoaded", false);

            editor.Commit();
        }

        private static string getAPI()
        {
            if (!preferences.Contains("API"))
            {
                setAPI("WUnderground");
                return "WUnderground";
            }
            else
                return preferences.GetString("API", null);
        }

        private static void setAPI(string value)
        {
            editor.PutString("API", value);
            editor.Commit();
        }

        public static async Task<List<string>> getLocations()
        {
            OrderedDictionary dict = await getWeatherData();
            List<string> locations = new List<string>();
            foreach (string location in dict.Keys)
            {
                locations.Add(location);
            }

            return locations;
        }

        public static async Task<OrderedDictionary> getWeatherData()
        {
            if (dataFile == null)
                dataFile = new File(appDataFolder, "data.json");

            System.IO.FileInfo fileinfo = new System.IO.FileInfo(dataFile.Path);

            if (!fileinfo.Exists || (fileinfo.Exists && fileinfo.Length == 0))
                return new OrderedDictionary();

            return (OrderedDictionary)JSONParser.Deserializer(await FileUtils.ReadFile(dataFile), typeof(OrderedDictionary));
        }

        public static void saveWeatherData(OrderedDictionary weatherData)
        {
            if (dataFile == null)
                dataFile = new File(appDataFolder, "data.json");

            JSONParser.Serializer(weatherData, dataFile);
        }

        #region WeatherUnderground
        private static string getAPIKEY()
        {
            if (!preferences.Contains("API_KEY"))
            {
                String key;
                key = readAPIKEYfile();

                if (!String.IsNullOrWhiteSpace(key))
                    setAPIKEY(key);

                return key;
            }
            else
                return preferences.GetString("API_KEY", null);
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
                editor.PutString("API_KEY", key);

            editor.Commit();
        }
        #endregion
    }
}
