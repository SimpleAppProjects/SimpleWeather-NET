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

namespace SimpleWeather.Weather_API.WeatherApi
{
    public class WeatherApiLocationProvider : WeatherLocationProviderImpl
    {
        private const string QUERY_URL = "https://api.weatherapi.com/v1/search.json?key={0}&q={1}&lang={2}";

        public override string LocationAPI => WeatherAPI.WeatherApi;

        public override bool KeyRequired => false;

        public override bool SupportsLocale => true;

        public override bool NeedsLocationFromName => true;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<ObservableCollection<LocationQuery>> GetLocations(string location_query, string weatherAPI)
        {
            ObservableCollection<LocationQuery> locations = null;

            var culture = LocaleUtils.GetLocale();
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            var key = GetProviderKey();

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

                Uri queryURL = new Uri(String.Format(QUERY_URL, key, location_query, locale));

                using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                {
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(14)
                    };

                    // Connect to webstream
                    var webClient = SharedModule.Instance.WebClient;
                    using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                    using (var response = await webClient.SendAsync(request, cts.Token))
                    {
                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        Stream contentStream = await response.Content.ReadAsStreamAsync();

                        // Load data
                        var locationSet = new HashSet<LocationQuery>();
                        LocationItem[] root = await JSONParser.DeserializerAsync<LocationItem[]>(contentStream);

                        foreach (LocationItem result in root)
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
                }
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

                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherApiLocationProvider: error getting locations");
            }

            if (wEx != null)
                throw wEx;

            if (locations == null || locations.Count == 0)
                locations = new ObservableCollection<LocationQuery>() { new LocationQuery() };

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
            return Task.FromResult(model);
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coord, string weatherAPI)
        {
            LocationQuery location = null;

            var culture = LocaleUtils.GetLocale();
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            var key = GetProviderKey();

            string location_query = string.Format(CultureInfo.InvariantCulture, "{0:0.##},{1:0.##}", coord.Latitude, coord.Longitude);

            LocationItem result = null;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                }

                Uri queryURL = new Uri(String.Format(QUERY_URL, key, location_query, locale));

                using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                {
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(14)
                    };

                    // Connect to webstream
                    var webClient = SharedModule.Instance.WebClient;
                    using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                    using (var response = await webClient.SendAsync(request, cts.Token))
                    {
                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        Stream contentStream = await response.Content.ReadAsStreamAsync();

                        // Load data
                        LocationItem[] root = await JSONParser.DeserializerAsync<LocationItem[]>(contentStream);

                        foreach (LocationItem item in root)
                        {
                            if (Math.Abs(ConversionMethods.CalculateHaversine(coord.Latitude, coord.Longitude, item.lat, item.lon)) <= 1000)
                            {
                                result = item;
                                break;
                            }
                        }

                        if (result == null)
                        {
                            result = root[0];
                        }
                    }
                }
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

                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherApiLocationProvider: error getting location");
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
            return APIKeys.GetWeatherApiKey();
        }

        public override string LocaleToLangCode(string iso, string name)
        {
            String code = "en";

            switch (iso)
            {
                // Chinese
                case "zh":
                    switch (name)
                    {
                        // Chinese - Traditional
                        case "zh-Hant":
                        case "zh-HK":
                        case "zh-MO":
                        case "zh-TW":
                            code = "zh_tw";
                            break;
                        // Mandarin
                        case "zh-cmn":
                            code = "zh_cmn";
                            break;
                        // Wu
                        case "zh-wuu":
                            code = "zh_wuu";
                            break;
                        // Xiang
                        case "zh-hsn":
                            code = "zh_hsn";
                            break;
                        // Cantonese
                        case "zh-yue":
                            code = "zh_yue";
                            break;
                        // Chinese - Simplified
                        default:
                            code = "zh";
                            break;
                    }
                    break;
                default:
                    code = iso;
                    break;
            }

            return code;
        }
    }
}