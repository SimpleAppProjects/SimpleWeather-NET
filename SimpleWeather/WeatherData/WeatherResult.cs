using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.WeatherData
{
    public sealed class WeatherResult
    {
        public bool IsSavedData { get; private set; } = false;
        public Weather Weather { get; private set; } = null;

        private WeatherResult()
        {
        }

        internal static WeatherResult Create(Weather weather, bool freshFromProvider)
        {
            return new WeatherResult()
            {
                IsSavedData = !freshFromProvider,
                Weather = weather
            };
        }
    }
}
