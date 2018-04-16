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

namespace SimpleWeather.Droid.App.Widgets
{
    public static class WidgetUtils
    {
        // Shared Settings
        private static ISharedPreferences widgetPrefs = Application.Context.GetSharedPreferences("widgets", FileCreationMode.Private);
        private static ISharedPreferencesEditor editor = widgetPrefs.Edit();

        public static void Init()
        {
            // First time, so load all current widgets under Home location
            if (Settings.WeatherLoaded && widgetPrefs.All.Count == 0)
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
                    SaveIds(location_query, idList);
                }
            }
        }

        public static void UpdateWidgetIds(String oldQuery, String newQuery)
        {
            String listJson = widgetPrefs.GetString(oldQuery, String.Empty);
            widgetPrefs.Edit().Remove(oldQuery);
            widgetPrefs.Edit().PutString(newQuery, listJson).Commit();
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
    }
}