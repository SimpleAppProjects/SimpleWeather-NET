using NLog;
using System;
#if WINDOWS_UWP
using Windows.Storage;
#endif

namespace SimpleWeather.Utils
{
    public enum LoggerLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public static class Logger
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        static Logger()
        {
            Init();
        }

        private static void Init()
        {
#if WINDOWS_UWP
            var storageFolder = ApplicationData.Current.LocalFolder;
            LogManager.Configuration.Variables["LogPath"] = storageFolder.Path;
#endif
        }

        public static void ForceShutdown()
        {
            LogManager.Shutdown();
        }

        public static void WriteLine(LoggerLevel loggerLevel, string value, params object[] args)
        {
            WriteLine(loggerLevel, null, value, args);
        }

        public static void WriteLine(LoggerLevel loggerLevel, Exception ex, string value)
        {
            WriteLine(loggerLevel, ex, value, null);
        }

        public static void WriteLine(LoggerLevel loggerLevel, Exception ex, string format, params object[] args)
        {
            switch (loggerLevel)
            {
                case LoggerLevel.Debug:
                    if (ex == null)
                        logger.Log(LogLevel.Debug, format, args);
                    else
                        logger.Log(LogLevel.Debug, ex, format, args);
                    break;

                case LoggerLevel.Info:
                    if (ex == null)
                        logger.Log(LogLevel.Info, format, args);
                    else
                        logger.Log(LogLevel.Info, ex, format, args);
                    break;

                case LoggerLevel.Warn:
                    if (ex == null)
                        logger.Log(LogLevel.Warn, format, args);
                    else
                        logger.Log(LogLevel.Warn, ex, format, args);
                    break;

                case LoggerLevel.Error:
                    if (ex == null)
                        logger.Log(LogLevel.Error, format, args);
                    else
                        logger.Log(LogLevel.Error, ex, format, args);
                    break;

                case LoggerLevel.Fatal:
                    if (ex == null)
                        logger.Log(LogLevel.Fatal, format, args);
                    else
                        logger.Log(LogLevel.Fatal, ex, format, args);
                    break;

                default:
                    break;
            }
        }
    }
}