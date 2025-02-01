using System;
using TimberLog;

namespace SimpleWeather.Utils
{
    public class ConsoleTree : Timber.Tree
    {
        protected override void Log(TimberLog.LoggerLevel loggerLevel, string category, string message,
            Exception exception)
        {
            Console.Write(loggerLevel.ToString().ToUpper() + "|" +
                          (category != null ? $"{category}: " : "")); // Write header
            Console.WriteLine(message); // Write message
        }
    }
}