#if false
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
using static SimpleWeather.Utils.APIRequestUtils;

namespace SimpleWeather.HERE
{
    public class HERELocationProvider : LocationProviderImpl
    {
        private const string AUTOCOMPLETE_QUERY_URL = "https://autocomplete.geocoder.ls.hereapi.com/6.2/suggest.json?query={0}&language={1}&maxresults=10";
        private const string GEOLOCATION_QUERY_URL = "https://reverse.geocoder.ls.hereapi.com/6.2/reversegeocode.json?" +
            "prox={0},150&mode=retrieveAreas&maxresults=1&additionaldata=Country2,true&gen=9&jsonattributes=1" +
            "&locationattributes=adminInfo,timeZone,-mapView,-mapReference&language={1}";
        private const string GEOCODER_QUERY_API = "https://geocoder.ls.hereapi.com/6.2/geocode.json" +
            "?locationid={0}&mode=retrieveAreas&maxresults=1&additionaldata=Country2,true&gen=9&jsonattributes=1" +
            "&locationattributes=adminInfo,timeZone,-mapView,-mapReference&language={1}";

        public override string LocationAPI => WeatherAPI.Here;

        public override bool KeyRequired => false;

        public override bool SupportsLocale => true;

        public override bool NeedsLocationFromID => true;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string location_query, string weatherAPI)
        {
            ObservableCollection<LocationQueryViewModel> locations = null;

            var culture = CultureUtils.UserCulture;

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            WeatherException wEx = null;
            // Limit amount of results shown
            int maxResults = 10;

            try
            {
                this.CheckRateLimit();

                Uri queryURL = new Uri(String.Format(AUTOCOMPLETE_QUERY_URL, location_query, locale));

                using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                {
                    // Add headers to request
                    var token = await HEREOAuthUtils.GetBearerToken();
                    if (!String.IsNullOrWhiteSpace(token))
                        request.Headers.Add("Authorization", token);
                    else
                        throw new WeatherException(WeatherUtils.ErrorStatus.NetworkError);

                    request.Headers.CacheControl.MaxAge = TimeSpan.FromDays(1);

                    // Connect to webstream
                    var webClient = SimpleLibrary.GetInstance().WebClient;
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                    {
                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                        // Load data
                        var locationSet = new HashSet<LocationQueryViewModel>();
                        AC_Rootobject root = JSONParser.Deserializer<AC_Rootobject>(contentStream);

                        foreach (Suggestion result in root.suggestions)
                        {
                            // Filter: only store city results
                            bool added = false;
                            if ("city".Equals(result.matchLevel)
                                    || "district".Equals(result.matchLevel)
                                    || "postalCode".Equals(result.matchLevel))
                                added = locationSet.Add(new LocationQueryViewModel(result, weatherAPI));
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
                Logger.WriteLine(LoggerLevel.Error, ex, "HEREWeatherProvider: error getting locations");
            }

            if (wEx != null)
                throw wEx;

            if (locations == null || locations.Count == 0)
                locations = new ObservableCollection<LocationQueryViewModel>() { new LocationQueryViewModel() };

            return locations;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coord, string weatherAPI)
        {
            LocationQueryViewModel location = null;

            var culture = CultureUtils.UserCulture;

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            string location_query = string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}", coord.Latitude, coord.Longitude);

            Result result = null;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                Uri queryURL = new Uri(String.Format(GEOLOCATION_QUERY_URL, location_query, locale));

                using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                {
                    // Add headers to request
                    var token = await HEREOAuthUtils.GetBearerToken();
                    if (!String.IsNullOrWhiteSpace(token))
                        request.Headers.Add("Authorization", token);
                    else
                        throw new WeatherException(WeatherUtils.ErrorStatus.NetworkError);

                    request.Headers.CacheControl.MaxAge = TimeSpan.FromDays(1);

                    // Connect to webstream
                    var webClient = SimpleLibrary.GetInstance().WebClient;
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                    {
                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                        // Load data
                        Geo_Rootobject root = JSONParser.Deserializer<Geo_Rootobject>(contentStream);

                        if (root.response.view.Length > 0 && root.response.view[0].result.Length > 0)
                            result = root.response.view[0].result[0];
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

                Logger.WriteLine(LoggerLevel.Error, ex, "HEREWeatherProvider: error getting location");
            }

            if (wEx != null)
                throw wEx;

            if (result != null && !String.IsNullOrWhiteSpace(result.location.locationId))
                location = new LocationQueryViewModel(result, weatherAPI);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<LocationQueryViewModel> GetLocationFromID(LocationQueryViewModel model)
        {
            LocationQueryViewModel location = null;

            var culture = CultureUtils.UserCulture;

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            Result result = null;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                Uri queryURL = new Uri(String.Format(GEOCODER_QUERY_API, model.LocationQuery, locale));

                using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                {
                    // Add headers to request
                    var token = await HEREOAuthUtils.GetBearerToken();
                    if (!String.IsNullOrWhiteSpace(token))
                        request.Headers.Add("Authorization", token);
                    else
                        throw new WeatherException(WeatherUtils.ErrorStatus.NetworkError);

                    request.Headers.CacheControl.MaxAge = TimeSpan.FromDays(1);

                    // Connect to webstream
                    var webClient = SimpleLibrary.GetInstance().WebClient;
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                    {
                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                        // Load data
                        Geo_Rootobject root = JSONParser.Deserializer<Geo_Rootobject>(contentStream);

                        if (root.response.view.Length > 0 && root.response.view[0].result.Length > 0)
                            result = root.response.view[0].result[0];
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

                Logger.WriteLine(LoggerLevel.Error, ex, "HEREWeatherProvider: error getting location");
            }

            if (wEx != null)
                throw wEx;

            if (result != null && !String.IsNullOrWhiteSpace(result.location.locationId))
                location = new LocationQueryViewModel(result, model.WeatherSource);
            else
                location = new LocationQueryViewModel();

            return location;
        }

        public override Task<LocationQueryViewModel> GetLocationFromName(LocationQueryViewModel model)
        {
            return Task.FromResult<LocationQueryViewModel>(null);
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override Task<bool> IsKeyValid(string key)
        {
            return Task.FromResult(false);
        }

        public override String GetAPIKey()
        {
            return null;
        }
    }
}
#endif