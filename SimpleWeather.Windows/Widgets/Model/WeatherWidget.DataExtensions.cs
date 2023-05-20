using SimpleWeather.Common.Controls;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.Utils;

namespace SimpleWeather.NET.Widgets.Model
{
    public static class WeatherWidgetDataExtensions
    {
        public static async Task<Current> ToCurrent(this WeatherUiModel weather)
        {
            var imageData = await weather.GetImageData();

            return new Current()
            {
                weatherIcon = await GetWeatherIcon(weather.WeatherIcon),
                temp = weather.CurTemp,
                feelsLike = weather.WeatherDetailsMap[WeatherDetailsType.FeelsLike]?.Value ?? Icons.WeatherIcons.EM_DASH,
                windSpeed = weather.WeatherDetailsMap[WeatherDetailsType.WindSpeed]?.Value ?? Icons.WeatherIcons.EM_DASH,
                chance = weather.WeatherDetailsMap[WeatherDetailsType.PoPChance]?.Value ?? Icons.WeatherIcons.EM_DASH,
                location = weather.Location,
                background = await SetBackground(imageData)
            };
        }

        public static Task<Forecast[]> ToForecasts(this IEnumerable<ForecastItemViewModel> forecasts)
        {
            return Task.WhenAll(forecasts.AsParallel().Select(async f =>
            {
                return new Forecast()
                {
                    date = f.ShortDate,
                    hi = f.HiTemp,
                    lo = f.LoTemp,
                    weatherIcon = await GetWeatherIcon(f.WeatherIcon)
                };
            }).ToArray());
        }

        public static Task<HourlyForecast[]> ToForecasts(this IEnumerable<HourlyForecastItemViewModel> forecasts)
        {
            return Task.WhenAll(forecasts.AsParallel().Select(async f =>
            {
                return new HourlyForecast()
                {
                    date = f.ShortDate,
                    hi = f.HiTemp,
                    weatherIcon = await GetWeatherIcon(f.WeatherIcon)
                };
            }).ToArray());
        }

        public static async Task<WeatherIcon> GetWeatherIcon(string icon)
        {
            return new WeatherIcon()
            {
                light = await SetWeatherIcon(icon, isLight: true),
                dark = await SetWeatherIcon(icon, isLight: false)
            };
        }

        private static async Task<string> SetWeatherIcon(string icon, bool isLight = false)
        {
            var wim = SharedModule.Instance.WeatherIconsManager;

            string path = wim.GetWeatherIconURI(icon, true, isLight);

            if (path?.StartsWith("ms-appx") == true)
            {
                path = await ImageUtils.WeatherIconToBase64(icon, isLight);
            }

            return path;
        }

        private static async Task<string> SetBackground(ImageDataViewModel imageData)
        {
            if (NET.Utils.FeatureSettings.TileBackgroundImage && imageData?.ImageUri != null)
            {
                var path = imageData.ImageUri.ToString();

                if (path.StartsWith("ms-appx"))
                {
                    // Widgets don't seem to support local images
                    path = await ImageUtils.ColorToBase64(imageData.Color.WithAlpha(0x50));
                }

                return path;
            }

            return string.Empty;
        }
    }
}
