using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;

namespace SimpleWeather.UWP.BackgroundTasks
{
    public sealed class WeatherUpdateBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            if (Settings.WeatherLoaded)
            {
                // Retrieve weather data.
                var weather = await GetWeather();

                // Update the live tile with data.
                Helpers.WeatherTileCreator.TileUpdater(weather);

                // Set update time
                Settings.UpdateTime = DateTime.Now;
            }

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        private async Task<Weather> GetWeather()
        {
            Weather weather = null;

            try
            {
                if (Settings.FollowGPS)
                    await UpdateLocation();

                var wloader = new WeatherDataLoader(null, Settings.HomeData);
                /*Task t = */ await wloader.LoadWeatherData(false);
                //t.Wait();

                weather = wloader.GetWeather();
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
                Geolocator geolocal = new Geolocator() { DesiredAccuracyInMeters = 5000, ReportInterval = 900000, MovementThreshold = 2500 };

                try
                {
                    newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10));
                }
                catch (Exception)
                {
                    GeolocationAccessStatus geoStatus = GeolocationAccessStatus.Unspecified;

                    try
                    {
                        geoStatus = await Geolocator.RequestAccessAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.StackTrace);
                    }
                    finally
                    {
                        if (geoStatus == GeolocationAccessStatus.Allowed)
                        {
                            newGeoPos = await geolocal.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10));
                        }
                    }
                }

                // Access to location granted
                if (newGeoPos != null)
                {
                    LocationData lastGPSLocData = await Settings.GetLastGPSLocData();

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
                        var view = await GeopositionQuery.GetLocation(newGeoPos);

                        if (!String.IsNullOrEmpty(view.LocationQuery))
                            selected_query = view.LocationQuery;
                        else
                            selected_query = string.Empty;
                    });

                    if (String.IsNullOrWhiteSpace(selected_query))
                    {
                        // Stop since there is no valid query
                        return false;
                    }

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
