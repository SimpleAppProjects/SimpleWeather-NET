#if !UNIT_TEST
using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Crashes;
using TimberLog;

namespace SimpleWeather.Utils
{
    public class AppCenterLoggingTree : Timber.Tree
    {
        private const string TAG = nameof(AppCenterLoggingTree);
        private const string KEY_PRIORITY = "Priority";
        private const string KEY_TAG = "Tag";
        private const string KEY_MESSAGE = "Message";

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
                    Crashes.TrackError(exception, properties,
                        ErrorAttachmentLog.AttachmentWithText(message, "message.txt"),
                        ErrorAttachmentLog.AttachmentWithText(exception.ToString(), "exception.txt"),
                        ErrorAttachmentLog.AttachmentWithText(exception.InnerException?.ToString(),
                            "inner_exception.txt"));
                }
                else
                {
                    Crashes.TrackError(new Exception(message), properties,
                        ErrorAttachmentLog.AttachmentWithText(message, "message.txt"));
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