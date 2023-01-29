using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common.Utils;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.TZDB;
using System;
using System.Threading;
using System.Threading.Tasks;
#if __ANDROID__
using Android.Locations;
using AndroidX.Core.Content;
#endif
#if WINUI
using Windows.Devices.Geolocation;
#else
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
#endif
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Common.Location
{
    public sealed class LocationProvider
    {
        private const string TAG = nameof(LocationProvider);
        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();
        private readonly ITZDBService TZDBService = Ioc.Default.GetService<ITZDBService>();

#if __ANDROID__
        private readonly LocationManager LocationMgr;

        public LocationProvider()
        {
            LocationMgr = Platform.AppContext.GetSystemService(Android.Content.Context.LocationService) as LocationManager;
        }
#endif

        public async Task<bool> CheckPermissions()
        {
#if WINUI
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
#elif __ANDROID__
            return AndroidX.Core.Location.LocationManagerCompat.IsLocationEnabled(LocationMgr) &&
                ((ContextCompat.CheckSelfPermission(Platform.AppContext, Android.Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted) ||
                (ContextCompat.CheckSelfPermission(Platform.AppContext, Android.Manifest.Permission.AccessCoarseLocation) == Android.Content.PM.Permission.Granted));
#else
            return await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted;
#endif
        }

        public async Task<LocationData.Location> GetLastLocation()
        {
            if (!await CheckPermissions())
            {
                return null;
            }

            try
            {
#if WINUI
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
#else
                var location = await Geolocation.Default.GetLastKnownLocationAsync();
                return new LocationData.Location(location.Latitude, location.Longitude);
#endif
            }
            catch
            {
                return null;
            }
        }

        public async Task<LocationData.Location> GetCurrentLocation()
        {
            if (!await CheckPermissions())
            {
                Logger.WriteLine(LoggerLevel.Info, $"{TAG}: Location permission denied...");
                return null;
            }

            try
            {
#if WINUI
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
#else
                var request = new GeolocationRequest(GeolocationAccuracy.Default, TimeSpan.FromMinutes(15));

                var geoposition = await Geolocation.Default.GetLocationAsync(request);

                Logger.WriteLine(LoggerLevel.Info, $"{TAG}: Location update received...");

                return geoposition.ToLocation();
#endif
            }
            catch
            {
                Logger.WriteLine(LoggerLevel.Info, $"{TAG}: Error retrieving location...");
                return null;
            }
        }

        public async Task<LocationResult> GetLatestLocationData(LocationData.LocationData previousLocation = null)
        {
            if (!await CheckPermissions()) return new LocationResult.PermissionDenied();

            var location = await this.RunCatching(async () =>
            {
                return await GetLastLocation();
            }).GetOrNull();

            location ??= await GetCurrentLocation();

            if (location != null)
            {
                LocationData.LocationData lastGPSLocData = await SettingsManager.GetLastGPSLocData();

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

                var wm = WeatherModule.Instance.WeatherManager;

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
                    return new LocationResult.Error(new ErrorMessage.String(ResStrings.error_retrieve_location));
                }
                else if (String.IsNullOrWhiteSpace(view.LocationTZLong) && view.LocationLat != 0 && view.LocationLong != 0)
                {
                    String tzId = await TZDBService.GetTimeZone(view.LocationLat, view.LocationLong);
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
