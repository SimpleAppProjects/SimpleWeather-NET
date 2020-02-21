using Windows.UI;

namespace SimpleWeather.WeatherData
{
    public partial interface IWeatherProviderImpl
    {
        string GetBackgroundURI(Weather weather);
        Color GetWeatherBackgroundColor(Weather weather);
    }
}