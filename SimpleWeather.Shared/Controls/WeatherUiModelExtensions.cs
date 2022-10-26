using SimpleWeather.WeatherData;

namespace SimpleWeather.Controls
{
    public static class WeatherUiModelExtensions
    {
        public static WeatherUiModel ToUiModel(this Weather weather)
        {
            return new WeatherUiModel(weather);
        }
    }
}