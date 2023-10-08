#if __IOS__
using Foundation;
using SimpleWeather.Common.WeatherData;
using SimpleWeather.Utils;
using System.Text;

namespace SimpleWeather.Maui.Widget
{
    public static class WeatherWidgetUpdater
    {
#if DEBUG
        private const string GROUP_IDENTIFIER = "group.com.thewizrd.simpleweather.debug";
#else
        private const string GROUP_IDENTIFIER = "group.com.thewizrd.simpleweather";
#endif

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
                        weatherMap[result.Data.query] = result.Data.ToiOSWidgetWeather(location);
                    }
                }

                try
                {
                    var containerUrl = NSFileManager.DefaultManager.GetContainerUrl(GROUP_IDENTIFIER);
                    containerUrl.Append("widget", true).Append("weatherData.json", false);
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

        public static bool WidgetsExist()
        {
            // TODO: Implement
            return false;
        }

        public static void ReloadWidgets()
        {
            // TODO: implement
        }
    }
}
#endif