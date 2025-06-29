using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
        public enum ErrorStatus
        {
            Unknown = -1,
            Success,
            NoWeather,
            NetworkError,
            InvalidAPIKey,
            QueryNotFound,
            LocationNotSupported,
            RateLimited,
        }
    }
}
