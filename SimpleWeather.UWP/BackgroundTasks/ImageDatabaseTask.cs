using SimpleWeather.Utils;
using SimpleWeather.UWP.Utils;
using SimpleWeather.WeatherData.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace SimpleWeather.UWP.BackgroundTasks
{
    public class ImageDatabaseTask : IBackgroundTask
    {
        private const string taskName = nameof(ImageDatabaseTask);

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            var deferral = taskInstance?.GetDeferral();

            // Check if cache is populated
            if (!await ImageDataHelper.ImageDataHelperImpl.IsEmpty() && !FeatureSettings.IsUpdateAvailable)
            {
                // Check firestore timestamp against Settings
                var updateTime = await ImageDatabase.GetLastUpdateTime();

                if (updateTime > ImageDataHelper.ImageDBUpdateTime)
                {
                    AnalyticsLogger.LogEvent(taskName + ": Invalidating cache");

                    ImageDataHelper.ImageDBUpdateTime = updateTime;

                    await ImageDataHelper.ImageDataHelperImpl.ClearCachedImageData();
                    await ImageDatabase.ImageDatabaseCache.ClearCache();
                }
            }

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
                taskBuilder.SetTrigger(new TimeTrigger(10080, false)); // Weekly task
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