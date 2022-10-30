﻿using SimpleWeather.WeatherData;
using System.Collections.Generic;

namespace SimpleWeather.Utils
{
    public static class WeatherProviderExtras
    {
        public static void LogMissingIcon(this IWeatherProviderImpl weatherProvider, string icon)
        {
            var encoded = icon?.EscapeUnicode();

            AnalyticsLogger.LogEvent("W_UnknownIcon", new Dictionary<string, string>
            {
                { "provider", weatherProvider.WeatherAPI },
                { "icon", icon },
                { "encoded", encoded }
            });

#if DEBUG
            Logger.WriteLine(LoggerLevel.Info, $"WeatherProvider: Unknown Icon provided - icon ({icon}), encoded ({encoded}), provider ({weatherProvider.WeatherAPI})");
#endif
        }
    }
}
