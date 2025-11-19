#if __IOS__ || __MACCATALYST__
using BackgroundTasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Maui.Images;
using SimpleWeather.RemoteConfig;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData.Images;
using UIKit;

namespace SimpleWeather.Maui.BackgroundTasks
{
    public class ImageDatabaseTask : IBackgroundTask
    {
        private const string taskName = nameof(ImageDatabaseTask);
        public const string TASK_ID = $"SimpleWeather.{taskName}";
        public static readonly TimeSpan INTERVAL = TimeSpan.FromDays(7);
        private static bool Registered = false;

        private readonly CancellationTokenSource cts = new();

        public bool IsCancelled => cts.IsCancellationRequested;

        public event EventHandler TaskCompleted;

        private ImageDatabaseTask() { }

        public void Cancel()
        {
            cts.Cancel();
        }

        public async Task Run()
        {
            try
            {
                // Check if cache is populated
                var imageDataService = Ioc.Default.GetService<IImageDataService>();
                if (!await imageDataService.IsEmpty() && !UpdateSettings.IsUpdateAvailable)
                {
                    // If so, check the last time the Image Database (Firestore) was updated
                    var remoteDBVersionTimestamp = 0L;
                    try
                    {
                        remoteDBVersionTimestamp = await ImageDatabase.GetVersionTimestamp();
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(LoggerLevel.Error, e);
                    }

                    var localDBVersionTimestamp = imageDataService.GetImageDBVersionTimestamp();
                    var localDBUpdateTime = imageDataService.GetImageDBUpdateTime();
                    var localCachedDays = TimeSpan.FromTicks(DateTimeOffset.UtcNow.Ticks - localDBUpdateTime).TotalDays;

                    var remoteConfigService = Ioc.Default.GetService<IRemoteConfigService>();
                    var validDays = (remoteConfigService.GetLong("key_imagedb_valid_days").TakeIf(l => l != 0) ?? 30) + Random.Shared.Next(0, 8);

                    // Check if a new image db version is available or local image db cache time has expired
                    if (remoteDBVersionTimestamp > localDBVersionTimestamp || localCachedDays > validDays)
                    {
                        AnalyticsLogger.LogEvent($"{taskName}: clearing image cache", new Dictionary<string, string>()
                        {
                            { "remoteDBVersionTimestamp", remoteDBVersionTimestamp.ToString() },
                            { "localDBVersionTimestamp", localDBVersionTimestamp.ToString() },
                            { "localCachedDays", localCachedDays.ToString() },
                        });

                        // if so, invalidate
                        imageDataService.SetImageDBVersionTimestamp(remoteDBVersionTimestamp);
                        imageDataService.SetImageDBUpdateTime(DateTimeOffset.UtcNow.Ticks);
                        await imageDataService.ClearCachedImageData();
                        imageDataService.InvalidateCache(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex);
            }
            finally
            {
                TaskCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        public static void RegisterTask()
        {
            if (!Registered)
            {
                Registered = BGTaskScheduler.Shared.Register(TASK_ID, null, (task) =>
                {
                    HandleTaskRegistration(task as BGProcessingTask);
                });
            }
            else
            {
                ScheduleTask();
            }
        }

        public static void CheckConfig()
        {
            Logger.WriteLine(LoggerLevel.Debug, "{0}: Requesting to start work immediately", taskName);

            try
            {
                var cts = new CancellationTokenSource();
                var id = UIKit.UIApplication.SharedApplication.BeginBackgroundTask($"{TASK_ID}.immediate", cts.Cancel);

                if (id != UIApplication.BackgroundTaskInvalid)
                {
                    Task.Run(new ImageDatabaseTask().Run, cts.Token).ContinueWith(t =>
                    {
                        UIApplication.SharedApplication.EndBackgroundTask(id);
                    });
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e);
            }
        }

        public static void CancelPendingTasks()
        {
            BGTaskScheduler.Shared.Cancel(TASK_ID);
        }

        private static void HandleTaskRegistration(BGProcessingTask task)
        {
            // Schedule a new refresh task.
            ScheduleTask();

            var taskOp = new ImageDatabaseTask();

            task.ExpirationHandler = taskOp.Cancel;

            taskOp.TaskCompleted += (s, e) =>
            {
                task.SetTaskCompleted(!taskOp.IsCancelled);
            };

            Task.Run(taskOp.Run);
        }

        public static void ScheduleTask()
        {
            var request = new BGProcessingTaskRequest(TASK_ID)
            {
                EarliestBeginDate = Foundation.NSDate.FromTimeIntervalSinceNow(INTERVAL.TotalSeconds),
                RequiresNetworkConnectivity = true
            };

            try
            {
                BGTaskScheduler.Shared.Submit(request, out var error);

                if (error != null)
                {
                    Logger.WriteLine(LoggerLevel.Error, $"{taskName}: Error - ${error}");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, $"{taskName}: Unable to register task");
            }
        }
    }
}
#endif
