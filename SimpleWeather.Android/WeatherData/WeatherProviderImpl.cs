using Android.Content.Res;
using Android.Graphics;
using SimpleWeather.Droid;
using SimpleWeather.WeatherData;
using System;

namespace SimpleWeather.WeatherData
{
    public abstract partial class WeatherProviderImpl : IWeatherProviderImpl
    {
        // Utils Methods
        public abstract String GetBackgroundURI(Weather weather);
        public abstract Color GetWeatherBackgroundColor(Weather weather);
        public abstract int GetWeatherIconResource(string icon);
    }
}