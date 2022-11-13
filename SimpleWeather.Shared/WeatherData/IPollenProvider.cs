using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public interface IPollenProvider
    {
        Task<Pollen> GetPollenData(LocationData.LocationData location);
    }
}
