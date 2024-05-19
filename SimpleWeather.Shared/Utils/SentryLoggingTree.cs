#if !UNIT_TEST && WINDOWS
using Microsoft.AppCenter.Crashes;
using Sentry;
using System;
using System.Collections.Generic;
using TimberLog;

namespace SimpleWeather.Utils
{
    public class SentryLoggingTree : TimberLog.Timber.Tree
    {
        private const String TAG = nameof(SentryLoggingTree);
        private const String KEY_PRIORITY = "Priority";
        private const String KEY_MESSAGE = "Message";
        private const String KEY_EXCEPTION = "Exception";

        protected override void Log(TimberLog.LoggerLevel loggerLevel, string message, Exception exception)
        {
            try
            {
                if (loggerLevel < TimberLog.LoggerLevel.Warn)
                    return;

                if (exception != null)
                {
                    SentrySdk.CaptureException(exception, (s) =>
                    {
                        s.Level = loggerLevel.ToSentryLevel();

                        s.AddBreadcrumb(message, level: BreadcrumbLevel.Error);
                    });
                }
                else
                {
                    SentrySdk.CaptureMessage(message, loggerLevel.ToSentryLevel());
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
    }
}
#endif