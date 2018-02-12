using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.WeatherAlerts;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;

namespace SimpleWeather.UWP.BackgroundTasks
{
    public sealed class WeatherUpdateBackgroundTask : IBackgroundTask
    {
        private const string taskName = "WeatherUpdateBackgroundTask";
        private CancellationTokenSource cts;
        private WeatherManager wm;
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
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            if (Settings.WeatherLoaded)
            {
                // Retrieve weather data.
                var weather = await GetWeather();

                if (cts.IsCancellationRequested) return;

                // Update the live tile with data.
                if (weather != null)
                    WeatherTileCreator.TileUpdater(weather);

                if (cts.IsCancellationRequested) return;

                // Post alerts if setting is on
                if (Settings.ShowAlerts && wm.SupportsAlerts && weather != null)
                    await WeatherAlertHandler.PostAlerts(Settings.HomeData, weather.weather_alerts);

                if (cts.IsCancellationRequested) return;

                // Set update time
                if (weather != null)
                    Settings.UpdateTime = DateTime.Now;
            }

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // TODO: Add code to notify the background task that it is cancelled.
            cts.Cancel();
            Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
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
                await AppTrigger.RequestAsync();
            }
            else
            {
                Debug.WriteLine("BackgroundTaskHandler: Can't trigger ApplicationTrigger, background access not allowed");
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

                var wloader = new WeatherDataLoader(null, Settings.HomeData);
                await wloader.LoadWeatherData(false);

                weather = wloader.GetWeather();
            }
            catch (OperationCanceledException cancelEx)
            {
                Debug.WriteLine(cancelEx.ToString());
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
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
                    newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10));
                }
                catch (OperationCanceledException)
                {
                    return locationChanged;
                }
                catch (Exception)
                {
                    GeolocationAccessStatus geoStatus = GeolocationAccessStatus.Unspecified;

                    try
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        geoStatus = await Geolocator.RequestAccessAsync();
                    }
                    catch (OperationCanceledException)
                    {
                        return locationChanged;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.StackTrace);
                    }
                    finally
                    {
                        if (!cts.IsCancellationRequested && geoStatus == GeolocationAccessStatus.Allowed)
                        {
                            newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10));
                        }
                    }
                }

                // Access to location granted
                if (newGeoPos != null)
                {
                    LocationData lastGPSLocData = await Settings.GetLastGPSLocData();

                    if (cts.IsCancellationRequested) return locationChanged;

                    // Check previous location difference
                    if (lastGPSLocData.query != null &&
                        Math.Abs(ConversionMethods.CalculateHaversine(lastGPSLocData.latitude, lastGPSLocData.longitude,
                        newGeoPos.Coordinate.Point.Position.Latitude, newGeoPos.Coordinate.Point.Position.Longitude)) < geolocal.MovementThreshold)
                    {
                        return false;
                    }

                    LocationQueryViewModel view = null;

                    await Task.Run(async () =>
                    {
                        view = await wm.GetLocation(newGeoPos);

                        if (String.IsNullOrEmpty(view.LocationQuery))
                            view = new LocationQueryViewModel();
                    }, cts.Token);

                    if (String.IsNullOrWhiteSpace(view.LocationQuery))
                    {
                        // Stop since there is no valid query
                        return false;
                    }

                    if (cts.IsCancellationRequested) return locationChanged;

                    // Save location as last known
                    lastGPSLocData.SetData(view, newGeoPos);
                    Settings.SaveLastGPSLocData();

                    locationChanged = true;
                }
            }

            return locationChanged;
        }
    }
}
