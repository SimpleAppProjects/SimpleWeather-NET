using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Bing;
using SimpleWeather.Weather_API.TZDB;
using SimpleWeather.Weather_API.Utils;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.Web;

namespace SimpleWeather.LocationData
{
    public abstract partial class WeatherLocationProviderImpl : IWeatherLocationProvider, IRateLimitedRequest
    {
        protected readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();
        protected readonly ITZDBService TZDBService = Ioc.Default.GetService<ITZDBService>();

        // Variables
        public abstract string LocationAPI { get; }
        public abstract bool KeyRequired { get; }
        public abstract bool SupportsLocale { get; }
        public virtual bool NeedsLocationFromID => false;
        public virtual bool NeedsLocationFromName => false;
        public virtual bool NeedsLocationFromGeocoder => false;

        public virtual long GetRetryTime() => 5000;

        // Methods
        // AutoCompleteQuery
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<ObservableCollection<LocationQuery>> GetLocations(String ac_query, String weatherAPI);
        // GeopositionQuery
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coordinate, String weatherAPI);

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<LocationQuery> GetLocationFromID(LocationQuery model);

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public virtual async Task<LocationQuery> GetLocationFromName(LocationQuery model)
        {
            LocationQuery location = null;

            var culture = CultureUtils.UserCulture;

            MapLocation result = null;
            WeatherException wEx = null;

            try
            {
                MapService.ServiceToken = APIKeys.GetBingMapsKey();
                // The nearby location to use as a query hint.
                BasicGeoposition queryHint = new BasicGeoposition
                {
                    Latitude = 0,
                    Longitude = 0
                };
                Geopoint hintPoint = new Geopoint(queryHint);

                using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                {
                    // Geocode the specified address, using the specified reference point
                    // as a query hint. Return no more than a single result.
                    MapLocationFinderResult mapResult = await MapLocationFinder.FindLocationsAsync(model.LocationName, hintPoint, 1).AsTask(cts.Token);

                    switch (mapResult.Status)
                    {
                        case MapLocationFinderStatus.Success:
                            result = mapResult.Locations[0];
                            break;

                        case MapLocationFinderStatus.UnknownError:
                        case MapLocationFinderStatus.IndexFailure:
                            wEx = new WeatherException(WeatherUtils.ErrorStatus.Unknown);
                            break;

                        case MapLocationFinderStatus.InvalidCredentials:
                            wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                            break;

                        case MapLocationFinderStatus.BadLocation:
                        case MapLocationFinderStatus.NotSupported:
                            wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
                            break;

                        case MapLocationFinderStatus.NetworkFailure:
                            wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown || ex is HttpRequestException || ex is SocketException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting location");
            }

            if (wEx != null)
                throw wEx;

            if (result != null && !String.IsNullOrWhiteSpace(result.DisplayName))
                location = this.CreateLocationModel(result, model.WeatherSource);
            else
                location = new LocationQuery();

            location.LocationSource = LocationAPI;

            return location;
        }

        // KeyCheck
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<bool> IsKeyValid(String key);
        public abstract String GetAPIKey();

        // Utils Methods
        public virtual async Task UpdateLocationData(LocationData location, String weatherAPI)
        {
            LocationQuery qview = null;
            try
            {
                qview = await GetLocation(new WeatherUtils.Coordinate(location), weatherAPI);
            }
            catch (WeatherException wEx)
            {
                Logger.WriteLine(LoggerLevel.Error, wEx, "LocationProviderImpl: UpdateLocationData: WeatherException!");
            }

            if (qview != null && !String.IsNullOrWhiteSpace(qview.Location_Query))
            {
                location.name = qview.LocationName;
                location.latitude = qview.LocationLat;
                location.longitude = qview.LocationLong;
                location.tz_long = qview.LocationTZLong;
                if (String.IsNullOrWhiteSpace(qview.LocationTZLong) && location.latitude != 0 && location.longitude != 0)
                {
                    String tzId = await TZDBService.GetTimeZone(location.latitude, location.longitude);
                    if (!Equals("unknown", tzId))
                        location.tz_long = tzId;
                }
                location.locationSource = qview.LocationSource;

                // Update DB here or somewhere else
                await SettingsManager.UpdateLocation(location);
            }
        }

        public virtual String LocaleToLangCode(String iso, String name) { return "EN"; }
    }
}