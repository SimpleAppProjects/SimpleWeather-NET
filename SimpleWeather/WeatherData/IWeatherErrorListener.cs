using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.WeatherData
{
    public interface IWeatherErrorListener
    {
        void OnWeatherError(Utils.WeatherException wEx);
    }
}
