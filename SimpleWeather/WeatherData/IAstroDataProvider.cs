using SimpleWeather.Location;
using SimpleWeather.Utils;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public interface IAstroDataProvider
    {
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        Task<Astronomy> GetAstronomyData(LocationData location);
    }
}
