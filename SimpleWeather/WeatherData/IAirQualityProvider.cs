using SimpleWeather.Location;
using SimpleWeather.Utils;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public interface IAirQualityProvider
    {
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<AirQuality> GetAirQualityData(LocationData location);
    }
}
