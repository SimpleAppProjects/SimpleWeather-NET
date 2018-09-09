using System;
using System.Collections.Generic;
using System.Text;
using NLog;

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
        private static readonly NLog.Logger logger;

        static Logger()
        {
            logger = LogManager.GetCurrentClassLogger();
            Init();
        }

        private static void Init()
        {
#if WINDOWS_UWP
            var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            LogManager.Configuration.Variables["LogPath"] = storageFolder.Path;
#elif __ANDROID__
            var extFilesDir = Android.App.Application.Context.GetExternalFilesDir(null);
            LogManager.Configuration.Variables["LogPath"] = extFilesDir.Path;
#endif
        }

        public static void ForceShutdown()
        {
            LogManager.Shutdown();
        }

        public static void WriteLine(LoggerLevel loggerLevel, string message, params object[] args)
        {
            WriteLine(loggerLevel, null, message, args);
        }

        public static void WriteLine(LoggerLevel loggerLevel, Exception ex, string message)
        {
            WriteLine(loggerLevel, ex, message, null);
        }

        public static void WriteLine(LoggerLevel loggerLevel, Exception ex, string message, params object[] args)
        {
            switch (loggerLevel)
            {
                case LoggerLevel.Debug:
                    if (ex == null)
                        logger.Log(LogLevel.Debug, message, args);
                    else
                        logger.Log(LogLevel.Debug, ex, message, args);
                    break;
                case LoggerLevel.Info:
                    if (ex == null)
                        logger.Log(LogLevel.Info, message, args);
                    else
                        logger.Log(LogLevel.Info, ex, message, args);
                    break;
                case LoggerLevel.Warn:
                    if (ex == null)
                        logger.Log(LogLevel.Warn, message, args);
                    else
                        logger.Log(LogLevel.Warn, ex, message, args);
                    break;
                case LoggerLevel.Error:
                    if (ex == null)
                        logger.Log(LogLevel.Error, message, args);
                    else
                        logger.Log(LogLevel.Error, ex, message, args);
                    break;
                case LoggerLevel.Fatal:
                    if (ex == null)
                        logger.Log(LogLevel.Fatal, message, args);
                    else
                        logger.Log(LogLevel.Fatal, ex, message, args);
                    break;
                default:
                    break;
            }
        }
    }
}
