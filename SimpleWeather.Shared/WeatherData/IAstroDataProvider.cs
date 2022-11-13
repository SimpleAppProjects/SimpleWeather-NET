using SimpleWeather.Utils;
using System;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public interface IAstroDataProvider
    {
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<Astronomy> GetAstronomyData(LocationData.LocationData location);
    }

    public interface IAstroDataProviderDate
    {
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<Astronomy> GetAstronomyData(LocationData.LocationData location, DateTimeOffset date);
    }
}