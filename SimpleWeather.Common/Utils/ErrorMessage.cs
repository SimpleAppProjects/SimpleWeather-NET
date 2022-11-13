using SimpleWeather.Utils;

namespace SimpleWeather.Common.Utils
{
    public interface ErrorMessage
    {
        public sealed record Resource(string ResourceId) : ErrorMessage;
        public sealed record String(string Message) : ErrorMessage;
        public sealed record WeatherError(WeatherException Exception) : ErrorMessage;
    }
}
