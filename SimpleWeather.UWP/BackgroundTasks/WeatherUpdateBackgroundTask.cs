using SimpleWeather.Utils;
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
        private CancellationTokenSource cts;
        private WeatherManager wm;

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
                Helpers.WeatherTileCreator.TileUpdater(weather);

                if (cts.IsCancellationRequested) return;

                // Set update time
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

                    string selected_query = string.Empty;

                    await Task.Run(async () =>
                    {
                        var view = await wm.GetLocation(newGeoPos);

                        if (!String.IsNullOrEmpty(view.LocationQuery))
                            selected_query = view.LocationQuery;
                        else
                            selected_query = string.Empty;
                    }, cts.Token);

                    if (String.IsNullOrWhiteSpace(selected_query))
                    {
                        // Stop since there is no valid query
                        return false;
                    }

                    if (cts.IsCancellationRequested) return locationChanged;

                    // Save location as last known
                    lastGPSLocData.SetData(selected_query, newGeoPos);
                    Settings.SaveLastGPSLocData();

                    locationChanged = true;
                }
            }

            return locationChanged;
        }
    }
}
