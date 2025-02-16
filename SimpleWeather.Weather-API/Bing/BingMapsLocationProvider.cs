using BingMapsRESTToolkit;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.Bing
{
    [Obsolete("Bing Maps service will shut down June 30, 2025")]
    public partial class BingMapsLocationProvider : WeatherLocationProviderImpl, IRateLimitedRequest
    {
        public override string LocationAPI => WeatherAPI.BingMaps;

        public override bool KeyRequired => false;

        public override bool SupportsLocale => true;

        public override bool NeedsLocationFromName => true;

        public override long GetRetryTime() => 5000;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<ObservableCollection<LocationQuery>> GetLocations(string location_query, string weatherAPI)
        {
            // http://dev.virtualearth.net/REST/v1/Autosuggest?query=new%20york&userLocation=0,0&includeEntityTypes=Place&key=API_KEY&culture=fr-FR&userRegion=FR
            //private const string AUTOCOMPLETE_QUERY_URL = "http://dev.virtualearth.net/REST/v1/Autosuggest?query={0}&userLocation=0,0&includeEntityTypes=Place&key={1}&culture={2}&maxResults=10";
            ObservableCollection<LocationQuery> locations = null;

            var culture = LocaleUtils.GetLocale();

            string key = GetAPIKey();

            WeatherException wEx = null;
            // Limit amount of results shown
            int maxResults = 10;

            try
            {
                this.CheckRateLimit();

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                }

                using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);

                var request = new AutosuggestRequest()
                {
                    BingMapsKey = key,
                    Culture = culture.Name,
                    IncludeEntityTypes = new List<AutosuggestEntityType>
                    {
                        AutosuggestEntityType.Place
                    },
                    MaxResults = 10,
                    Query = location_query,
                    UserLocation = new Coordinate(),
                };

                var response = await request.Execute();

                switch (response.StatusCode)
                {
                    case 200: // OK
                    case 201: // Created
                    case 202: // Accepted
                        break;
                    case 400: // Bad Request
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound, new Exception(JSONParser.Serializer(response)));
                        break;
                    case 401: // Unauthorized
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey, new Exception(JSONParser.Serializer(response)));
                        break;
                    case 429: // Too Many Requests
                        this.SetRateLimit();
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, new Exception(JSONParser.Serializer(response)));
                        break;
                    case 500: // Server Error
                    case 503: // Service Unavailable
                    default:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.Unknown, new Exception(JSONParser.Serializer(response)));
                        break;
                }

                if (wEx != null)
                    throw wEx;

                // Load data
                var locationSet = new HashSet<LocationQuery>();
                var results = response.ResourceSets[0].Resources[0] as Autosuggest;

                foreach (var result in results.Value)
                {
                    // Filter: only store city results
                    bool added = false;
                    if (!string.IsNullOrWhiteSpace(result.Address.Locality) || Equals(result.Type, "Place"))
                        added = locationSet.Add(this.CreateLocationModel(result, weatherAPI));
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
            catch (Exception ex)
            {
                if (ex is HttpRequestException || ex is WebException || ex is SocketException || ex is IOException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                else if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }
                else
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound, ex);
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

            var culture = LocaleUtils.GetLocale();

            string key = GetAPIKey();

            BingMapsRESTToolkit.Location result = null;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                }

                using var cts = new CancellationTokenSource(Preferences.SettingsManager.READ_TIMEOUT);

                var request = new ReverseGeocodeRequest()
                {
                    BingMapsKey = key,
                    Culture = culture.Name,
                    IncludeIso2 = true,
                    IncludeNeighborhood = true,
                    IncludeEntityTypes = new List<EntityType>
                    {
                        EntityType.PopulatedPlace,
                        EntityType.Neighborhood,
                    },
                    Point = new Coordinate(coord.Latitude, coord.Longitude),
                };

                var response =
#if WINDOWS || NETSTANDARD2_0
                    await request.Execute().AsAsyncOperation().AsTask(cts.Token);
#else
                    await request.Execute().WaitAsync(cts.Token);
#endif

                switch (response.StatusCode)
                {
                    case 200: // OK
                    case 201: // Created
                    case 202: // Accepted
                        result = response?.ResourceSets?.FirstOrDefault()?.Resources?.FirstOrDefault() as BingMapsRESTToolkit.Location;
                        break;
                    case 400: // Bad Request
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound, new Exception(JSONParser.Serializer(response)));
                        break;
                    case 401: // Unauthorized
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey, new Exception(JSONParser.Serializer(response)));
                        break;
                    case 429: // Too Many Requests
                        this.SetRateLimit();
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, new Exception(JSONParser.Serializer(response)));
                        break;
                    case 500: // Server Error
                    case 503: // Service Unavailable
                    default:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.Unknown, new Exception(JSONParser.Serializer(response)));
                        break;
                }
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException || ex is WebException || ex is SocketException || ex is IOException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting location");
            }

            if (wEx != null)
                throw wEx;

            if (result != null && !string.IsNullOrWhiteSpace(result.Name))
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

            var culture = LocaleUtils.GetLocale();

            string key = GetAPIKey();

            BingMapsRESTToolkit.Location result = null;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                }

                using var cts = new CancellationTokenSource(Preferences.SettingsManager.READ_TIMEOUT);

                var request = new GeocodeRequest()
                {
                    BingMapsKey = key,
                    Culture = culture.Name,
                    IncludeIso2 = true,
                    IncludeNeighborhood = true,
                    MaxResults = 1,
                    Query = model.Location_Query,
                    UserLocation = new Coordinate(0, 0),
                };

                var response =
#if WINDOWS || NETSTANDARD2_0
                    await request.Execute().AsAsyncOperation().AsTask(cts.Token);
#else
                    await request.Execute().WaitAsync(cts.Token);
#endif

                switch (response.StatusCode)
                {
                    case 200: // OK
                    case 201: // Created
                    case 202: // Accepted
                        result = response?.ResourceSets?.FirstOrDefault()?.Resources?.FirstOrDefault() as BingMapsRESTToolkit.Location;
                        break;
                    case 400: // Bad Request
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound, new Exception(JSONParser.Serializer(response)));
                        break;
                    case 401: // Unauthorized
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey, new Exception(JSONParser.Serializer(response)));
                        break;
                    case 429: // Too Many Requests
                        this.SetRateLimit();
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, new Exception(JSONParser.Serializer(response)));
                        break;
                    case 500: // Server Error
                    case 503: // Service Unavailable
                    default:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.Unknown, new Exception(JSONParser.Serializer(response)));
                        break;
                }
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException || ex is WebException || ex is SocketException || ex is IOException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting location");
            }

            if (wEx != null)
                throw wEx;

            if (result != null && !string.IsNullOrWhiteSpace(result.Name))
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

            try
            {
                var request = new ReverseGeocodeRequest()
                {
                    BingMapsKey = key,
                    Point = new Coordinate(0, 0),
                };

                var response = await request.Execute();

                switch (response.StatusCode)
                {
                    case 200: // OK
                    case 201: // Created
                    case 202: // Accepted
                    case 400: // Bad Request
                    case 429: // Too Many Requests
                        isValid = true;
                        break;
                    case 401: // Unauthorized
                        isValid = false;
                        break;
                    case 500: // Server Error
                    case 503: // Service Unavailable
                    default:
                        isValid = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException || ex is WebException || ex is SocketException || ex is IOException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting location");
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