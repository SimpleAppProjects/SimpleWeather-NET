using SimpleWeather.Utils;
using SimpleWeather.ViewModels;
using SimpleWeather.WeatherData;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace SimpleWeather.Location
{
    public sealed class LocationProvider
    {
        private const string TAG = nameof(LocationProvider);

        public async Task<bool> CheckPermissions()
        {
            var geoStatus = GeolocationAccessStatus.Unspecified;

            try
            {
                geoStatus = await Geolocator.RequestAccessAsync();
            }
            catch (Exception)
            {
                // Access denied
            }

            return geoStatus == GeolocationAccessStatus.Allowed;
        }

        public async Task<Location> GetLastLocation()
        {
            if (!await CheckPermissions())
            {
                return null;
            }

            try
            {
                var geolocator = new Geolocator()
                {
                    DesiredAccuracyInMeters = 5000,
                    ReportInterval = 900000,
                    MovementThreshold = 1600,
                    DesiredAccuracy = PositionAccuracy.Default,
                };

                geolocator.AllowFallbackToConsentlessPositions();
                var geoposition = await geolocator.GetGeopositionAsync()
                    .AsTask(new CancellationTokenSource(10000).Token);
                return geoposition.ToLocation();
            }
            catch
            {
                return null;
            }
        }

        public async Task<Location> GetCurrentLocation()
        {
            if (!await CheckPermissions())
            {
                Logger.WriteLine(LoggerLevel.Info, $"{TAG}: Location permission denied...");
                return null;
            }

            try
            {
                var geolocator = new Geolocator()
                {
                    DesiredAccuracyInMeters = 5000,
                    ReportInterval = 900000,
                    MovementThreshold = 1600,
                    DesiredAccuracy = PositionAccuracy.Default,
                };

                geolocator.AllowFallbackToConsentlessPositions();
                var geoposition = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(15), TimeSpan.FromSeconds(10));

                Logger.WriteLine(LoggerLevel.Info, $"{TAG}: Location update received...");

                return geoposition.ToLocation();
            }
            catch
            {
                Logger.WriteLine(LoggerLevel.Info, $"{TAG}: Error retrieving location...");
                return null;
            }
        }

        public async Task<LocationResult> GetLatestLocationData(LocationData previousLocation = null)
        {
            if (!await CheckPermissions()) return new LocationResult.PermissionDenied();

            var location = await this.RunCatching(async () =>
            {
                return await GetLastLocation();
            }).GetOrNull();

            location ??= await GetCurrentLocation();

            if (location != null)
            {
                LocationData lastGPSLocData = await Settings.GetLastGPSLocData();

                // Check previous location difference
                if (lastGPSLocData?.IsValid() == true
                    && previousLocation != null && ConversionMethods.CalculateGeopositionDistance(previousLocation.ToLocation(), location) < 1600)
                {
                    return new LocationResult.NotChanged(previousLocation);
                }

                if (lastGPSLocData?.IsValid() == true
                    && Math.Abs(
                        ConversionMethods.CalculateHaversine(lastGPSLocData.latitude, lastGPSLocData.longitude,
                        location.Latitude, location.Longitude)
                        ) < 1600)
                {
                    return new LocationResult.NotChanged(previousLocation);
                }

                var wm = WeatherManager.GetInstance();

                LocationQuery view;

                try
                {
                    view = await wm.GetLocation(location);
                }
                catch (WeatherException e)
                {
                    return new LocationResult.Error(new ErrorMessage.WeatherError(e));
                }

                if (view == null || string.IsNullOrWhiteSpace(view.Location_Query))
                {
                    return new LocationResult.Error(new ErrorMessage.String(SharedModule.Instance.ResLoader.GetString("error_retrieve_location")));
                }
                else if (String.IsNullOrWhiteSpace(view.LocationTZLong) && view.LocationLat != 0 && view.LocationLong != 0)
                {
                    String tzId = await TZDB.TZDBCache.GetTimeZone(view.LocationLat, view.LocationLong);
                    if (!Equals("unknown", tzId))
                        view.LocationTZLong = tzId;
                }

                // Save location as last known
                lastGPSLocData = view.ToLocationData(location);

                return new LocationResult.Changed(lastGPSLocData, true);
            }

            return new LocationResult.NotChanged(previousLocation, false);
        }
    }
}
