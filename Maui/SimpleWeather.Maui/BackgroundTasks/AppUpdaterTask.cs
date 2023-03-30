#if __IOS__ || __MACCATALYST__
using BackgroundTasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.BackgroundTasks;
using SimpleWeather.Common.Helpers;
using SimpleWeather.Maui.Updates;
using SimpleWeather.Utils;
using UserNotifications;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.BackgroundTasks
{
    public partial class AppUpdaterTask : IBackgroundTask
    {
        private const string taskName = nameof(AppUpdaterTask);
        public const string TASK_ID = $"SimpleWeather.{taskName}";
        private static bool Registered = false;

        private readonly CancellationTokenSource cts = new();

        public bool IsCancelled => cts.IsCancellationRequested;

        public event EventHandler TaskCompleted;

        private AppUpdaterTask() { }

        public void Cancel()
        {
            cts.Cancel();
        }

        public async Task Run()
        {
            try
            {
                // Run update logic
                Logger.WriteLine(LoggerLevel.Debug, "{0}: running app update check task", taskName);

#if !DEBUG
                await CheckForUpdates();
#endif
            }
            finally
            {
                TaskCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool IsAppUpdaterSupported()
        {
#if DEBUG
            return false;
#else
            return true;
#endif
        }

        private async Task CheckForUpdates()
        {
            if (IsAppUpdaterSupported())
            {
                var appUpdateManager = Ioc.Default.GetService<InAppUpdateManager>();

                if (await appUpdateManager.CheckIfUpdateAvailable())
                {
                    if (appUpdateManager.UpdatePriority > 3 && !FeatureSettings.IsUpdateAvailable && await NotificationPermissionRequestHelper.AreNotificationsEnabled())
                    {
                        // Notify user of update availability
                        var notificationCenter = UNUserNotificationCenter.Current;

                        var content = new UNMutableNotificationContent()
                        {
                            Title = ResStrings.prompt_update_title,
                            Body = ResStrings.prompt_update_available,
                            Sound = null
                        };
                        // TODO: add action to open app store
                        var request = UNNotificationRequest.FromIdentifier(
                            (DateTime.Now.Ticks + appUpdateManager.UpdatePriority).ToString(), content, null
                        );

                        try
                        {
                            await notificationCenter.AddNotificationRequestAsync(request);
                        }
                        catch (Exception ex) { }
                    }
                }
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

        public static void CancelPendingTasks()
        {
            BGTaskScheduler.Shared.Cancel(TASK_ID);
        }

        private static void HandleTaskRegistration(BGProcessingTask task)
        {
            // Schedule a new refresh task.
            ScheduleTask();

            var taskOp = new AppUpdaterTask();

            task.ExpirationHandler = taskOp.Cancel;

            taskOp.TaskCompleted += (s, e) =>
            {
                task.SetTaskCompleted(!taskOp.IsCancelled);
            };

            Task.Run(taskOp.Run);
        }

        private static void ScheduleTask()
        {
            var request = new BGProcessingTaskRequest(TASK_ID)
            {
                EarliestBeginDate = Foundation.NSDate.FromTimeIntervalSinceNow(TimeSpan.FromDays(1).TotalSeconds),
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
