using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Notifications;
using SimpleWeather.UWP.Tiles;
using SimpleWeather.UWP.WeatherAlerts;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.UI.StartScreen;

namespace SimpleWeather.UWP.BackgroundTasks
{
    public sealed class WeatherUpdateBackgroundTask : IBackgroundTask, IDisposable
    {
        private const string taskName = nameof(WeatherUpdateBackgroundTask);
        private CancellationTokenSource cts;
        private readonly WeatherManager wm;
        private static ApplicationTrigger AppTrigger = null;

        public WeatherUpdateBackgroundTask()
        {
            cts = new CancellationTokenSource();
            wm = WeatherManager.GetInstance();
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

                    if (Settings.WeatherLoaded)
                    {
                        var locationChanged = false;

                        if (Settings.FollowGPS)
                        {
                            Logger.WriteLine(LoggerLevel.Debug, "WeatherUpdateBackgroundTask: Updating location...");

                            try
                            {
                                locationChanged = await UpdateLocation();
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

                        if (weather != null)
                        {
                            // Post alerts if setting is on
                            if (Settings.ShowAlerts && wm.SupportsAlerts)
                            {
                                await WeatherAlertHandler.PostAlerts(await Settings.GetHomeData(), weather.weather_alerts);
                            }

                            if (Settings.PoPChanceNotificationEnabled)
                            {
                                await PoPNotificationCreator.CreateNotification(await Settings.GetHomeData());
                            }

                            // Set update time
                            Settings.UpdateTime = DateTime.Now;

                            if (locationChanged)
                            {
                                SimpleLibrary.GetInstance().RequestAction(CommonActions.ACTION_WEATHER_SENDLOCATIONUPDATE,
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
                tb1.SetTrigger(new TimeTrigger((uint)Settings.RefreshInterval, false));
                tb1.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                var tb2 = new BackgroundTaskBuilder() { Name = taskName };
                tb2.SetTrigger(new SystemTrigger(SystemTriggerType.SessionConnected, false));
                tb2.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                var tb3 = new BackgroundTaskBuilder() { Name = taskName };
                tb3.SetTrigger(AppTrigger);
                tb3.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

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

        /// <exception cref="WeatherException">Ignore.</exception>
        private async Task<Weather> GetWeather()
        {
            Weather weather;

            try
            {
                cts.Token.ThrowIfCancellationRequested();

                weather = await new WeatherDataLoader(await Settings.GetHomeData())
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
            var locations = await Settings.GetFavorites() ?? new System.Collections.ObjectModel.Collection<Location.LocationData>();

            try
            {
                foreach (var location in locations)
                {
                    if (SecondaryTileUtils.Exists(location.query))
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

        private async Task<bool> UpdateLocation()
        {
            bool locationChanged = false;

            if (Settings.FollowGPS)
            {
                Geoposition newGeoPos = null;
                Geolocator geolocal = new Geolocator() { DesiredAccuracyInMeters = 5000, ReportInterval = 900000, MovementThreshold = 1600 };

                try
                {
                    cts.Token.ThrowIfCancellationRequested();
                    geolocal.AllowFallbackToConsentlessPositions();
                    newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10))
                        .AsTask(cts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    return locationChanged;
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "{0}: error requesting location permission", taskName);
                }

                // Access to location granted
                if (newGeoPos != null)
                {
                    var lastGPSLocData = await Settings.GetLastGPSLocData();

                    if (cts.IsCancellationRequested) return locationChanged;

                    // Check previous location difference
                    if (lastGPSLocData?.IsValid() == true
                        && Math.Abs(ConversionMethods.CalculateHaversine(lastGPSLocData.latitude, lastGPSLocData.longitude,
                        newGeoPos.Coordinate.Point.Position.Latitude, newGeoPos.Coordinate.Point.Position.Longitude)) < geolocal.MovementThreshold)
                    {
                        return false;
                    }

                    LocationQueryViewModel view = null;

                    await Task.Run(async () =>
                    {
                        try
                        {
                            view = await wm.GetLocation(newGeoPos);

                            if (String.IsNullOrWhiteSpace(view.LocationQuery))
                            {
                                view = new LocationQueryViewModel();
                            }
                            else if (String.IsNullOrWhiteSpace(view.LocationTZLong) && view.LocationLat != 0 && view.LocationLong != 0)
                            {
                                String tzId = await TZDB.TZDBCache.GetTimeZone(view.LocationLat, view.LocationLong);
                                if (!Equals("unknown", tzId))
                                    view.LocationTZLong = tzId;
                            }
                        }
                        catch (WeatherException)
                        {
                            view = new LocationQueryViewModel();
                        }
                    }, cts.Token).ConfigureAwait(false);

                    if (String.IsNullOrWhiteSpace(view.LocationQuery))
                    {
                        // Stop since there is no valid query
                        return false;
                    }

                    if (cts.IsCancellationRequested) return locationChanged;

                    // Save location as last known
                    lastGPSLocData = new Location.LocationData(view, newGeoPos);
                    Settings.SaveLastGPSLocData(lastGPSLocData);

                    locationChanged = true;
                }
            }

            return locationChanged;
        }

        public void Dispose()
        {
            cts?.Dispose();
        }
    }
}