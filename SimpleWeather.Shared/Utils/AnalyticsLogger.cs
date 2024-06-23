#if !UNIT_TEST
#if WINDOWS
#if RELEASE
using Microsoft.AppCenter.Analytics;
#endif
using SimpleWeather.Firebase;
using System.Text.RegularExpressions;
#else
using Microsoft.AppCenter.Analytics;
#endif

#if __IOS__
using FirebaseAnalytics = Firebase.Analytics.Analytics;
#if RELEASE
using System.Text.RegularExpressions;
#endif
#endif

#endif
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SimpleWeather.Utils
{
    public static partial class AnalyticsLogger
    {
#if WINDOWS && !UNIT_TEST
        private static readonly Lazy<FirebaseAnalytics> analyticsLazy = new Lazy<FirebaseAnalytics>(() =>
        {
            return FirebaseHelper.GetFirebaseAnalytics();
        });

        private static FirebaseAnalytics analytics => analyticsLazy.Value;
#endif

        public static void LogEvent(string eventName, IDictionary<string, string> properties = null)
        {
#if DEBUG
            string append = properties == null ? String.Empty : Environment.NewLine + JSONParser.Serializer(properties);
            Logger.WriteLine(LoggerLevel.Info, "EVENT | " + eventName + append);
#elif !UNIT_TEST
            Analytics.TrackEvent(eventName, properties);
#if WINDOWS
            analytics.LogEvent(GAnalyticsRegex().Replace(eventName, "_"), properties);
#elif __IOS__
            FirebaseAnalytics.LogEvent(GAnalyticsRegex().Replace(eventName, "_"), properties.ToDictionary(k => k.Key as object, v => v.Value as object));
#endif
#endif
        }

#if !UNIT_TEST
#if WINDOWS
        public static void SetUserProperty([MaxLength(24)] string property, [MaxLength(36)] string value)
        {
            analytics.SetUserProperty(property, value);
        }

        public static void SetUserProperty([MaxLength(24)] string property, bool value)
        {
            analytics.SetUserProperty(property, value);
        }
#elif __IOS__
        public static void SetUserProperty([MaxLength(24)] string property, [MaxLength(36)] string value)
        {
            FirebaseAnalytics.SetUserProperty(value, property);
        }

        public static void SetUserProperty([MaxLength(24)] string property, bool value)
        {
            FirebaseAnalytics.SetUserProperty(value.ToString(), property);
        }
#else
        public static void SetUserProperty([MaxLength(24)] string property, [MaxLength(36)] string value)
        {
            // no-op
        }

        public static void SetUserProperty([MaxLength(24)] string property, bool value)
        {
            // no-op
        }
#endif
#endif

#if !DEBUG && (WINDOWS || __IOS__) && !UNIT_TEST
        [GeneratedRegex("[^a-zA-Z0-9]")]
        private static partial Regex GAnalyticsRegex();
#endif
    }
}