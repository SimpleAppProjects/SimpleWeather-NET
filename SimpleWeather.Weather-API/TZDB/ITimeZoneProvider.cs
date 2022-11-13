using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.TZDB
{
    public interface ITimeZoneProvider
    {
        Task<string> GetTimeZone(double latitude, double longitude);
    }
}
