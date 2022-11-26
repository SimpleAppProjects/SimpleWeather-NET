using System;
using System.Threading.Tasks;
using Windows.Storage;
using Timber = TimberLog.Timber;

namespace SimpleWeather.Utils
{
    public enum LoggerLevel
    {
        Debug = TimberLog.LoggerLevel.Debug,
        Info = TimberLog.LoggerLevel.Info,
        Warn = TimberLog.LoggerLevel.Warn,
        Error = TimberLog.LoggerLevel.Error,
        Fatal = TimberLog.LoggerLevel.Fatal
    }

    public static class Logger
    {

        public static void Init()
        {
#if DEBUG
            Timber.Plant(new Timber.DebugTree());
            Timber.Plant(new FileLoggingTree());
#elif !UNIT_TEST
            CleanupLogs();
            Timber.Plant(new AppCenterLoggingTree());
#endif
        }

        public static void Shutdown()
        {
            Timber.UprootAll();
        }

        public static void WriteLine(LoggerLevel loggerLevel, string value, params object[] args)
        {
            WriteLine(loggerLevel, null, value, args);
        }

        public static void WriteLine(LoggerLevel loggerLevel, Exception ex)
        {
            WriteLine(loggerLevel, ex, null, null);
        }

        public static void WriteLine(LoggerLevel loggerLevel, Exception ex, string value)
        {
            WriteLine(loggerLevel, ex, value, null);
        }

        public static void WriteLine(LoggerLevel loggerLevel, Exception ex, string format, params object[] args)
        {
            // ${longdate}|${level:uppercase=true}|${message}${when:when=length('${exception}')>0:Inner=${newline}}${exception:format=tostring}
            Timber.Log((TimberLog.LoggerLevel)loggerLevel, ex, format, args);
        }

        private static void CleanupLogs()
        {
#if !UNIT_TEST
            Task.Run(async () =>
            {
                await FileUtils.DeleteDirectory(System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "logs"));
            });
#endif
        }
    }
}