using System;
using System.Diagnostics;
using TimberLog;
#if __IOS__
using Logg = System.Console;
#else
using Logg = System.Diagnostics.Trace;
#endif

namespace SimpleWeather.Utils
{
    public class ConsoleTree : Timber.DebugTree
    {
        protected override bool IsLoggable(string category, TimberLog.LoggerLevel loggerLevel)
        {
#if DEBUG
            return true;
#else
            return Logger.DEBUG_MODE_ENABLED || loggerLevel > TimberLog.LoggerLevel.Debug;
#endif
        }

        protected override void Log(TimberLog.LoggerLevel loggerLevel, string category, string message,
            Exception exception)
        {
            Logg.Write(loggerLevel.ToString().ToUpper() + "|" + (category != null ? category + ": " : "")); // Write header
            Logg.WriteLine(message); // Write message
        }
    }
}