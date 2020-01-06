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

namespace SimpleWeather.HERE
{
    public class HERELocationProvider : LocationProviderImpl
    {
        public override string LocationAPI => WeatherAPI.Here;

        public override bool KeyRequired => false;

        public override bool SupportsWeatherLocale => true;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string location_query, string weatherAPI)
        {
            ObservableCollection<LocationQueryViewModel> locations = null;

            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

#if DEBUG
            string queryAPI = "https://autocomplete.geocoder.ls.hereapi.com/6.2/suggest.json";
#else
            string queryAPI = "https://autocomplete.geocoder.ls.hereapi.com/6.2/suggest.json";
#endif
            string query = "?query={0}&language={1}&maxresults=10";

            OAuthRequest authRequest = new OAuthRequest(APIKeys.GetHERECliID(), APIKeys.GetHERECliSecr());

            Uri queryURL = new Uri(String.Format(queryAPI + query, location_query, locale));
            WeatherException wEx = null;
            // Limit amount of results shown
            int maxResults = 10;

            using (HttpClient webClient = new HttpClient())
            using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
            using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
            {
                try
                {
                    // Add headers to request
                    request.Headers.Add("Authorization", await AsyncTask.RunAsync(HEREOAuthUtils.GetBearerToken()));

                    // Connect to webstream
                    HttpResponseMessage response = await AsyncTask.RunAsync(webClient.SendRequestAsync(request).AsTask(cts.Token));
                    response.EnsureSuccessStatusCode();
                    Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
                    // End Stream
                    webClient.Dispose();

                    // Load data
                    var locationSet = new HashSet<LocationQueryViewModel>();
                    AC_Rootobject root = await AsyncTask.RunAsync(() =>
                    {
                        return JSONParser.Deserializer<AC_Rootobject>(contentStream);
                    });

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

                    // End Stream
                    contentStream?.Dispose();
                }
                catch (Exception ex)
                {
                    if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                    {
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    }
                    Logger.WriteLine(LoggerLevel.Error, ex, "HEREWeatherProvider: error getting locations");
                }

                if (wEx != null)
                    throw wEx;

                if (locations == null || locations.Count == 0)
                    locations = new ObservableCollection<LocationQueryViewModel>() { new LocationQueryViewModel() };

                return locations;
            }
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coord, string weatherAPI)
        {
            LocationQueryViewModel location = null;

            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

#if DEBUG
            string queryAPI = "https://reverse.geocoder.ls.hereapi.com/6.2/reversegeocode.json";
#else
            string queryAPI = "https://reverse.geocoder.ls.hereapi.com/6.2/reversegeocode.json";
#endif
            string location_query = string.Format("{0},{1}", coord.Latitude.ToString(CultureInfo.InvariantCulture), coord.Longitude.ToString(CultureInfo.InvariantCulture));
            string query = "?prox={0},150&mode=retrieveAddresses&maxresults=1&additionaldata=Country2,true&gen=9&jsonattributes=1" +
                "&locationattributes=adminInfo,timeZone,-mapView,-mapReference&language={1}";

            Uri queryURL = new Uri(String.Format(queryAPI + query, location_query, locale));
            Result result = null;
            WeatherException wEx = null;

            using (HttpClient webClient = new HttpClient())
            using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
            using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
            {
                try
                {
                    // Add headers to request
                    request.Headers.Add("Authorization", await AsyncTask.RunAsync(HEREOAuthUtils.GetBearerToken()));

                    // Connect to webstream
                    HttpResponseMessage response = await AsyncTask.RunAsync(webClient.SendRequestAsync(request).AsTask(cts.Token));
                    response.EnsureSuccessStatusCode();
                    Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                    // End Stream
                    webClient.Dispose();

                    // Load data
                    Geo_Rootobject root = await AsyncTask.RunAsync(() =>
                    {
                        return JSONParser.Deserializer<Geo_Rootobject>(contentStream);
                    });

                    if (root.response.view.Length > 0 && root.response.view[0].result.Length > 0)
                        result = root.response.view[0].result[0];

                    // End Stream
                    contentStream?.Dispose();
                }
                catch (Exception ex)
                {
                    result = null;

                    if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                    {
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
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
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public async Task<LocationQueryViewModel> GetLocationFromLocID(string locationID, string weatherAPI)
        {
            LocationQueryViewModel location = null;

            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

#if DEBUG
            string queryAPI = "https://geocoder.ls.hereapi.com/6.2/geocode.json";
#else
            string queryAPI = "https://geocoder.ls.hereapi.com/6.2/geocode.json";
#endif
            string query = "?locationid={0}&mode=retrieveAddresses&maxresults=1&additionaldata=Country2,true&gen=9&jsonattributes=1" +
                "&locationattributes=adminInfo,timeZone,-mapView,-mapReference&language={1}";

            Uri queryURL = new Uri(String.Format(queryAPI + query, locationID, locale));
            Result result = null;
            WeatherException wEx = null;

            using (HttpClient webClient = new HttpClient())
            using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
            using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
            {
                try
                {
                    // Add headers to request
                    request.Headers.Add("Authorization", await AsyncTask.RunAsync(HEREOAuthUtils.GetBearerToken()));

                    // Connect to webstream
                    HttpResponseMessage response = await AsyncTask.RunAsync(webClient.SendRequestAsync(request).AsTask(cts.Token));
                    response.EnsureSuccessStatusCode();
                    Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                    // End Stream
                    webClient.Dispose();

                    // Load data
                    Geo_Rootobject root = await AsyncTask.RunAsync(() =>
                    {
                        return JSONParser.Deserializer<Geo_Rootobject>(contentStream);
                    });

                    if (root.response.view.Length > 0 && root.response.view[0].result.Length > 0)
                        result = root.response.view[0].result[0];

                    // End Stream
                    contentStream?.Dispose();
                }
                catch (Exception ex)
                {
                    result = null;

                    if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                    {
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    }

                    Logger.WriteLine(LoggerLevel.Error, ex, "HEREWeatherProvider: error getting location");
                }
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
        public override Task<bool> IsKeyValid(string key)
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(false);
            return tcs.Task;
        }

        public override String GetAPIKey()
        {
            return null;
        }
    }
}