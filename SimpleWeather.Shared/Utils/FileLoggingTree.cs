using SimpleWeather.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SimpleWeather.Utils
{
#pragma warning disable CA1063 // Implement IDisposable Correctly

    public class FileLoggingTree : TimberLog.Timber.Tree, IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        private const string TAG = nameof(FileLoggingTree);
        private const string LOG_NAME_FORMAT = "Logger.{0}.log";
        private const string LOG_DATE_FORMAT = "yyyy-MM-dd";

        private static bool ranCleanup = false;

        private FileStream fileStream;
        private StreamWriter writer;
        private Timer flushTimer;

        public FileLoggingTree()
            : base()
        {
            flushTimer = new Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
            flushTimer.Elapsed += (s, e) =>
            {
                if (writer != null)
                {
                    lock (writer)
                    {
                        writer.Flush();
                    }
                }
            };
            flushTimer.Disposed += (s, e) =>
            {
                if (writer != null)
                {
                    lock (writer)
                    {
                        writer.Flush();
                    }
                }
            };
        }

        protected override void Log(TimberLog.LoggerLevel loggerLevel, string message, Exception exception)
        {
            try
            {
#if WINDOWS
                var directory = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "logs");
#else
                var directory = Path.Combine(ApplicationDataHelper.GetLocalFolderPath(), "logs");
#endif

                var today = DateTimeOffset.Now;
                var culture = CultureInfo.InvariantCulture;

                var dateTimeStamp = today.ToString(LOG_DATE_FORMAT, culture);
                var logTimeStamp = today.ToString("yyyy-MM-dd HH:mm:ss:fff", culture);

                var fileName = String.Format(culture, LOG_NAME_FORMAT, dateTimeStamp);

                var priorityTAG = loggerLevel.ToString().ToUpper(culture);
                var logMessage = logTimeStamp + "|" + priorityTAG + "|" + message/* + Environment.NewLine*/;

                var path = Path.Combine(directory, fileName);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (fileStream == null || fileStream.Name != path)
                {
                    fileStream?.Flush();
                    fileStream?.Dispose();
                    fileStream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.Read);
                    writer = new StreamWriter(fileStream, Encoding.UTF8, 4096);
                }

                lock (writer)
                {
                    flushTimer.Stop();
                    writer.WriteLine(logMessage);
                    flushTimer.Start();
                }

                if (!ranCleanup)
                {
                    Task.Run(() =>
                    {
                        CleanupLogs(logsDirectory: directory);
                    });
                    ranCleanup = true;
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Error while logging into file : " + e, TAG);
#endif
            }
        }

        public static void CleanupLogs(string logsDirectory)
        {
            try
            {
                // Only keep a weeks worth of logs
                int daysToKeep = 7;

                // Get todays date
                var todayNow = DateTimeOffset.Now;

                // Create a list of the last 7 day's dates
                IList<string> dateStampsToKeep = new List<string>(7);
                for (int i = 0; i < daysToKeep; i++)
                {
                    var date = todayNow.AddDays(-i);
                    var dateStamp = date.ToString(LOG_DATE_FORMAT, CultureInfo.InvariantCulture);

                    dateStampsToKeep.Add(string.Format(CultureInfo.InvariantCulture, LOG_NAME_FORMAT, dateStamp));
                }

                // List all log files not in the above list
                var logDir = new DirectoryInfo(logsDirectory);
                var logs = logDir.EnumerateFiles("Logger*.log", SearchOption.TopDirectoryOnly);

                foreach (var log in logs)
                {
                    if (!dateStampsToKeep.Contains(log.Name))
                    {
                        log.Delete();
                    }
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Error cleaning up log files : " + e, TAG);
#endif
            }
        }

        public void Dispose()
        {
            flushTimer?.Dispose();
            writer?.Dispose();
            fileStream?.Dispose();
        }
    }
}