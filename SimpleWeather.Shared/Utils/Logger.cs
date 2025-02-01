#if WINDOWS
using Windows.Storage;
#endif
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SimpleWeather.Helpers;
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
        internal static bool DEBUG_MODE_ENABLED
        {
#if DEBUG
            get => true;
            private set { }
#else
            get => DI.Utils.SettingsManager.DebugModeEnabled;
            private set => DI.Utils.SettingsManager.DebugModeEnabled = value;
#endif
        }

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
#if WINDOWS || __MACCATALYST__
            Timber.Plant(new SentryLoggingTree());
#endif
            EnableDebugLogger(DEBUG_MODE_ENABLED);
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
            DEBUG_MODE_ENABLED = enable;

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

#nullable enable
        public static void WriteLine(LoggerLevel loggerLevel, Exception ex, string message, params object[] args)
        {
            Timber.Log((TimberLog.LoggerLevel)loggerLevel, ex, message, args);
        }

        public static void WriteLine(LoggerLevel loggerLevel, string message, params object[] args)
        {
            Timber.Log((TimberLog.LoggerLevel)loggerLevel, message, args);
        }

        public static void WriteLine(LoggerLevel loggerLevel, Exception ex)
        {
            Timber.Log((TimberLog.LoggerLevel)loggerLevel, ex);
        }

        public static void Debug(string tag, string message, params object[] args)
        {
            Log(TimberLog.LoggerLevel.Debug, tag, message: message, args: args);
        }

        public static void Debug(string tag, Exception? ex = null, string? message = null, params object[] args)
        {
            Log(TimberLog.LoggerLevel.Debug, tag, ex, message, args);
        }

        public static void Info(string tag, string message, params object[] args)
        {
            Log(TimberLog.LoggerLevel.Info, tag, message: message, args: args);
        }

        public static void Info(string tag, Exception? ex = null, string? message = null, params object[] args)
        {
            Log(TimberLog.LoggerLevel.Info, tag, ex, message, args);
        }

        public static void Warn(string tag, string message, params object[] args)
        {
            Log(TimberLog.LoggerLevel.Warn, tag, message: message, args: args);
        }

        public static void Warn(string tag, Exception? ex = null, string? message = null, params object[] args)
        {
            Log(TimberLog.LoggerLevel.Warn, tag, ex, message, args);
        }

        public static void Error(string tag, string message, params object[] args)
        {
            Log(TimberLog.LoggerLevel.Error, tag, message: message, args: args);
        }

        public static void Error(string tag, Exception? ex = null, string? message = null, params object[] args)
        {
            Log(TimberLog.LoggerLevel.Error, tag, ex, message, args);
        }

        public static void Fatal(string tag, string message, params object[] args)
        {
            Log(TimberLog.LoggerLevel.Fatal, tag, message: message, args: args);
        }

        public static void Fatal(string tag, Exception? ex = null, string? message = null, params object[] args)
        {
            Log(TimberLog.LoggerLevel.Fatal, tag, ex, message, args);
        }

        private static void Log(TimberLog.LoggerLevel level, string tag, Exception? ex = null, string? message = null,
            params object[] args)
        {
            Timber.Tag(tag).Log(level, ex, message, args);
        }
#nullable restore

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