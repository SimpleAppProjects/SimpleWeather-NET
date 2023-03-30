#if __IOS__ || __MACCATALYST__
using BackgroundTasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.BackgroundTasks;
using SimpleWeather.Common.Helpers;
using SimpleWeather.Maui.Notifications;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using UIKit;

namespace SimpleWeather.Maui.BackgroundTasks
{
    public class DailyNotificationTask : IBackgroundTask
    {
        private const string taskName = nameof(DailyNotificationTask);
        public const string TASK_ID = $"SimpleWeather.{taskName}";
        private static bool Registered = false;

        private readonly CancellationTokenSource cts = new();

        public bool IsCancelled => cts.IsCancellationRequested;

        public event EventHandler TaskCompleted;

        private DailyNotificationTask() { }

        public void Cancel()
        {
            cts.Cancel();
        }

        public async Task Run()
        {
            try
            {
                var settingsMgr = Ioc.Default.GetService<SettingsManager>();

                if (settingsMgr.WeatherLoaded && settingsMgr.DailyNotificationEnabled && await NotificationPermissionRequestHelper.AreNotificationsEnabled())
                {
                    // Run update logic
                    Logger.WriteLine(LoggerLevel.Debug, "{0}: running update task", taskName);

                    await settingsMgr.LoadIfNeeded();

                    if (settingsMgr.WeatherLoaded && settingsMgr.DailyNotificationEnabled)
                    {
                        // Create toast notification
                        await DailyNotificationCreator.CreateNotification(await settingsMgr.GetHomeData());

                        // Register task for next time
                        ScheduleTask();
                    }
                }
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
                    HandleTaskRegistration(task as BGAppRefreshTask);
                });
            }
            else
            {
                ScheduleTask();
            }
        }

        public static void SendDailyNotification()
        {
            Logger.WriteLine(LoggerLevel.Debug, "{0}: Requesting to start work immediately", taskName);

            try
            {
                var cts = new CancellationTokenSource();
                var id = UIKit.UIApplication.SharedApplication.BeginBackgroundTask($"{TASK_ID}.immediate", cts.Cancel);

                if (id != UIApplication.BackgroundTaskInvalid)
                {
                    Task.Run(new DailyNotificationTask().Run, cts.Token).ContinueWith(t =>
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

        private static void HandleTaskRegistration(BGAppRefreshTask task)
        {
            // Schedule a new refresh task.
            ScheduleTask();

            var taskOp = new DailyNotificationTask();

            task.ExpirationHandler = taskOp.Cancel;

            taskOp.TaskCompleted += (s, e) =>
            {
                task.SetTaskCompleted(!taskOp.IsCancelled);
            };

            Task.Run(taskOp.Run);
        }

        public static void ScheduleTask()
        {
            var request = new BGAppRefreshTaskRequest(TASK_ID)
            {
                EarliestBeginDate = Foundation.NSDate.FromTimeIntervalSinceNow(GetTaskDelayInSeconds())
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

        private static double GetTaskDelayInSeconds()
        {
            var now = DateTime.Now;

            var SettingsManager = Ioc.Default.GetService<SettingsManager>();
            var notifTime = SettingsManager.DailyNotificationTime;
            var notifDateTime = now.Date.Add(notifTime);

            if (now > notifDateTime)
            {
                // The time past; execute tomorrow
                notifDateTime = notifDateTime.AddDays(1);
            }

            var delay = (notifDateTime - now).TotalSeconds;
            Logger.WriteLine(LoggerLevel.Debug, "{0}: delay = {1}s", taskName, delay);
            return delay;
        }
    }
}
#endif
