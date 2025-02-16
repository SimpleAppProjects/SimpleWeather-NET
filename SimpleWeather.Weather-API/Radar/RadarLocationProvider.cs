using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.Radar
{
    public class RadarLocationProvider : WeatherLocationProviderImpl
    {
        private const string AUTOCOMPLETE_URL = "https://api.radar.io/v1/search/autocomplete";
        private const string REVERSE_GEOCODE_URL = "https://api.radar.io/v1/geocode/reverse";

        public override string LocationAPI => WeatherAPI.Radar;

        public override bool KeyRequired => false;

        public override bool SupportsLocale => true;

        public override bool NeedsLocationFromName => false;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<ObservableCollection<LocationQuery>> GetLocations(string location_query, string weatherAPI)
        {
            ObservableCollection<LocationQuery> locations = null;

            var culture = LocaleUtils.GetLocale();
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            var key = GetAPIKey();

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

                Uri queryURL = AUTOCOMPLETE_URL.ToUriBuilderEx()
                    .AppendQueryParameter("query", location_query)
                    .AppendQueryParameter("layers", "neighborhood,locality,county,state,postalCode")
                    .AppendQueryParameter("limit", maxResults.ToInvariantString())
                    .AppendQueryParameter("lang", locale)
                    .BuildUri();

                using var request = new HttpRequestMessage(HttpMethod.Get, queryURL);
                request.Headers.Authorization = new AuthenticationHeaderValue(key);
                request.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    MaxAge = TimeSpan.FromDays(30)
                };

                // Connect to webstream
                var webClient = SharedModule.Instance.WebClient;
                using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);

                using var response = await webClient.SendAsync(request, cts.Token);
                await this.CheckForErrors(response);
                response.EnsureSuccessStatusCode();

                Stream contentStream = await response.Content.ReadAsStreamAsync();

                // Load data
                var locationSet = new HashSet<LocationQuery>();
                var root = await JSONParser.DeserializerAsync<AutocompleteRootobject>(contentStream);

                foreach (Address result in root.addresses)
                {
                    // Filter: only store city results
                    bool added = locationSet.Add(this.CreateLocationModel(result, weatherAPI));

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

                Logger.WriteLine(LoggerLevel.Error, ex, "RadarLocationProvider: error getting locations");
            }

            if (wEx != null)
                throw wEx;

            if (locations == null || locations.Count == 0)
                locations = [new LocationQuery()];

            return locations;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override Task<LocationQuery> GetLocationFromID(LocationQuery model)
        {
            return Task.FromResult<LocationQuery>(null);
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override Task<LocationQuery> GetLocationFromName(LocationQuery model)
        {
            return Task.FromResult<LocationQuery>(null);
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coord, string weatherAPI)
        {
            LocationQuery location = null;

            var culture = LocaleUtils.GetLocale();
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            var key = GetAPIKey();

            string location_query = string.Format(CultureInfo.InvariantCulture, "{0:0.##},{1:0.##}", coord.Latitude, coord.Longitude);

            GeocodeAddress result = null;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                }

                Uri queryURL = REVERSE_GEOCODE_URL.ToUriBuilderEx()
                    .AppendQueryParameter("coordinates", location_query)
                    .AppendQueryParameter("layers", "neighborhood,locality,county,state,postalCode")
                    .AppendQueryParameter("lang", locale)
                    .BuildUri();

                using var request = new HttpRequestMessage(HttpMethod.Get, queryURL);
                request.Headers.Authorization = new AuthenticationHeaderValue(key);
                request.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    MaxAge = TimeSpan.FromDays(30)
                };

                // Connect to webstream
                var webClient = SharedModule.Instance.WebClient;
                using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);

                using var response = await webClient.SendAsync(request, cts.Token);
                await this.CheckForErrors(response);
                response.EnsureSuccessStatusCode();

                Stream contentStream = await response.Content.ReadAsStreamAsync();

                // Load data
                var root = await JSONParser.DeserializerAsync<GeocodeRootobject>(contentStream);

                foreach (GeocodeAddress item in root.addresses)
                {
                    if (Math.Abs(ConversionMethods.CalculateHaversine(coord.Latitude, coord.Longitude, item.latitude, item.longitude)) <= 1000)
                    {
                        result = item;
                        break;
                    }
                }

                result ??= root.addresses[0];
            }
            catch (Exception ex)
            {
                result = null;

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

                Logger.WriteLine(LoggerLevel.Error, ex, "RadarLocationProvider: error getting location");
            }

            if (wEx != null)
                throw wEx;

            if (result != null)
                location = this.CreateLocationModel(result, weatherAPI);
            else
                location = new LocationQuery();

            return location;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override Task<bool> IsKeyValid(string key)
        {
            return Task.FromResult(false);
        }

        public override String GetAPIKey()
        {
            return APIKeys.GetRadarKey();
        }

        public override string LocaleToLangCode(string iso, string name)
        {
            string code = "en";

            switch (iso)
            {
                case "ar":
                case "de":
                case "en":
                case "es":
                case "fr":
                case "ja":
                case "ko":
                case "pt":
                case "ru":
                case "zh":
                    code = iso;
                    break;
                default:
                    code = "en";
                    break;
            }

            return code;
        }
    }
}