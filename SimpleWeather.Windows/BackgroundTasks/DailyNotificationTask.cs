using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.NET.Notifications;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using Windows.ApplicationModel.Background;

namespace SimpleWeather.NET.BackgroundTasks
{
    public sealed partial class DailyNotificationTask : IBackgroundTask
    {
        private const string taskName = nameof(DailyNotificationTask);

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            var deferral = taskInstance?.GetDeferral();

            await Task.Run(async () =>
            {
                // Run update logic
                Logger.WriteLine(LoggerLevel.Debug, "{0}: running update task", taskName);

                await SettingsManager.LoadIfNeeded();

                if (SettingsManager.WeatherLoaded && SettingsManager.DailyNotificationEnabled)
                {
                    // Create toast notification
                    await DailyNotificationCreator.CreateNotification(await SettingsManager.GetHomeData());

                    // Register task for next time
                    await RegisterBackgroundTask(true);
                }
            });

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        public static async Task ScheduleDailyNotification()
        {
            Logger.WriteLine(LoggerLevel.Debug, "{0}: Requesting to start work immediately", taskName);

            var SettingsManager = Ioc.Default.GetService<SettingsManager>();

            await SettingsManager.LoadIfNeeded();

            if (SettingsManager.WeatherLoaded && SettingsManager.DailyNotificationEnabled)
            {
                // Create toast notification
                var now = DateTimeOffset.Now;
                var delay = GetTaskDelayInMinutes();
                var deliveryTime = now.AddMinutes(delay);

                await DailyNotificationCreator.ScheduleNotification(await SettingsManager.GetHomeData(), deliveryTime);
            }
        }

        public static async Task RegisterBackgroundTask(bool reregister = false)
        {
            var taskRegistration = GetTaskRegistration();

            if (taskRegistration != null)
            {
                if (reregister)
                {
                    // Unregister any previous exising background task
                    taskRegistration.Unregister(true);
                }
                else
                {
                    return;
                }
            }

            // Request access
            var backgroundAccessStatus = BackgroundAccessStatus.Unspecified;

            try
            {
                BackgroundExecutionManager.RemoveAccess();
                backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            }
            catch (UnauthorizedAccessException)
            {
                // An access denied exception may be thrown if two requests are issued at the same time
                // For this specific sample, that could be if the user double clicks "Request access"
            }

            // If allowed
            if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                // Register a task for each trigger
                var taskBuilder = BackgroundTaskUtils.CreateTask(taskName).Apply(t =>
                {
                    var delay = GetTaskDelayInMinutes();

                    t.Trigger(new TimeTrigger(delay < 15 ? 15 : delay, true))
                     .Condition(new SystemCondition(SystemConditionType.InternetAvailable));
                });

                try
                {
                    taskBuilder.Register();
                }
                catch (Exception ex)
                {
                    if (ex.HResult == -2147942583)
                    {
                        Logger.WriteLine(LoggerLevel.Error, ex, "{0}: background task already registered", taskName);
                    }
                    else
                    {
                        Logger.WriteLine(LoggerLevel.Error, ex, "{0}: error registering background task", taskName);
                    }
                }
            }
        }

        public static void UnregisterBackgroundTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    task.Value.Unregister(true);
                }
            }
        }

        private static IBackgroundTaskRegistration GetTaskRegistration()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    return task.Value;
                }
            }

            return null;
        }

        private static uint GetTaskDelayInMinutes()
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

            var delay = (uint)(notifDateTime - now).TotalMinutes;
            Logger.WriteLine(LoggerLevel.Debug, "{0}: delay = {1}", taskName, delay);
            return delay;
        }
    }
}