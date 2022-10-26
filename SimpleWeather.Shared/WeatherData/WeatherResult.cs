using SimpleWeather.Utils;

namespace SimpleWeather.WeatherData
{
    public interface WeatherResult
    {
        Weather Data { get; }

        public sealed class Success : WeatherResult
        {
            public Weather Data { get; }
            public bool IsSavedData { get; }

            public Success(Weather data, bool isSavedData = false)
            {
                Data = data;
                IsSavedData = isSavedData;
            }
        }

        public sealed class WeatherWithError : WeatherResult
        {
            public Weather Data { get; }
            public bool IsSavedData { get; }
            public WeatherException Exception { get; }

            public WeatherWithError(Weather data, WeatherException exception, bool isSavedData = true)
            {
                Data = data;
                IsSavedData = isSavedData;
                Exception = exception;
            }
        }

        public sealed class NoWeather : WeatherResult
        {
            public Weather Data { get; }
            public bool IsSavedData { get; }

            public NoWeather(Weather data = null, bool isSavedData = false)
            {
                Data = data;
                IsSavedData = isSavedData;
            }
        }

        public sealed class Error : WeatherResult
        {
            public Weather Data { get; }
            public WeatherException Exception { get; }

            public Error(WeatherException exception, Weather data = null)
            {
                Data = data;
                Exception = exception;
            }
        }
    }

    public static class WeatherResultExtensions
    {
#nullable enable
        public static WeatherResult ToWeatherResult(this Weather? weather, bool isSavedData = false)
#nullable disable
        {
            if (weather == null)
            {
                return new WeatherResult.NoWeather(isSavedData: isSavedData);
            }
            else
            {
                return new WeatherResult.Success(weather, isSavedData);
            }
        }
    }
}
