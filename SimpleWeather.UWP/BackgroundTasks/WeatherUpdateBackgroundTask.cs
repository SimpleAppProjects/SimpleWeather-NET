using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Tiles;
using SimpleWeather.UWP.WeatherAlerts;
using SimpleWeather.WeatherData;
using System;
using System.Linq;
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
        private WeatherManager wm;
        private static ApplicationTrigger AppTrigger = null;

        public WeatherUpdateBackgroundTask()
        {
            cts = new CancellationTokenSource();
            wm = WeatherManager.GetInstance();
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            var deferral = taskInstance?.GetDeferral();

            taskInstance.Canceled += OnCanceled;

            Task.Run(async () =>
            {
                try
                {
                    Logger.WriteLine(LoggerLevel.Debug, "WeatherUpdateBackgroundTask: Run...");

                    if (Settings.WeatherLoaded)
                    {
                        Logger.WriteLine(LoggerLevel.Debug, "WeatherUpdateBackgroundTask: Getting weather data...");
                        // Retrieve weather data.
                        var weather = await GetWeather();

                        if (cts.IsCancellationRequested) return;

                        // Update the live tile with data.
                        Logger.WriteLine(LoggerLevel.Debug, "WeatherUpdateBackgroundTask: Weather is NULL = " + (weather == null).ToString());
                        Logger.WriteLine(LoggerLevel.Debug, "WeatherUpdateBackgroundTask: Updating primary tile...");
                        if (weather != null)
                            WeatherTileCreator.TileUpdater(Settings.HomeData, new WeatherNowViewModel(weather));

                        if (cts.IsCancellationRequested) return;

                        // Update secondary tiles
                        var tiles = await SecondaryTile.FindAllAsync();
                        foreach (SecondaryTile tile in tiles)
                        {
                            Logger.WriteLine(LoggerLevel.Debug, "WeatherUpdateBackgroundTask: Updating secondary tile...");

                            var locations = await Settings.GetLocationData();
                            var location = locations.FirstOrDefault(
                                loc => loc.query != null && loc.query.Equals(SecondaryTileUtils.GetQueryFromId(tile.TileId)));

                            Logger.WriteLine(LoggerLevel.Debug, "Location = " + location?.ToString());
                            Logger.WriteLine(LoggerLevel.Debug, "TileID = " + tile.TileId);

                            if (location != null)
                                await WeatherTileCreator.TileUpdater(location);

                            if (cts.IsCancellationRequested) return;
                        }

                        // Post alerts if setting is on
                        Logger.WriteLine(LoggerLevel.Debug, "WeatherUpdateBackgroundTask: Posting alerts...");
                        if (Settings.ShowAlerts && wm.SupportsAlerts && weather != null)
                            await WeatherAlertHandler.PostAlerts(Settings.HomeData, weather.weather_alerts);

                        if (cts.IsCancellationRequested) return;

                        // Set update time
                        if (weather != null)
                            Settings.UpdateTime = DateTime.Now;
                    }

                    Logger.WriteLine(LoggerLevel.Debug, "WeatherUpdateBackgroundTask: End of run...");
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "{0}: exception occurred...", taskName);
                }
            }).ContinueWith((t) =>
            {
                // Inform the system that the task is finished.
                deferral.Complete();

                cts.Dispose();
            });
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // TODO: Add code to notify the background task that it is cancelled.
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
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

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

        public static async Task RegisterBackgroundTask()
        {
            // Unregister any previous exising background task
            UnregisterBackgroundTask();

            if (AppTrigger == null)
                AppTrigger = new ApplicationTrigger();

            // Request access
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

            // If allowed
            if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                // Register a task for each trigger
                var tb1 = new BackgroundTaskBuilder() { Name = taskName };
                tb1.SetTrigger(new TimeTrigger((uint)Settings.RefreshInterval, false));
                var tb2 = new BackgroundTaskBuilder() { Name = taskName };
                tb2.SetTrigger(new SystemTrigger(SystemTriggerType.SessionConnected, false));
                var tb3 = new BackgroundTaskBuilder() { Name = taskName };
                tb3.SetTrigger(AppTrigger);

                tb1.Register();
                tb2.Register();
                tb3.Register();
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

        private async Task<Weather> GetWeather()
        {
            Weather weather = null;

            try
            {
                if (Settings.FollowGPS)
                    await UpdateLocation();

                cts.Token.ThrowIfCancellationRequested();

                var wloader = new WeatherDataLoader(Settings.HomeData);
                await wloader.LoadWeatherData(false);

                weather = wloader.GetWeather();
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
                    newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10)).AsTask(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    return locationChanged;
                }
                catch (Exception)
                {
                    var geoStatus = GeolocationAccessStatus.Unspecified;

                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        geoStatus = await Geolocator.RequestAccessAsync().AsTask(cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        return locationChanged;
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(LoggerLevel.Error, ex, "{0}: error requesting location permission", taskName);
                    }
                    finally
                    {
                        if (!cts.IsCancellationRequested && geoStatus == GeolocationAccessStatus.Allowed)
                        {
                            try
                            {
                                newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10)).AsTask(cts.Token);
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLine(LoggerLevel.Error, ex, "{0}: GetWeather error", taskName);
                            }
                        }
                        else if (geoStatus == GeolocationAccessStatus.Denied)
                        {
                            // Disable gps feature
                            Settings.FollowGPS = false;
                        }
                    }

                    if (!Settings.FollowGPS)
                        return false;
                }

                // Access to location granted
                if (newGeoPos != null)
                {
                    var lastGPSLocData = await Settings.GetLastGPSLocData();

                    if (cts.IsCancellationRequested) return locationChanged;

                    // Check previous location difference
                    if (lastGPSLocData.query != null
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

                            if (String.IsNullOrEmpty(view.LocationQuery))
                                view = new LocationQueryViewModel();
                        }
                        catch (WeatherException ex)
                        {
                            view = new LocationQueryViewModel();
                        }
                    }, cts.Token);

                    if (String.IsNullOrWhiteSpace(view.LocationQuery))
                    {
                        // Stop since there is no valid query
                        return false;
                    }

                    if (cts.IsCancellationRequested) return locationChanged;

                    // Save oldkey
                    string oldkey = lastGPSLocData.query;

                    // Save location as last known
                    lastGPSLocData.SetData(view, newGeoPos);
                    Settings.SaveLastGPSLocData(lastGPSLocData);

                    // Update tile id for location
                    if (oldkey != null && SecondaryTileUtils.Exists(oldkey))
                    {
                        await SecondaryTileUtils.UpdateTileId(oldkey, lastGPSLocData.query);
                    }

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