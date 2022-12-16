#if !UNIT_TEST
#if !__MACCATALYST__ && !__MACOS__
using Microsoft.AppCenter.Crashes;
#endif
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
                    { KEY_PRIORITY, loggerLevel.ToString()?.ToUpper() }
                };

                if (exception != null)
                {
#if !__MACCATALYST__ && !__MACOS__
                    Crashes.TrackError(exception, properties,
                        ErrorAttachmentLog.AttachmentWithText(message, "message.txt"),
                        ErrorAttachmentLog.AttachmentWithText(exception.ToString(), "exception.txt"),
                        ErrorAttachmentLog.AttachmentWithText(exception.InnerException?.ToString(), "inner_exception.txt"));
#endif
                }
                else
                {
#if !__MACCATALYST__ && !__MACOS__
                    Crashes.TrackError(new Exception(message), properties,
                        ErrorAttachmentLog.AttachmentWithText(message, "message.txt"));
#endif
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