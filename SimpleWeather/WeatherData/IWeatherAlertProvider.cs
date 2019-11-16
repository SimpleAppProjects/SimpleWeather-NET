using SimpleWeather.Location;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public interface IWeatherAlertProvider
    {
        Task<List<WeatherAlert>> GetAlerts(LocationData location);
    }
}
