#if __IOS__
using Microsoft.Maui.Platform;
using SimpleWeather.Common.Controls;
using SimpleWeather.Common.Images;
using SimpleWeather.LocationData;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SimpleWeather.WeatherData.Images;

namespace SimpleWeather.Maui.Widget
{
    public class WeatherData
    {
        public Current current;
        public Forecast[] forecasts;
        public HourlyForecast[] hr_forecasts;
    }

    public class Current
    {
        public string locationName;
        public string tz_long;
        public bool isGPS;

        public string temp;
        public string condition;
        public string icon;
        public string hi;
        public string lo;
        public bool showHiLo;

        public string chance;

        public string backgroundColor;
    }

    public class Forecast
    {
        public double epochTime;
        public string hi;
        public string lo;
        public string icon;
        public string chance;
    }

    public class HourlyForecast
    {
        public double epochTime;
        public string temp;
        public string icon;
    }

    public static class WeatherDataExtensions
    {
        public static WeatherData ToiOSWidgetWeather(this Weather weather, LocationData.LocationData location)
        {
            var uiModel = weather.ToUiModel();
            var isFahrenheit = Units.FAHRENHEIT.Equals(DI.Utils.SettingsManager.TemperatureUnit);
            var imageData = new ImageDataHelperDefault().GetDefaultImageData(weather.GetBackgroundCode());

            return new WeatherData()
            {
                current = new Current()
                {
                    locationName = uiModel.Location,
                    tz_long = location.tz_long,
                    isGPS = location.locationType == LocationType.GPS,

                    temp = uiModel.CurTemp,
                    condition = uiModel.CurCondition,
                    icon = uiModel.WeatherIcon,
                    hi = uiModel.HiTemp,
                    lo = uiModel.LoTemp,
                    showHiLo = uiModel.ShowHiLo,

                    chance = uiModel.WeatherDetailsMap[WeatherDetailsType.PoPRain]?.Value,

                    backgroundColor = imageData.HexColor
                },
                forecasts = weather.forecast?.Select(f =>
                {
                    var model = new ForecastItemViewModel(f);

                    return new Forecast()
                    {
                        epochTime = f.date.ToNSDate().SecondsSince1970,
                        icon = model.WeatherIcon,
                        hi = model.HiTemp,
                        lo = model.LoTemp,
                        chance = model.DetailExtras[WeatherDetailsType.PoPRain]?.Value
                    };
                }).ToArray(),
                hr_forecasts = weather.hr_forecast?.Select(f =>
                {
                    var model = new HourlyForecastItemViewModel(f);

                    return new HourlyForecast()
                    {
                        epochTime = f.date.ToUnixTimeSeconds(),
                        icon = model.WeatherIcon,
                        temp = model.HiTemp
                    };
                }).ToArray()
            };
        }
    }
}
#endif