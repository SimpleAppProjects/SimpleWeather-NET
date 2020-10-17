using SimpleWeather.Location;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public interface IWeatherAlertProvider
    {
        Task<ICollection<WeatherAlert>> GetAlerts(LocationData location);
    }
}
