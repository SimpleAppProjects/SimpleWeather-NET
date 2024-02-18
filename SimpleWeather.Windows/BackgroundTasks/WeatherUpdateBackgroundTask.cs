using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common.Location;
using SimpleWeather.Common.WeatherData;
using SimpleWeather.NET.Notifications;
using SimpleWeather.NET.Tiles;
using SimpleWeather.NET.WeatherAlerts;
using SimpleWeather.NET.Widgets;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Background;

namespace SimpleWeather.NET.BackgroundTasks
{
    public sealed class WeatherUpdateBackgroundTask : IBackgroundTask, IDisposable
    {
        private const string taskName = nameof(WeatherUpdateBackgroundTask);
        private readonly CancellationTokenSource cts;
        private static ApplicationTrigger AppTrigger = null;

        private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public WeatherUpdateBackgroundTask()
        {
            cts = new CancellationTokenSource();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            var deferral = taskInstance?.GetDeferral();

            taskInstance.Canceled += OnCanceled;

            await Task.Run(async () =>
            {
                try
                {
                    Logger.WriteLine(LoggerLevel.Debug, "WeatherUpdateBackgroundTask: Run...");

                    if (SettingsManager.WeatherLoaded)
                    {
                        var locationChanged = false;

                        if (SettingsManager.FollowGPS)
                        {
                            Logger.WriteLine(LoggerLevel.Debug, "WeatherUpdateBackgroundTask: Updating location...");

                            try
                            {
                                var result = await UpdateLocation();

                                switch (result)
                                {
                                    case LocationResult.Changed ret:
                                        locationChanged = true;
                                        await SettingsManager.SaveLastGPSLocData(ret.Data);
                                        break;
                                    default:
                                        // no-op
                                        break;
                                }
                            }
                            catch (Exception e)
                            {
                                Logger.WriteLine(LoggerLevel.Error, e);
                            }
                        }

                        if (cts.IsCancellationRequested) return;

                        Logger.WriteLine(LoggerLevel.Debug, "WeatherUpdateBackgroundTask: Refreshing weather for tiles...");

                        // Refresh weather for tiles
                        await PreloadWeather();

                        Logger.WriteLine(LoggerLevel.Debug, "WeatherUpdateBackgroundTask: Getting weather data...");
                        // Retrieve weather data.
                        var weather = await GetWeather();

                        if (cts.IsCancellationRequested) return;

                        await WeatherTileUpdaterTask.UpdateTiles();
                        await WeatherTileUpdaterTask.UpdateWidgets();

                        if (SettingsManager.PoPChanceNotificationEnabled)
                        {
                            await PoPNotificationCreator.CreateNotification(await SettingsManager.GetHomeData());
                        }

                        if (SettingsManager.DailyNotificationEnabled)
                        {
                            await DailyNotificationTask.ScheduleDailyNotification();
                        }

                        if (weather != null)
                        {
                            // Post alerts if setting is on
                            if (SettingsManager.ShowAlerts && wm.SupportsAlerts)
                            {
                                await WeatherAlertHandler.PostAlerts(await SettingsManager.GetHomeData(), weather.weather_alerts);
                            }

                            // Set update time
                            SettingsManager.UpdateTime = DateTime.Now;

                            if (locationChanged)
                            {
                                SharedModule.Instance.RequestAction(CommonActions.ACTION_WEATHER_SENDLOCATIONUPDATE,
                                    new Dictionary<string, object>
                                    {
                                        { CommonActions.EXTRA_FORCEUPDATE, false }
                                    });
                            }
                        }
                    }

                    Logger.WriteLine(LoggerLevel.Debug, "WeatherUpdateBackgroundTask: End of run...");
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "{0}: exception occurred...", taskName);
                }
            });

            // Inform the system that the task is finished.
            deferral.Complete();

            cts.Dispose();
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            try
            {
                cts?.Cancel();
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "{0}: Error cancelling task...", taskName);
            }

            Logger.WriteLine(LoggerLevel.Info, "{0}: Cancel Requested...", sender?.Task?.Name);
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
                var SettingsManager = Ioc.Default.GetService<SettingsManager>();

                // Register a task for each trigger
                var tb1 = BackgroundTaskUtils.CreateTask(taskName)
                    .Trigger(new TimeTrigger((uint)SettingsManager.RefreshInterval, false))
                    .Condition(new SystemCondition(SystemConditionType.InternetAvailable));
                var tb2 = BackgroundTaskUtils.CreateTask(taskName)
                    .Trigger(new SystemTrigger(SystemTriggerType.SessionConnected, false))
                    .Condition(new SystemCondition(SystemConditionType.InternetAvailable));
                var tb3 = BackgroundTaskUtils.CreateTask(taskName)
                    .Trigger(AppTrigger)
                    .Condition(new SystemCondition(SystemConditionType.InternetAvailable));

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

        private static IEnumerable<IBackgroundTaskRegistration> GetTaskRegistration()
        {
            return BackgroundTaskRegistration.AllTasks
                .Where(t => t.Value.Name == taskName)
                .Select(t => t.Value);
        }

        /// <exception cref="WeatherException">Ignore.</exception>
        private async Task<Weather> GetWeather()
        {
            Weather weather;

            try
            {
                cts.Token.ThrowIfCancellationRequested();

                weather = await new WeatherDataLoader(await SettingsManager.GetHomeData())
                        .LoadWeatherData(new WeatherRequest.Builder()
                            .ForceRefresh(false)
                            .LoadAlerts()
                            .LoadForecasts()
                            .Build()
                        );
            }
            catch (OperationCanceledException cancelEx)
            {
                Logger.WriteLine(LoggerLevel.Info, cancelEx, "{0}: GetWeather cancelled", taskName);
                return null;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "{0}: GetWeather error", taskName);
                return null;
            }

            return weather;
        }

        /// <exception cref="WeatherException">Ignore.</exception>
        private async Task PreloadWeather()
        {
            var locations = await SettingsManager.GetFavorites() ?? new Collection<LocationData.LocationData>();

            try
            {
                foreach (var location in locations)
                {
                    if (SecondaryTileUtils.Exists(location.query) || WidgetUtils.Exists(location.query))
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        await new WeatherDataLoader(location)
                            .LoadWeatherData(new WeatherRequest.Builder()
                                .ForceRefresh(false)
                                .LoadAlerts()
                                .Build()
                            );
                    }
                }
            }
            catch (OperationCanceledException cancelEx)
            {
                Logger.WriteLine(LoggerLevel.Info, cancelEx, "{0}: PreloadWeather cancelled", taskName);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "{0}: PreloadWeather error", taskName);
            }
        }

        private async Task<LocationResult> UpdateLocation()
        {
            var locationProvider = new LocationProvider();

            if (SettingsManager.FollowGPS)
            {
                var lastLocation = await SettingsManager.GetLastGPSLocData();
                return await locationProvider.GetLatestLocationData(lastLocation);
            }

            return new LocationResult.NotChanged(null);
        }

        public void Dispose()
        {
            cts?.Dispose();
        }
    }
}