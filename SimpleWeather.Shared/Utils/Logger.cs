using SimpleWeather.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
#if WINDOWS
using Windows.Storage;
#endif
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
#if __IOS__
            Timber.Plant(new ConsoleTree());
#else
            Timber.Plant(new Timber.DebugTree());
#endif
            Timber.Plant(new FileLoggingTree());
#elif !UNIT_TEST
            CleanupLogs();
            Timber.Plant(new AppCenterLoggingTree());
#if WINDOWS
            Timber.Plant(new SentryLoggingTree());
#endif
#endif
        }

        public static bool IsDebugLoggerEnabled() => Timber.Forest.Any(it =>
        {
            return it is FileLoggingTree
#if __IOS__
                || it is ConsoleTree;
#else
                || it is Timber.DebugTree;
#endif
        });

        public static void EnableDebugLogger(bool enable)
        {
            if (enable)
            {
                if (!Timber.Forest.Any(it => it is FileLoggingTree))
                {
                    Timber.Plant(new FileLoggingTree());
                }
#if __IOS__
                if (!Timber.Forest.Any(it => it is ConsoleTree))
                {
                    Timber.Plant(new ConsoleTree());
                }
#else
                if (!Timber.Forest.Any(it => it is Timber.DebugTree))
                {
                    Timber.Plant(new Timber.DebugTree());
                }
#endif
            }
            else
            {
                Timber.Forest.ForEach(tree =>
                {
                    if (tree is FileLoggingTree)
                    {
                        Timber.Uproot(tree);
                    }
#if __IOS__
                    if (tree is ConsoleTree)
#else
                    if (tree is Timber.DebugTree)
#endif
                    {
                        Timber.Uproot(tree);
                    }
                });
            }
        }

        public static void RegisterLogger(Timber.Tree tree)
        {
            Timber.Plant(tree);
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
            Task.Run(() =>
            {
#if WINUI
                var logsDirectory = Path.Combine(ApplicationData.Current.LocalFolder.Path, "logs");
#else
                var logsDirectory = Path.Combine(ApplicationDataHelper.GetLocalFolderPath(), "logs");
#endif
                FileLoggingTree.CleanupLogs(logsDirectory);
            });
#endif
        }
    }
}