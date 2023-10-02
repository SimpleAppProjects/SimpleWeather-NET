#if !DEBUG && !UNIT_TEST
using Microsoft.AppCenter.Analytics;
#endif
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Utils
{
    public static class AnalyticsLogger
    {
        public static void LogEvent(string eventName, IDictionary<string, string> properties = null)
        {
#if DEBUG
            string append = properties == null ? String.Empty : Environment.NewLine + JSONParser.Serializer(properties);
            Logger.WriteLine(LoggerLevel.Info, "EVENT | " + eventName + append);
#elif !UNIT_TEST
            Analytics.TrackEvent(eventName, properties);
#endif
        }
    }
}