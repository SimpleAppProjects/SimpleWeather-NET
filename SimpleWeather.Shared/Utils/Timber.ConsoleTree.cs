using System;

namespace SimpleWeather.Utils
{
    public class ConsoleTree : TimberLog.Timber.Tree
    {
        protected override void Log(TimberLog.LoggerLevel loggerLevel, string message, Exception exception)
        {
            Console.WriteLine(loggerLevel.ToString().ToUpper() + "|" + message);
        }
    }
}

