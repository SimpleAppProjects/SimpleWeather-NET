using SimpleWeather.Controls;
using SimpleWeather.Keys;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using Windows.Web;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace SimpleWeather.WeatherApi
{
    public class WeatherApiLocationProvider : LocationProviderImpl
    {
        private const string QUERY_URL = "https://api.weatherapi.com/v1/search.json?key={0}&q={1}&lang={2}";

        public override string LocationAPI => WeatherAPI.WeatherApi;

        public override bool KeyRequired => false;

        public override bool SupportsLocale => true;

        public override bool NeedsLocationFromName => true;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string location_query, string weatherAPI)
        {
            ObservableCollection<LocationQueryViewModel> locations = null;

            var culture = CultureUtils.UserCulture;
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            var key = GetAPIKey();

            WeatherException wEx = null;
            // Limit amount of results shown
            int maxResults = 10;

            try
            {
                this.CheckRateLimit();

                Uri queryURL = new Uri(String.Format(QUERY_URL, key, location_query, locale));

                using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                {
                    request.Headers.CacheControl.MaxAge = TimeSpan.FromDays(1);

                    // Connect to webstream
                    var webClient = SimpleLibrary.GetInstance().WebClient;
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                    {
                        this.CheckForErrors(response.StatusCode);
                        response.EnsureSuccessStatusCode();

                        Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                        // Load data
                        var locationSet = new HashSet<LocationQueryViewModel>();
                        LocationItem[] root = await JSONParser.DeserializerAsync<LocationItem[]>(contentStream);

                        foreach (LocationItem result in root)
                        {
                            // Filter: only store city results
                            bool added = locationSet.Add(new LocationQueryViewModel(result, weatherAPI));

                            // Limit amount of results
                            if (added)
                            {
                                maxResults--;
                                if (maxResults <= 0)
                                    break;
                            }
                        }

                        locations = new ObservableCollection<LocationQueryViewModel>(locationSet);
                    }
                }
            }
            catch (Exception ex)
            {
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                }
                else if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherApiLocationProvider: error getting locations");
            }

            if (wEx != null)
                throw wEx;

            if (locations == null || locations.Count == 0)
                locations = new ObservableCollection<LocationQueryViewModel>() { new LocationQueryViewModel() };

            return locations;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override Task<LocationQueryViewModel> GetLocationFromID(LocationQueryViewModel model)
        {
            return Task.FromResult<LocationQueryViewModel>(null);
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override Task<LocationQueryViewModel> GetLocationFromName(LocationQueryViewModel model)
        {
            return base.GetLocationFromName(model);
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coord, string weatherAPI)
        {
            LocationQueryViewModel location = null;

            var culture = CultureUtils.UserCulture;
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            var key = GetAPIKey();

            string location_query = string.Format(CultureInfo.InvariantCulture, "{0:0.##},{1:0.##}", coord.Latitude, coord.Longitude);

            LocationItem result = null;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                Uri queryURL = new Uri(String.Format(QUERY_URL, key, location_query, locale));

                using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                {
                    request.Headers.CacheControl.MaxAge = TimeSpan.FromDays(1);

                    // Connect to webstream
                    var webClient = SimpleLibrary.GetInstance().WebClient;
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                    {
                        this.CheckForErrors(response.StatusCode);
                        response.EnsureSuccessStatusCode();

                        Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

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

                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                }
                else if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherApiLocationProvider: error getting location");
            }

            if (wEx != null)
                throw wEx;

            if (result != null)
                location = new LocationQueryViewModel(result, weatherAPI);
            else
                location = new LocationQueryViewModel();

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