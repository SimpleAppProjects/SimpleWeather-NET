using SimpleWeather.Controls;
using SimpleWeather.Keys;
using SimpleWeather.Utils;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.Web;

namespace SimpleWeather.Location
{
    public abstract partial class LocationProviderImpl : ILocationProviderImpl
    {
        // Variables
        public abstract string LocationAPI { get; }
        public abstract bool KeyRequired { get; }
        public abstract bool SupportsLocale { get; }
        public virtual bool NeedsLocationFromID => false;
        public virtual bool NeedsLocationFromName => false;
        public virtual bool NeedsLocationFromGeocoder => false;

        // Methods
        // AutoCompleteQuery
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<ObservableCollection<LocationQueryViewModel>> GetLocations(String ac_query, String weatherAPI);
        // GeopositionQuery
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coordinate, String weatherAPI);

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<LocationQueryViewModel> GetLocationFromID(LocationQueryViewModel model);

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public virtual Task<LocationQueryViewModel> GetLocationFromName(LocationQueryViewModel model)
        {
            return Task.Run(async () =>
            {
                LocationQueryViewModel location = null;

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

                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
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
                    if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                    {
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    }
                    Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting location");
                }

                if (wEx != null)
                    throw wEx;

                if (result != null && !String.IsNullOrWhiteSpace(result.DisplayName))
                    location = new LocationQueryViewModel(result, model.WeatherSource);
                else
                    location = new LocationQueryViewModel();

                location.LocationSource = LocationAPI;

                return location;
            });
        }

        // KeyCheck
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<bool> IsKeyValid(String key);
        public abstract String GetAPIKey();

        // Utils Methods
        public virtual Task UpdateLocationData(LocationData location, String weatherAPI)
        {
            return Task.Run(async () =>
            {
                LocationQueryViewModel qview = null;
                try
                {
                    qview = await GetLocation(new WeatherUtils.Coordinate(location), weatherAPI);
                }
                catch (WeatherException wEx)
                {
                    Logger.WriteLine(LoggerLevel.Error, wEx, "LocationProviderImpl: UpdateLocationData: WeatherException!");
                }

                if (qview != null && !String.IsNullOrWhiteSpace(qview.LocationQuery))
                {
                    location.name = qview.LocationName;
                    location.latitude = qview.LocationLat;
                    location.longitude = qview.LocationLong;
                    location.tz_long = qview.LocationTZLong;
                    if (String.IsNullOrEmpty(qview.LocationTZLong) && location.latitude != 0 && location.longitude != 0)
                    {
                        String tzId = await TZDB.TZDBCache.GetTimeZone(location.latitude, location.longitude);
                        if (!String.IsNullOrWhiteSpace(tzId))
                            location.tz_long = tzId;
                    }
                    location.locationSource = qview.LocationSource;

                    // Update DB here or somewhere else
                    await Settings.UpdateLocation(location);
                }
            });
        }

        public virtual String LocaleToLangCode(String iso, String name) { return "EN"; }
    }
}