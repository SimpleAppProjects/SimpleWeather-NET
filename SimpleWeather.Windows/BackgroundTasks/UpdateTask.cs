﻿using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Extras.BackgroundTasks;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using Windows.ApplicationModel.Background;

namespace SimpleWeather.NET.BackgroundTasks
{
    public sealed partial class UpdateTask : IBackgroundTask
    {
        private const string taskName = nameof(UpdateTask);

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

                if (SettingsManager.WeatherLoaded)
                {
                    // Check if WidgetTask is registered
                    // If not register
                    await WeatherTileUpdaterTask.RegisterBackgroundTask(false);

                    // Check if WeatherTask is registered
                    // If not register
                    await WeatherUpdateBackgroundTask.RegisterBackgroundTask(false);

                    // Register tasks if not already registered
                    if (SettingsManager.DailyNotificationEnabled)
                    {
                        await DailyNotificationTask.RegisterBackgroundTask(false);
                    }

                    await RemoteConfigUpdateTask.RegisterBackgroundTask(false);
                    await PremiumStatusTask.RegisterBackgroundTask(false);
                    await AppUpdaterTask.RegisterBackgroundTask(false);
                }
            });

            // Inform the system that the task is finished.
            deferral.Complete();
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
                var taskBuilder = BackgroundTaskUtils.CreateTask(taskName)
                    .Trigger(new SystemTrigger(SystemTriggerType.ServicingComplete, false))
                    .Condition(new SystemCondition(SystemConditionType.InternetAvailable));

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
    }
}