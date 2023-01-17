using SimpleWeather.Utils;

namespace SimpleWeather.Common.Utils
{
    public interface ErrorMessage
    {
#if WINUI
        public sealed record Resource(string ResourceId) : ErrorMessage;
#endif
        public sealed record String(string Message) : ErrorMessage;
        public sealed record WeatherError(WeatherException Exception) : ErrorMessage;
    }
}
