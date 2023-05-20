using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Windows.Widgets.Providers;
using SimpleWeather.NET.Widgets.Templates;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using System.Collections.Immutable;
using Windows.Storage;

namespace SimpleWeather.NET.Widgets
{
    public static class WidgetUtils
    {
        // Shared Settings
        private static readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private static readonly ApplicationDataContainer widgetContainer = localSettings.CreateContainer("AppWidgets", ApplicationDataCreateDisposition.Always);

        private const int CURRENT_VERSION = 1;

        // Keys
        private const string KEY_VERSION = "key_version";
        private const string KEY_LOCATIONDATA = "key_locationdata";

        static WidgetUtils()
        {
            Init();
        }

        private static void Init()
        {
            var versionCode = GetVersion();
            if (versionCode < CURRENT_VERSION)
            {

            }

            // Set to latest version
            SetVersion(CURRENT_VERSION);
        }

        private static int GetVersion()
        {
            return widgetContainer.Values[KEY_VERSION]?.ToString()?.TryParseInt() ?? CURRENT_VERSION;
        }

        private static void SetVersion(int value)
        {
            widgetContainer.Values[KEY_VERSION] = value;
        }

        public static void AddWidgetId(string query, string id)
        {
            var listJson = widgetContainer.Values.GetValueOrDefault(query, "")?.ToString();
            if (string.IsNullOrWhiteSpace(listJson))
            {
                var newList = ImmutableList.Create(id);
                SaveIds(query, newList);
            }
            else
            {
                var idList = JSONParser.Deserializer<List<string>>(listJson);
                if (idList != null && !idList.Contains(id))
                {
                    idList.Add(id);
                    SaveIds(query, idList);
                }
            }

            CleanupWidgetData();
            CleanupWidgetIds();
        }

        public static void RemoveWidgetId(string query, string id, bool deletePrefs = true)
        {
            var listJson = widgetContainer.Values.GetValueOrDefault(query, "")?.ToString();
            if (!string.IsNullOrWhiteSpace(listJson))
            {
                var idList = JSONParser.Deserializer<List<string>>(listJson);
                if (idList?.Contains(id) == true)
                {
                    idList.Remove(id);
                    if (idList.Count == 0)
                    {
                        widgetContainer.Values.Remove(query);
                    }
                    else
                    {
                        SaveIds(query, idList);
                    }
                }
            }

            if (deletePrefs)
            {
                DeletePreferences(id);
            }
        }

        private static void DeletePreferences(string id)
        {
            widgetContainer.DeleteContainer($"AppWidget_{id}");
        }

        public static void UpdateWidgetIds(string oldQuery, LocationData.LocationData newLocation)
        {
            var listJson = widgetContainer.Values.GetValueOrDefault(oldQuery, "")?.ToString();
            widgetContainer.Values.Remove(oldQuery);
            widgetContainer.Values[newLocation.query] = listJson;

            foreach (string id in GetWidgetIds(newLocation.query))
            {
                SaveLocationData(id, newLocation);
            }

            CleanupWidgetData();
            CleanupWidgetIds();
        }

        public static void UpdateWidgetIds(string oldQuery, string newQuery)
        {
            var listJson = widgetContainer.Values.GetValueOrDefault(oldQuery, "")?.ToString();
            widgetContainer.Values.Remove(oldQuery);
            widgetContainer.Values[newQuery] = listJson;

            foreach (string id in GetWidgetIds(newQuery))
            {
                SaveLocationData(id, null);
            }

            CleanupWidgetData();
            CleanupWidgetIds();
        }

        public static IList<string> GetWidgetIds(string query)
        {
            var listJson = widgetContainer.Values.GetValueOrDefault(query, "")?.ToString();
            if (!string.IsNullOrWhiteSpace(listJson))
            {
                var idList = JSONParser.Deserializer<List<string>>(listJson);
                if (idList != null)
                {
                    return idList;
                }
            }

            return ImmutableList<string>.Empty;
        }

        public static bool Exists(string query)
        {
            var listJson = widgetContainer.Values.GetValueOrDefault(query, "")?.ToString();
            if (!string.IsNullOrWhiteSpace(listJson))
            {
                var idList = JSONParser.Deserializer<List<string>>(listJson);
                if (idList?.Any() == true)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IdExists(string id)
        {
            var locData = GetLocationData(id);

            if (locData != null)
            {
                var listJson = widgetContainer.Values.GetValueOrDefault(locData.query, "")?.ToString();
                if (!string.IsNullOrWhiteSpace(listJson))
                {
                    var idList = JSONParser.Deserializer<List<string>>(listJson);
                    if (idList != null)
                    {
                        return idList.Contains(id);
                    }
                }
            }

            return false;
        }

        private static void SaveIds(string key, IList<string> idList)
        {
            var json = JSONParser.Serializer(idList);
            widgetContainer.Values[key] = json;
        }

        private static ApplicationDataContainer GetPreferences(string id)
        {
            return widgetContainer.CreateContainer($"AppWidget_{id}", ApplicationDataCreateDisposition.Always);
        }

        public static void SaveLocationData(string id, LocationData.LocationData location)
        {
            var prefs = GetPreferences(id);
            var locJson = JSONParser.Serializer(location);
            if (locJson != null)
            {
                prefs.Values[KEY_LOCATIONDATA] = locJson;
            }
        }

        public static LocationData.LocationData GetLocationData(string id)
        {
            var prefs = GetPreferences(id);
            var locDataJson = prefs.Values.GetValueOrDefault(KEY_LOCATIONDATA)?.ToString();

            return locDataJson?.Let(JSONParser.Deserializer<LocationData.LocationData>);
        }

        public static void CleanupWidgetIds()
        {
            Task.Run(async () =>
            {
                var settingsManager = Ioc.Default.GetService<SettingsManager>();
                var locs = (await settingsManager.GetLocationData()).ToList();
                (await settingsManager.GetLastGPSLocData())?.Let(it =>
                {
                    locs.Add(it);
                });

                var currLocQueries = locs.Select(l => l.query);
                var keys = widgetContainer.Values.Keys;
                foreach (var key in keys)
                {
                    if (KEY_VERSION != key && Constants.KEY_GPS != key && !currLocQueries.Contains(key))
                    {
                        widgetContainer.Values.Remove(key);
                    }
                }
            });
        }

        public static void CleanupWidgetData()
        {
            Task.Run(() =>
            {
                try
                {
                    var currentIds = WidgetManager.GetDefault().GetWidgetIds();

                    var widgetPrefs = widgetContainer.Containers.Keys
                        .Where(s => s.StartsWith("AppWidget_"))
                        .Select(s => s.ReplaceFirst("AppWidget_", ""))
                        .ToList();

                    foreach (var key in widgetPrefs)
                    {
                        if (!currentIds.Contains(key))
                        {
                            widgetContainer.DeleteContainer($"AppWidget_{key}");
                        }
                    }
                }
                catch (Exception) { }
            });
        }

        public static bool IsGPS(string id)
        {
            var listJson = widgetContainer.Values.GetValueOrDefault(Constants.KEY_GPS, "")?.ToString();
            if (!string.IsNullOrWhiteSpace(listJson))
            {
                var idList = JSONParser.Deserializer<List<string>>(listJson);
                if (idList?.Any() == true)
                {
                    return idList.Contains(id);
                }
            }

            return false;
        }

        public static void DeleteWidget(string id)
        {
            if (IsGPS(id))
            {
                RemoveWidgetId(Constants.KEY_GPS, id);
            }
            else
            {
                var locData = GetLocationData(id);
                if (locData != null)
                {
                    RemoveWidgetId(locData.query, id);
                }
            }
        }

        public static void RemapWidget(string oldId, string newId)
        {
            if (IsGPS(oldId))
            {
                RemoveWidgetId(Constants.KEY_GPS, oldId);
                AddWidgetId(Constants.KEY_GPS, newId);
            }
            else
            {
                var locData = GetLocationData(oldId);
                if (locData != null)
                {
                    RemoveWidgetId(Constants.KEY_GPS, oldId);
                    AddWidgetId(Constants.KEY_GPS, newId);
                    SaveLocationData(newId, locData);
                }
            }
        }

        public static void RemoveLocation(string query)
        {
            widgetContainer.Values.Remove(query);
        }

        public static AbstractWidgetCreator GetWidgetCreator(string widgetId)
        {
            var widgetInfo = WidgetManager.GetDefault().GetWidgetInfo(widgetId);

            if (widgetInfo?.WidgetContext?.DefinitionId == WeatherWidget.DefinitionId)
            {
                return new WeatherWidgetCreator();
            }
            else
            {
                throw new ArgumentException("Unknown widget type");
            }
        }
    }
}
