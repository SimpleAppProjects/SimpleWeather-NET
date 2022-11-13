using SimpleWeather.Weather_API.Keys;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.Web;

namespace SimpleWeather.Weather_API.Bing
{
    public class BingMapsLocationProvider : WeatherLocationProviderImpl
    {
        // http://dev.virtualearth.net/REST/v1/Autosuggest?query=new%20york&userLocation=0,0&includeEntityTypes=Place&key=API_KEY&culture=fr-FR&userRegion=FR
        private const string AUTOCOMPLETE_QUERY_URL = "http://dev.virtualearth.net/REST/v1/Autosuggest?query={0}&userLocation=0,0&includeEntityTypes=Place&key={1}&culture={2}&maxResults=10";

        public override string LocationAPI => WeatherAPI.BingMaps;

        public override bool KeyRequired => false;

        public override bool SupportsLocale => true;

        public override bool NeedsLocationFromName => true;

        public override long GetRetryTime() => 5000;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<ObservableCollection<LocationQuery>> GetLocations(string location_query, string weatherAPI)
        {
            ObservableCollection<LocationQuery> locations = null;

            var culture = CultureUtils.UserCulture;

            string key = GetAPIKey();

            WeatherException wEx = null;
            // Limit amount of results shown
            int maxResults = 10;

            try
            {
                this.CheckRateLimit();

                Uri queryURL = new Uri(string.Format(AUTOCOMPLETE_QUERY_URL, location_query, key, culture.Name));

                // Connect to webstream
                var webClient = SharedModule.Instance.WebClient;
                using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                {
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(1)
                    };

                    using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                    using (var response = await webClient.SendAsync(request, cts.Token))
                    {
                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        Stream contentStream = await response.Content.ReadAsStreamAsync();

                        // Load data
                        var locationSet = new HashSet<LocationQuery>();
                        AC_Rootobject root = await JSONParser.DeserializerAsync<AC_Rootobject>(contentStream);

                        foreach (Value result in root.resourceSets[0].resources[0].value)
                        {
                            // Filter: only store city results
                            bool added = false;
                            if (!string.IsNullOrWhiteSpace(result.address.locality))
                                added = locationSet.Add(this.CreateLocationModel(result.address, weatherAPI));
                            else
                                continue;

                            // Limit amount of results
                            if (added)
                            {
                                maxResults--;
                                if (maxResults <= 0)
                                    break;
                            }
                        }

                        locations = new ObservableCollection<LocationQuery>(locationSet);
                    }
                }
            }
            catch (Exception ex)
            {
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown || ex is HttpRequestException || ex is SocketException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                else if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting locations");
            }

            if (wEx != null)
                throw wEx;

            if (locations == null || locations.Count == 0)
                locations = new ObservableCollection<LocationQuery>() { new LocationQuery() };

            return locations;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coord, string weatherAPI)
        {
            LocationQuery location = null;

            var culture = CultureUtils.UserCulture;

            string key = GetAPIKey();

            MapLocation result = null;
            WeatherException wEx = null;

            try
            {
                MapService.ServiceToken = key;
                // The nearby location to use as a query hint.
                BasicGeoposition geoPoint = new BasicGeoposition
                {
                    Latitude = coord.Latitude,
                    Longitude = coord.Longitude
                };
                Geopoint pointToReverseGeocode = new Geopoint(geoPoint);

                using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                {
                    // Geocode the specified address, using the specified reference point
                    // as a query hint. Return no more than a single result.
                    MapLocationFinderResult mapResult = await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode, MapLocationDesiredAccuracy.High).AsTask(cts.Token);

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
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown || ex is HttpRequestException || ex is SocketException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting location");
            }

            if (wEx != null)
                throw wEx;

            if (result != null && !string.IsNullOrWhiteSpace(result.DisplayName))
                location = this.CreateLocationModel(result, weatherAPI);
            else
                location = new LocationQuery();

            return location;
        }

        public override Task<LocationQuery> GetLocationFromID(LocationQuery model)
        {
            return Task.FromResult<LocationQuery>(null);
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<LocationQuery> GetLocationFromName(LocationQuery model)
        {
            LocationQuery location = null;

            var culture = CultureUtils.UserCulture;

            string key = GetAPIKey();

            MapLocation result = null;
            WeatherException wEx = null;

            try
            {
                MapService.ServiceToken = key;
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
                    MapLocationFinderResult mapResult = await MapLocationFinder.FindLocationsAsync(model.Location_Query, hintPoint, 1).AsTask(cts.Token);

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

            if (result != null && !string.IsNullOrWhiteSpace(result.DisplayName))
                location = this.CreateLocationModel(result, model.WeatherSource);
            else
                location = new LocationQuery();

            return location;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<bool> IsKeyValid(string key)
        {
            bool isValid = false;
            WeatherException wEx = null;

            MapService.ServiceToken = key;
            // The nearby location to use as a query hint.
            BasicGeoposition geoPoint = new BasicGeoposition
            {
                Latitude = 0,
                Longitude = 0
            };
            Geopoint pointToReverseGeocode = new Geopoint(geoPoint);

            // Geocode the specified address, using the specified reference point
            // as a query hint. Return no more than a single result.
            MapLocationFinderResult mapResult = await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode, MapLocationDesiredAccuracy.Low);

            switch (mapResult.Status)
            {
                case MapLocationFinderStatus.UnknownError:
                case MapLocationFinderStatus.IndexFailure:
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.Unknown);
                    isValid = false;
                    break;

                case MapLocationFinderStatus.InvalidCredentials:
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                    isValid = false;
                    break;

                case MapLocationFinderStatus.NetworkFailure:
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    isValid = false;
                    break;

                case MapLocationFinderStatus.Success:
                case MapLocationFinderStatus.NotSupported:
                case MapLocationFinderStatus.BadLocation:
                    isValid = true;
                    break;
            }

            if (wEx != null)
                throw wEx;

            return isValid;
        }

        public override string GetAPIKey()
        {
            return APIKeys.GetBingMapsKey();
        }
    }
}