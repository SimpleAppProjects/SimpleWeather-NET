#if !DEBUG && !UNIT_TEST
using Microsoft.AppCenter.Analytics;
using SimpleWeather.Firebase;
#endif
#if DEBUG
using System;
#endif
using System.Collections.Generic;

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
#if WINDOWS
            var analytics = FirebaseHelper.GetFirebaseAnalytics();
            analytics.LogEvent(eventName, properties);
#endif
#endif
        }
    }
}