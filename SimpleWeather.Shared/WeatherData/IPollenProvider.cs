using SimpleWeather.Location;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public interface IPollenProvider
    {
        Task<Pollen> GetPollenData(LocationData location);
    }
}
