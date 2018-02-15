using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public interface IWeatherAlertProvider
    {
        Task<List<WeatherAlert>> GetAlerts(LocationData location);
    }
}
