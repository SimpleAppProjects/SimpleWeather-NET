using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.TZDB
{
    public interface ITZDBService
    {
        Task<string> GetTimeZone(double latitude, double longitude);
    }
}
