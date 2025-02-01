#if !UNIT_TEST && (WINDOWS || __MACCATALYST__)
using System;
using System.Collections.Generic;
using Sentry;
using TimberLog;

namespace SimpleWeather.Utils
{
    public class SentryLoggingTree : Timber.Tree
    {
        private const string TAG = nameof(SentryLoggingTree);
        private const string KEY_PRIORITY = "priority";
        private const string KEY_TAG = "tag";
        private const string KEY_MESSAGE = "message";

        protected override bool IsLoggable(string category, TimberLog.LoggerLevel loggerLevel)
        {
            return loggerLevel >= TimberLog.LoggerLevel.Warn;
        }

        protected override void Log(TimberLog.LoggerLevel loggerLevel, string tag, string message, Exception exception)
        {
            try
            {
                var properties = new Dictionary<string, string>
                {
                    { KEY_PRIORITY, loggerLevel.ToString()?.ToUpper() },
                    { KEY_TAG, tag },
                    { KEY_MESSAGE, message }
                };

                if (exception != null)
                {
                    SentrySdk.CaptureException(exception, (s) =>
                    {
                        s.Level = loggerLevel.ToSentryLevel();

                        s.AddBreadcrumb(message, category: tag, level: loggerLevel.ToBreadcrumbLevel(),
                            data: properties);
                    });
                }
                else
                {
                    SentrySdk.AddBreadcrumb(message, category: tag, level: loggerLevel.ToBreadcrumbLevel(),
                        data: properties);
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Error writing log : " + e, TAG);
#endif
            }
        }
    }

    internal static class SentryExtensions
    {
        public static SentryLevel ToSentryLevel(this TimberLog.LoggerLevel level)
        {
            return level switch
            {
                TimberLog.LoggerLevel.Debug => SentryLevel.Debug,
                TimberLog.LoggerLevel.Info => SentryLevel.Info,
                TimberLog.LoggerLevel.Warn => SentryLevel.Warning,
                TimberLog.LoggerLevel.Error => SentryLevel.Error,
                TimberLog.LoggerLevel.Fatal => SentryLevel.Fatal,
                _ => SentryLevel.Info
            };
        }

        public static BreadcrumbLevel ToBreadcrumbLevel(this TimberLog.LoggerLevel level)
        {
            return level switch
            {
                TimberLog.LoggerLevel.Debug => BreadcrumbLevel.Debug,
                TimberLog.LoggerLevel.Info => BreadcrumbLevel.Info,
                TimberLog.LoggerLevel.Warn => BreadcrumbLevel.Warning,
                TimberLog.LoggerLevel.Error => BreadcrumbLevel.Error,
                TimberLog.LoggerLevel.Fatal => BreadcrumbLevel.Critical,
                _ => BreadcrumbLevel.Info
            };
        }
    }
}
#endif