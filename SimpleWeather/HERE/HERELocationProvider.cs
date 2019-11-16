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
            string queryAPI = "https://autocomplete.geocoder.cit.api.here.com/6.2/suggest.json";
#else
            string queryAPI = "https://autocomplete.geocoder.api.here.com/6.2/suggest.json";
#endif
            string query = "?query={0}&app_id={1}&app_code={2}&language={3}&maxresults=10";

            string app_id = GetAppID();
            string app_code = GetAppCode();

            Uri queryURL = new Uri(String.Format(queryAPI + query, location_query, app_id, app_code, locale));
            WeatherException wEx = null;
            // Limit amount of results shown
            int maxResults = 10;

            using (HttpClient webClient = new HttpClient())
            using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
            {
                try
                {
                    // Connect to webstream
                    HttpResponseMessage response = await webClient.GetAsync(queryURL).AsTask(cts.Token);
                    response.EnsureSuccessStatusCode();
                    Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
                    // End Stream
                    webClient.Dispose();

                    // Load data
                    var locationSet = new HashSet<LocationQueryViewModel>();
                    AC_Rootobject root = null;
                    await Task.Run(() =>
                    {
                        root = JSONParser.Deserializer<AC_Rootobject>(contentStream);
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
            string queryAPI = "https://reverse.geocoder.cit.api.here.com/6.2/reversegeocode.json";
#else
            string queryAPI = "https://reverse.geocoder.api.here.com/6.2/reversegeocode.json";
#endif
            string location_query = string.Format("{0},{1}", coord.Latitude.ToString(CultureInfo.InvariantCulture), coord.Longitude.ToString(CultureInfo.InvariantCulture));
            string query = "?prox={0},150&mode=retrieveAddresses&maxresults=1&additionaldata=Country2,true&gen=9&jsonattributes=1" +
                "&locationattributes=adminInfo,timeZone,-mapView,-mapReference&language={1}&app_id={2}&app_code={3}";

            string app_id = GetAppID();
            string app_code = GetAppCode();

            Uri queryURL = new Uri(String.Format(queryAPI + query, location_query, locale, app_id, app_code));
            Result result = null;
            WeatherException wEx = null;

            using (HttpClient webClient = new HttpClient())
            using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
            {
                try
                {
                    // Connect to webstream
                    HttpResponseMessage response = await webClient.GetAsync(queryURL).AsTask(cts.Token);
                    response.EnsureSuccessStatusCode();
                    Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                    // End Stream
                    webClient.Dispose();

                    // Load data
                    Geo_Rootobject root = null;
                    await Task.Run(() =>
                    {
                        root = JSONParser.Deserializer<Geo_Rootobject>(contentStream);
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
            string queryAPI = "https://geocoder.cit.api.here.com/6.2/geocode.json";
#else
            string queryAPI = "https://geocoder.api.here.com/6.2/geocode.json";
#endif
            string query = "?locationid={0}&mode=retrieveAddresses&maxresults=1&additionaldata=Country2,true&gen=9&jsonattributes=1" +
                "&locationattributes=adminInfo,timeZone,-mapView,-mapReference&language={1}&app_id={2}&app_code={3}";

            string app_id = GetAppID();
            string app_code = GetAppCode();

            Uri queryURL = new Uri(String.Format(queryAPI + query, locationID, locale, app_id, app_code));
            Result result = null;
            WeatherException wEx = null;

            using (HttpClient webClient = new HttpClient())
            using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
            {
                try
                {
                    // Connect to webstream
                    HttpResponseMessage response = await webClient.GetAsync(queryURL).AsTask(cts.Token);
                    response.EnsureSuccessStatusCode();
                    Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                    // End Stream
                    webClient.Dispose();

                    // Load data
                    Geo_Rootobject root = null;
                    await Task.Run(() =>
                    {
                        root = JSONParser.Deserializer<Geo_Rootobject>(contentStream);
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
        public override async Task<bool> IsKeyValid(string key)
        {
            string queryAPI = "https://weather.cit.api.here.com/weather/1.0/report.json";

            string app_id = "";
            string app_code = "";

            if (!String.IsNullOrWhiteSpace(key))
            {
                string[] keyArr = key.Split(';');
                if (keyArr.Length > 0)
                {
                    app_id = keyArr[0];
                    app_code = keyArr[keyArr.Length > 1 ? keyArr.Length - 1 : 0];
                }
            }

            Uri queryURL = new Uri(String.Format("{0}?app_id={1}&app_code={2}", queryAPI, app_id, app_code));
            bool isValid = false;
            WeatherException wEx = null;

            try
            {
                if (String.IsNullOrWhiteSpace(app_id) || String.IsNullOrWhiteSpace(app_code))
                    throw (wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey));

                CancellationTokenSource cts = new CancellationTokenSource(Settings.READ_TIMEOUT);

                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(queryURL).AsTask(cts.Token);

                // Check for errors
                switch (response.StatusCode)
                {
                    // 400 (OK since this isn't a valid request)
                    case HttpStatusCode.BadRequest:
                        isValid = true;
                        break;
                    // 401 (Unauthorized - Key is invalid)
                    case HttpStatusCode.Unauthorized:
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                        isValid = false;
                        break;
                }

                // End Stream
                response.Dispose();
                webClient.Dispose();
                cts.Dispose();
            }
            catch (Exception ex)
            {
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                }

                isValid = false;
            }

            if (wEx != null)
            {
                throw wEx;
            }

            return isValid;
        }

        private String GetAppID()
        {
            return APIKeys.GetHEREAppID();
        }

        private String GetAppCode()
        {
            return APIKeys.GetHEREAppCode();
        }

        public override String GetAPIKey()
        {
            if (String.IsNullOrWhiteSpace(GetAppID()) && String.IsNullOrWhiteSpace(GetAppCode()))
                return String.Empty;
            else
                return String.Format("{0};{1}", GetAppID(), GetAppCode());
        }
    }
}