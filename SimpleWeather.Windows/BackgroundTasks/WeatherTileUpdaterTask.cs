using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Windows.Widgets.Providers;
using SimpleWeather.Common.WeatherData;
using SimpleWeather.NET.Tiles;
using SimpleWeather.NET.Widgets;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
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
                        await UpdateWidgets();

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

        public static async Task UpdateWidgets()
        {
            Logger.WriteLine(LoggerLevel.Debug, "{0}: Updating widgets...", taskName);

            var SettingsManager = Ioc.Default.GetService<SettingsManager>();

            // Update widgets
            var widgetManager = WidgetManager.GetDefault();
            IReadOnlyList<WidgetInfo> widgets = null;

            try
            {
                widgets = widgetManager.GetWidgetInfos();
            }
            catch { }

            if (widgets?.Count > 0)
            {
                foreach (var widgetInfo in widgets)
                {
                    LocationData.LocationData location = null;

                    var widgetId = widgetInfo.WidgetContext.Id;

                    if (WidgetUtils.IsGPS(widgetId))
                    {
                        location = await SettingsManager.GetLastGPSLocData();
                    }
                    else
                    {
                        location = WidgetUtils.GetLocationData(widgetId);
                    }

                    Logger.WriteLine(LoggerLevel.Debug, "Location = " + location?.ToString());
                    Logger.WriteLine(LoggerLevel.Debug, "WidgetID = " + widgetId);

                    if (location != null)
                    {
                        if (await GetWeather(location) == null)
                        {
                            await GetWeather(location, true);
                        }

                        await WidgetUpdateHelper.RefreshWidgets(location.locationType == LocationData.LocationType.GPS ? Constants.KEY_GPS : location.query);
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

            if (taskRegistration.Any())
            {
                if (reregister)
                {
                    // Unregister any previous exising background task
                    taskRegistration.ForEach(t => t.Unregister(true));
                }
                else
                {
                    return;
                }
            }

            if (AppTrigger == null)
                AppTrigger = new ApplicationTrigger();

            // Enable task if dependent features are enabled
            if (!await IsTaskFeaturesEnabled())
                return;

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
                var tb1 = BackgroundTaskUtils.CreateTask(taskName)
                    .Trigger(new TimeTrigger(60, false));
                var tb2 = BackgroundTaskUtils.CreateTask(taskName)
                    .Trigger(new SystemTrigger(SystemTriggerType.SessionConnected, false));
                var tb3 = BackgroundTaskUtils.CreateTask(taskName)
                    .Trigger(new SystemTrigger(SystemTriggerType.LockScreenApplicationAdded, false));
                var tb4 = BackgroundTaskUtils.CreateTask(taskName)
                    .Trigger(new SystemTrigger(SystemTriggerType.UserPresent, false));
                var tb5 = BackgroundTaskUtils.CreateTask(taskName)
                    .Trigger(AppTrigger);

                try
                {
                    tb1.Register();
                    tb2.Register();
                    tb3.Register();
                    tb4.Register();
                    tb5.Register();
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

        public static async Task UnregisterBackgroundTask()
        {
            if (!await IsTaskFeaturesEnabled())
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

        private static IEnumerable<IBackgroundTaskRegistration> GetTaskRegistration()
        {
            return BackgroundTaskRegistration.AllTasks
                .Where(t => t.Value.Name == taskName)
                .Select(t => t.Value);
        }

        public static async Task<bool> IsTaskFeaturesEnabled()
        {
            var settingsManager = Ioc.Default.GetService<SettingsManager>();
            return WidgetUpdateHelper.WidgetsExist() ||
                await IsSecondaryTilePinned() ||
                await IsPrimaryTilePinned() ||
                settingsManager.PoPChanceNotificationEnabled;
        }

        private static async Task<bool> IsPrimaryTilePinned()
        {
            try
            {
                // Get your own app list entry
                AppListEntry entry = (await Package.Current.GetAppListEntriesAsync())[0];

                // Check if your app is currently pinned
                bool isPinned = await StartScreenManager.GetDefault().ContainsAppListEntryAsync(entry);

                return isPinned;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static async Task<bool> IsSecondaryTilePinned()
        {
            try
            {
                var tiles = await SecondaryTile.FindAllAsync();
                return tiles.Any();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}