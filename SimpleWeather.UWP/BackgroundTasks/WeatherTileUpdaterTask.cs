using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Tiles;
using SimpleWeather.UWP.WeatherAlerts;
using SimpleWeather.WeatherData;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.UI.StartScreen;

namespace SimpleWeather.UWP.BackgroundTasks
{
    public sealed class WeatherTileUpdaterTask : IBackgroundTask
    {
        private const string taskName = nameof(WeatherTileUpdaterTask);
        private readonly WeatherManager wm;
        private static ApplicationTrigger AppTrigger = null;

        public WeatherTileUpdaterTask()
        {
            wm = WeatherManager.GetInstance();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            var deferral = taskInstance?.GetDeferral();

            await Task.Run(async () =>
            {
                try
                {
                    Logger.WriteLine(LoggerLevel.Debug, "{0}: Run...", taskName);

                    if (Settings.WeatherLoaded)
                    {
                        await UpdateTiles();
                    }

                    Logger.WriteLine(LoggerLevel.Debug, "{0}: End of run...", taskName);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "{0}: exception occurred...", taskName);
                }
            });

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        public static async Task UpdateTiles()
        {
            Logger.WriteLine(LoggerLevel.Debug, "{0}: Updating primary tile...", taskName);

            await AsyncTask.RunAsync(WeatherTileCreator.TileUpdater(await Settings.GetHomeData()));

            // Update secondary tiles
            var tiles = await SecondaryTile.FindAllAsync();

            if (tiles?.Count > 0)
            {
                var locations = await Settings.GetLocationData();

                foreach (SecondaryTile tile in tiles)
                {
                    Logger.WriteLine(LoggerLevel.Debug, "{0}: Updating secondary tile...", taskName);

                    Location.LocationData location = null;
                    var tileQuery = SecondaryTileUtils.GetQueryFromId(tile.TileId);

                    if (tileQuery == Constants.KEY_GPS)
                    {
                        // Skip; this was already updated above
                        continue;
                    }
                    else
                    {
                        location = locations.FirstOrDefault(loc => loc.query != null && loc.query.Equals(tileQuery));
                    }

                    Logger.WriteLine(LoggerLevel.Debug, "Location = " + location?.ToString());
                    Logger.WriteLine(LoggerLevel.Debug, "TileID = " + tile.TileId);

                    if (location != null)
                        await AsyncTask.RunAsync(WeatherTileCreator.TileUpdater(location));
                }
            }
        }

        public static async Task RequestAppTrigger()
        {
            if (AppTrigger == null)
                AppTrigger = new ApplicationTrigger();

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
                try
                {
                    await AppTrigger.RequestAsync();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "{0}: Error requesting ApplicationTrigger", taskName);
                }
            }
            else
            {
                Logger.WriteLine(LoggerLevel.Error, "{0}: Can't trigger ApplicationTrigger, background access not allowed", taskName);
            }
        }

        public static async Task RegisterBackgroundTask(bool reregister = true)
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

            if (AppTrigger == null)
                AppTrigger = new ApplicationTrigger();

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
                var tb1 = new BackgroundTaskBuilder() { Name = taskName };
                tb1.SetTrigger(new TimeTrigger(60, false));
                var tb2 = new BackgroundTaskBuilder() { Name = taskName };
                tb2.SetTrigger(new SystemTrigger(SystemTriggerType.SessionConnected, false));
                var tb3 = new BackgroundTaskBuilder() { Name = taskName };
                tb3.SetTrigger(new SystemTrigger(SystemTriggerType.LockScreenApplicationAdded, false));

                try
                {
                    tb1.Register();
                    tb2.Register();
                    tb3.Register();
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