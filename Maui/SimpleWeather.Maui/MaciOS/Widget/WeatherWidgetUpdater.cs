﻿#if __IOS__
using Foundation;
using SimpleWeather.Common.WeatherData;
using SimpleWeather.Utils;
using System.Text;
using WidgetKitProxy;

namespace SimpleWeather.Maui.Widget
{
    public static class WeatherWidgetUpdater
    {
        private const string GROUP_IDENTIFIER = AppDelegate.GROUP_IDENTIFIER;

        private static Lazy<WidgetCenterProxy> widgetCenterProxyLazy = new(() => {
            return new WidgetCenterProxy();
        });

        private static WidgetCenterProxy WidgetCenter => widgetCenterProxyLazy.Value;

        public static async Task UpdateWidgetData()
        {
            var settingsMgr = DI.Utils.SettingsManager;

            var homeData = await settingsMgr.GetHomeData();
            var locations = await settingsMgr.GetFavorites() ?? new List<LocationData.LocationData>();
            if (settingsMgr.FollowGPS && homeData?.IsValid() == true)
            {
                locations = locations.Prepend(homeData);
            }

            await UpdateWidgetData(locations);
        }

        public static Task UpdateWidgetData(IEnumerable<LocationData.LocationData> locations)
        {
            return Task.Run(async () =>
            {
                var weatherMap = new Dictionary<string, WeatherData>();

                foreach (var location in locations)
                {
                    var result = await new WeatherDataLoader(location).LoadWeatherResult(
                        new WeatherRequest.Builder()
                            .ForceLoadSavedData()
                            .LoadForecasts()
                            .Build()
                    );

                    if (result?.Data != null)
                    {
                        if (location.locationType == LocationData.LocationType.GPS)
                            weatherMap["GPS"] = result.Data.ToiOSWidgetWeather(location);
                        else
                            weatherMap[result.Data.query] = result.Data.ToiOSWidgetWeather(location);
                    }
                }

                try
                {
                    var containerUrl = NSFileManager.DefaultManager.GetContainerUrl(GROUP_IDENTIFIER)
                        .Append("widget", true)
                        .Append("weatherData.json", false);
                    var directory = Path.GetDirectoryName(containerUrl.Path);
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                    await File.WriteAllTextAsync(containerUrl.Path, JSONParser.Serializer(weatherMap), Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "Error updating widget data");
                }
            });
        }

        public static Task<Dictionary<string, WeatherData>> GetWidgetData()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var containerUrl = NSFileManager.DefaultManager.GetContainerUrl(GROUP_IDENTIFIER);
                    containerUrl.Append("widget", true).Append("weatherData.json", false);
                    var json = await File.ReadAllTextAsync(containerUrl.Path, Encoding.UTF8);
                    return JSONParser.Deserializer<Dictionary<string, WeatherData>>(json);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "Error updating widget data");
                }

                return null;
            });
        }

        public static Task<bool> WidgetsExist()
        {
            var tcs = new TaskCompletionSource<bool>();

            WidgetCenter.GetCurrentConfigurationsWithCompletion((widgets) => {
                tcs.SetResult(widgets.Count > 0);
            });

            return tcs.Task;
        }

        public static void ReloadWidgets()
        {
            WidgetCenter.ReloadAllTimeLines();
        }
    }
}
#endif