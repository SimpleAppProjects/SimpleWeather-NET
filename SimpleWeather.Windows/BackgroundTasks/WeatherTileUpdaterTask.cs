using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.BackgroundTasks;
using SimpleWeather.Common.WeatherData;
using SimpleWeather.NET.Tiles;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.StartScreen;

namespace SimpleWeather.NET.BackgroundTasks
{
    public sealed class WeatherTileUpdaterTask : IBackgroundTask
    {
        private const string taskName = nameof(WeatherTileUpdaterTask);
        private readonly WeatherProviderManager wm;
        private static ApplicationTrigger AppTrigger;

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public WeatherTileUpdaterTask()
        {
            wm = WeatherModule.Instance.WeatherManager;
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

                    if (SettingsManager.WeatherLoaded)
                    {
                        await UpdateTiles();

                        if (SettingsManager.PoPChanceNotificationEnabled)
                        {
                            await Notifications.PoPNotificationCreator.CreateNotification(await SettingsManager.GetHomeData());
                        }
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

            var SettingsManager = Ioc.Default.GetService<SettingsManager>();

            var homeLocation = await SettingsManager.GetHomeData();

            if (await GetWeather(homeLocation) == null)
            {
                await GetWeather(homeLocation, true);
            }

            await WeatherTileCreator.TileUpdater(homeLocation);

            // Update secondary tiles
            IReadOnlyList<SecondaryTile> tiles = null;

            try
            {
                tiles = await SecondaryTile.FindAllAsync();
            }
            catch
            {
                // Note: COMException may occur ("Element not found")
            }

            if (tiles?.Count > 0)
            {
                var locations = await SettingsManager.GetLocationData();

                foreach (SecondaryTile tile in tiles)
                {
                    Logger.WriteLine(LoggerLevel.Debug, "{0}: Updating secondary tile...", taskName);

                    LocationData.LocationData location = null;
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
                    {
                        if (await GetWeather(location) == null)
                        {
                            await GetWeather(location, true);
                        }

                        await WeatherTileCreator.TileUpdater(location);
                    }
                }
            }
        }

        private static async Task<Weather> GetWeather(LocationData.LocationData location, bool forceRefresh = false)
        {
            Logger.WriteLine(LoggerLevel.Debug, $"{taskName}: Getting weather data for ({location})...");

            Weather weather = null;

            try
            {
                weather = await new WeatherDataLoader(location)
                    .LoadWeatherData(new WeatherRequest.Builder().Let(req =>
                    {
                        if (forceRefresh)
                        {
                            req.ForceRefresh(false)
                            .LoadAlerts()
                            .LoadForecasts();
                        }
                        else
                        {
                            req.ForceLoadSavedData();
                        }

                        return req;
                    }).Build());
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, $"{taskName}: GetWeather error");
            }

            return weather;
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
                var tb1 = new BackgroundTaskBuilder()
                {
                    Name = taskName,
                    TaskEntryPoint = BackgroundTask.TASK_ENTRY_POINT
                };
                tb1.SetTrigger(new TimeTrigger(60, false));
                var tb2 = new BackgroundTaskBuilder()
                {
                    Name = taskName,
                    TaskEntryPoint = BackgroundTask.TASK_ENTRY_POINT
                };
                tb2.SetTrigger(new SystemTrigger(SystemTriggerType.SessionConnected, false));
                var tb3 = new BackgroundTaskBuilder()
                {
                    Name = taskName,
                    TaskEntryPoint = BackgroundTask.TASK_ENTRY_POINT
                };
                tb3.SetTrigger(new SystemTrigger(SystemTriggerType.LockScreenApplicationAdded, false));
                var tb4 = new BackgroundTaskBuilder()
                {
                    Name = taskName,
                    TaskEntryPoint = BackgroundTask.TASK_ENTRY_POINT
                };
                tb4.SetTrigger(new SystemTrigger(SystemTriggerType.UserPresent, false));

                try
                {
                    tb1.Register();
                    tb2.Register();
                    tb3.Register();
                    tb4.Register();
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