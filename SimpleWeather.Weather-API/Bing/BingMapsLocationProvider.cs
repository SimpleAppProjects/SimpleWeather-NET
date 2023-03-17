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
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.Bing
{
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
                Logger.WriteLine(LoggerLevel.Error, ex, "BingMapsLocationProvider: error getting locations");
            }

            if (wEx != null)
                throw wEx;

            if (locations == null || locations.Count == 0)
                locations = new ObservableCollection<LocationQuery>() { new LocationQuery() };

            return locations;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override partial Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coord, string weatherAPI);

        public override Task<LocationQuery> GetLocationFromID(LocationQuery model)
        {
            return Task.FromResult<LocationQuery>(null);
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override partial Task<LocationQuery> GetLocationFromName(LocationQuery model);

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override partial Task<bool> IsKeyValid(string key);

        public override string GetAPIKey()
        {
            return APIKeys.GetBingMapsKey();
        }
    }
}