using SimpleWeather.Utils;
using SimpleWeather.UWP.WNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace SimpleWeather.UWP.BackgroundTasks
{
    public sealed class UpdateTask : IBackgroundTask
    {
        private const string taskName = nameof(UpdateTask);

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            var deferral = taskInstance?.GetDeferral();

            await Task.Run(async () =>
            {
                // Run update logic
                Logger.WriteLine(LoggerLevel.Debug, "{0}: running update task", taskName);
                Settings.LoadIfNeeded();
                await WNSHelper.InitNotificationChannel();
            });

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
                taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.ServicingComplete, false));
                taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

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