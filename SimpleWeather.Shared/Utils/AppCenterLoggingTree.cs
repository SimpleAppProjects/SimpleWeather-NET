#if WINDOWS_UWP && !UNIT_TEST
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;

namespace SimpleWeather.Utils
{
    public class AppCenterLoggingTree : TimberLog.Timber.Tree
    {
        private const String TAG = nameof(AppCenterLoggingTree);
        private const String KEY_PRIORITY = "Priority";
        private const String KEY_MESSAGE = "Message";
        private const String KEY_EXCEPTION = "Exception";

        protected override void Log(TimberLog.LoggerLevel loggerLevel, string message, Exception exception)
        {
            try
            {
                if (loggerLevel < TimberLog.LoggerLevel.Warn)
                    return;

                var properties = new Dictionary<string, string>
                {
                    { KEY_PRIORITY, loggerLevel.ToString()?.ToUpper() },
                    { KEY_MESSAGE, message },
                    { KEY_EXCEPTION, exception?.ToString() }
                };

                if (exception != null)
                {
                    Crashes.TrackError(exception, properties,
                        ErrorAttachmentLog.AttachmentWithText(exception.ToString(), "exception.txt"));
                }
                else
                {
                    Crashes.TrackError(new Exception(message), properties);
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
}
#endif