using Android.Graphics;
using System;

namespace SimpleWeather.WeatherData
{
    public partial interface IWeatherProviderImpl
    {
        string GetBackgroundURI(Weather weather);
        Color GetWeatherBackgroundColor(Weather weather);
        int GetWeatherIconResource(string icon);
    }
}