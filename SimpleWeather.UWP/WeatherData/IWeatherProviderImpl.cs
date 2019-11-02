using System;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.WeatherData
{
    public partial interface IWeatherProviderImpl
    {
        string GetBackgroundURI(Weather weather);
        Color GetWeatherBackgroundColor(Weather weather);
    }
}