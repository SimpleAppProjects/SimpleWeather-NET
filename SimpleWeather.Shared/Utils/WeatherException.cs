using System;
using System.Diagnostics.CodeAnalysis;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Utils
{
    [Serializable]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
    [SuppressMessage("Usage", "CA2229:Implement serialization constructors", Justification = "<Pending>")]
    public class WeatherException : Exception
    {
        public WeatherUtils.ErrorStatus ErrorStatus { get; private set; }

        public WeatherException(WeatherUtils.ErrorStatus errorStatus)
        {
            ErrorStatus = errorStatus;
        }

        public WeatherException(WeatherUtils.ErrorStatus errorStatus, Exception innerException)
            : base("", innerException)
        {
            ErrorStatus = errorStatus;
        }

        public override string Message
        {
            get
            {
                return ErrorStatus switch
                {
                    WeatherUtils.ErrorStatus.NoWeather => ResStrings.werror_noweather,
                    WeatherUtils.ErrorStatus.NetworkError => ResStrings.werror_networkerror,
                    WeatherUtils.ErrorStatus.InvalidAPIKey => ResStrings.werror_invalidkey,
                    WeatherUtils.ErrorStatus.QueryNotFound => ResStrings.werror_querynotfound,
                    WeatherUtils.ErrorStatus.LocationNotSupported => ResStrings.werror_locationnotsupported,
                    WeatherUtils.ErrorStatus.RateLimited => ResStrings.werror_ratelimited,
                    _ => ResStrings.werror_unknown,
                };
            }
        }
    }
}