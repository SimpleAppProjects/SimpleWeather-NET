using SimpleWeather.WeatherData;

namespace SimpleWeather.Common.Controls
{
    public static class WeatherUiModelExtensions
    {
        public static WeatherUiModel ToUiModel(this Weather weather)
        {
            return new WeatherUiModel(weather);
        }
    }
}