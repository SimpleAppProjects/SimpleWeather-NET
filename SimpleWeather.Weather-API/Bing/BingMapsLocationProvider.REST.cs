#if !WINDOWS_UWP
using BingMapsRESTToolkit;
using SimpleWeather.LocationData;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.Bing
{
    public partial class BingMapsLocationProvider
    {
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override partial async Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coord, string weatherAPI)
        {
            LocationQuery location = null;

            var culture = CultureUtils.UserCulture;

            string key = GetAPIKey();

            BingMapsRESTToolkit.Location result = null;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

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
#if WINDOWS_UWP || NETSTANDARD2_0
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

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async partial Task<LocationQuery> GetLocationFromName(LocationQuery model)
        {
            LocationQuery location = null;

            var culture = CultureUtils.UserCulture;

            string key = GetAPIKey();

            BingMapsRESTToolkit.Location result = null;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

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
#if WINDOWS_UWP || NETSTANDARD2_0
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
        public override partial async Task<bool> IsKeyValid(string key)
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
    }
}
#endif