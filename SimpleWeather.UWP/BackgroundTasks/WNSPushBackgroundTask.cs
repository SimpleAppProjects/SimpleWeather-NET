using Newtonsoft.Json.Linq;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.PushNotifications;
using Windows.Storage;

namespace SimpleWeather.UWP.BackgroundTasks
{
    public sealed class WNSPushBackgroundTask : IBackgroundTask
    {
        private const string taskName = nameof(WNSPushBackgroundTask);

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            var deferral = taskInstance?.GetDeferral();

            // We received a push notification
            if (taskInstance.TriggerDetails is RawNotification rawNotification)
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        Logger.WriteLine(LoggerLevel.Debug, "{0}: received WNS push notification", taskName);

                        var payload = JObject.Parse(rawNotification.Content);
                        var invalidate = payload.Value<bool>("invalidate");
                        var date = payload.Value<long>("date");

                        if (invalidate)
                        {
                            Logger.WriteLine(LoggerLevel.Debug, "{0}: Enqueuing invalidation task", taskName);

                            ImageDataHelper.ImageDBUpdateTime = date;
                            await WNSWorkerBackgroundTask.RegisterBackgroundTask();
                        }
                    }
                    catch (Exception)
                    {
                    }
                });
            }

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        public static async Task RegisterBackgroundTask()
        {
            // Unregister any previous exising background task
            UnregisterBackgroundTask();

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
                var taskBuilder = new BackgroundTaskBuilder() { Name = taskName };
                PushNotificationTrigger trigger = new PushNotificationTrigger();
                taskBuilder.SetTrigger(trigger);

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
    }

    public sealed class WNSWorkerBackgroundTask : IBackgroundTask
    {
        private const string taskName = nameof(WNSWorkerBackgroundTask);

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            var deferral = taskInstance?.GetDeferral();

            await Task.Run(async () =>
            {
                // Check if cache is populated
                if (!(await ImageDataHelper.ImageDataHelperImpl.IsEmpty()) && !FeatureSettings.IsUpdateAvailable)
                {
                    // If so, check if we need to invalidate
                    var updateTime = await ImageDatabase.GetLastUpdateTime();

                    if (updateTime > ImageDataHelper.ImageDBUpdateTime)
                    {
                        AnalyticsLogger.LogEvent(taskName + ": Invalidating cache");

                        // if so, invalidate
                        ImageDataHelper.ImageDBUpdateTime = updateTime;

                        await ImageDataHelper.ImageDataHelperImpl.ClearCachedImageData();
                        ImageDataHelper.ShouldInvalidateCache = true;
                    }
                }
            });

            deferral?.Complete();
        }

        public static async Task RegisterBackgroundTask()
        {
            // Unregister any previous exising background task
            UnregisterBackgroundTask();

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
                var taskBuilder = new BackgroundTaskBuilder() { Name = taskName };
                taskBuilder.SetTrigger(new TimeTrigger(60, true)); // One-shot
                taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                taskBuilder.AddCondition(new SystemCondition(SystemConditionType.SessionConnected));

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
    }
}