using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SimpleWeather.Utils;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Droid.App.Widgets
{
    public static class WidgetUtils
    {
        // Shared Settings
        private static ISharedPreferences widgetPrefs = Application.Context.GetSharedPreferences("appwidgets", FileCreationMode.Private);
        private static ISharedPreferencesEditor editor = widgetPrefs.Edit();

        // Widget Prefs
        private const int CurrentPrefsVersion = 0;

        // Keys
        private const String KEY_VERSION = "key_version";
        private const String KEY_WEATHERDATA = "key_weatherdata";
        private const String KEY_LOCATIONDATA = "key_locationdata";
        private const String KEY_LOCATIONQUERY = "key_locationquery";

        public static void Init()
        {
            // Check prefs
            if (GetVersion() < CurrentPrefsVersion)
            {
                switch (GetVersion())
                {
                    case -1:
                        // First time, so load all current widgets under Home location
                        if (Settings.WeatherLoaded)
                        {
                            var mAppWidgetManager = AppWidgetManager.GetInstance(App.Context);
                            var mAppWidget1x1 = WeatherWidgetProvider1x1.GetInstance();
                            var mAppWidget2x2 = WeatherWidgetProvider2x2.GetInstance();
                            var mAppWidget4x1 = WeatherWidgetProvider4x1.GetInstance();
                            var mAppWidget4x2 = WeatherWidgetProvider4x2.GetInstance();

                            var currentIds = new List<int>();
                            currentIds.AddRange(mAppWidgetManager.GetAppWidgetIds(mAppWidget1x1.ComponentName));
                            currentIds.AddRange(mAppWidgetManager.GetAppWidgetIds(mAppWidget2x2.ComponentName));
                            currentIds.AddRange(mAppWidgetManager.GetAppWidgetIds(mAppWidget4x1.ComponentName));
                            currentIds.AddRange(mAppWidgetManager.GetAppWidgetIds(mAppWidget4x2.ComponentName));

                            var homeLocation = Settings.HomeData;
                            SaveIds(homeLocation.query, currentIds);
                        }
                        break;
                }

                // Set to latest version
                SetVersion(CurrentPrefsVersion);
            }
        }

        private static int GetVersion()
        {
            return int.Parse(widgetPrefs.GetString(KEY_VERSION, "-1"));
        }

        private static void SetVersion(int value)
        {
            editor.PutString(KEY_VERSION, value.ToString());
            editor.Commit();
        }

        public static void AddWidgetId(String location_query, int widgetId)
        {
            String listJson = widgetPrefs.GetString(location_query, String.Empty);
            if (String.IsNullOrWhiteSpace(listJson))
            {
                var newlist = new List<int>() { widgetId };
                SaveIds(location_query, newlist);
            }
            else
            {
                var idList = JSONParser.Deserializer<List<int>>(listJson);
                if (idList != null && !idList.Contains(widgetId))
                {
                    idList.Add(widgetId);
                    SaveIds(location_query, idList);
                }
            }
        }

        public static void RemoveWidgetId(String location_query, int widgetId)
        {
            String listJson = widgetPrefs.GetString(location_query, String.Empty);
            if (!String.IsNullOrWhiteSpace(listJson))
            {
                var idList = JSONParser.Deserializer<List<int>>(listJson);
                if (idList != null)
                {
                    idList.Remove(widgetId);

                    if (idList.Count == 0)
                        widgetPrefs.Edit().Remove(location_query).Commit();
                    else
                        SaveIds(location_query, idList);
                }
            }
            DeletePreferences(widgetId);
        }

        private static void DeletePreferences(int widgetId)
        {
            GetEditor(widgetId).Clear().Commit();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                Application.Context.DeleteSharedPreferences(String.Format("appwidget_{0}", widgetId));
            }
            else
            {
                var parentPath = Application.Context.FilesDir.Parent;
                var sharedPrefsPath = String.Format("{0}/shared_prefs/appwidget_{1}.xml", parentPath, widgetId);
                Java.IO.File sharedPrefsFile = new Java.IO.File(sharedPrefsPath);

                if (sharedPrefsFile.Exists() &&
                    sharedPrefsFile.CanWrite() && sharedPrefsFile.ParentFile.CanWrite())
                {
                    sharedPrefsFile.Delete();
                }
            }
        }

        public static void UpdateWidgetIds(String oldQuery, LocationData newLocation)
        {
            String listJson = widgetPrefs.GetString(oldQuery, String.Empty);
            widgetPrefs.Edit().Remove(oldQuery);
            widgetPrefs.Edit().PutString(newLocation.query, listJson).Commit();

            foreach (int id in GetWidgetIds(newLocation.query))
            {
                SaveLocationData(id, newLocation);
            }
        }

        public static int[] GetWidgetIds(String location_query)
        {
            String listJson = widgetPrefs.GetString(location_query, String.Empty);
            if (!String.IsNullOrWhiteSpace(listJson))
            {
                var idList = JSONParser.Deserializer<List<int>>(listJson);
                if (idList != null)
                {
                    return idList.ToArray();
                }
            }

            return new List<int>().ToArray();
        }

        public static bool Exists(String location_query)
        {
            String listJson = widgetPrefs.GetString(location_query, String.Empty);
            if (!String.IsNullOrWhiteSpace(listJson))
            {
                var idList = JSONParser.Deserializer<List<int>>(listJson);
                if (idList != null && idList.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool SaveIds(String key, List<int> idList)
        {
            return widgetPrefs.Edit()
                    .PutString(key, JSONParser.Serializer(idList, typeof(List<int>)))
                    .Commit();
        }

        public static ISharedPreferences GetPreferences(int appWidgetId)
        {
            return Application.Context.GetSharedPreferences(String.Format("appwidget_{0}", appWidgetId), FileCreationMode.Private);
        }

        public static ISharedPreferencesEditor GetEditor(int appWidgetId)
        {
            return GetPreferences(appWidgetId).Edit();
        }

        public static void SaveAllData(int appWidgetId, LocationData location, Weather weather)
        {
            var editor = GetEditor(appWidgetId);

            var locJson = location?.ToJson();
            var weatherJson = weather?.ToJson();

            if (locJson != null)
                editor.PutString(KEY_LOCATIONDATA, locJson);
            if (weatherJson != null)
                editor.PutString(KEY_WEATHERDATA, weatherJson);
            editor.Commit();
        }

        public static void SaveLocationData(int appWidgetId, LocationData location)
        {
            var editor = GetEditor(appWidgetId);

            var locJson = location?.ToJson();

            if (locJson != null)
                editor.PutString(KEY_LOCATIONDATA, locJson);
            editor.Commit();
        }

        public static void SaveWeatherData(int appWidgetId, Weather weather)
        {
            var editor = GetEditor(appWidgetId);

            var weatherJson = weather?.ToJson();

            if (weatherJson != null)
                editor.PutString(KEY_WEATHERDATA, weatherJson);
            editor.Commit();
        }

        public static LocationData GetLocationData(int appWidgetId)
        {
            var prefs = GetPreferences(appWidgetId);
            var locDataJson = prefs.GetString(KEY_LOCATIONDATA, null);

            if (String.IsNullOrWhiteSpace(locDataJson))
            {
                return null;
            }
            else
            {
                return LocationData.FromJson(
                    new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(locDataJson)));
            }
        }

        public static Weather GetWeatherData(int appWidgetId)
        {
            var prefs = GetPreferences(appWidgetId);
            var weatherDataJson = prefs.GetString(KEY_WEATHERDATA, null);

            if (String.IsNullOrWhiteSpace(weatherDataJson))
            {
                return null;
            }
            else
            {
                return Weather.FromJson(
                    new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(weatherDataJson)));
            }
        }
    }
}