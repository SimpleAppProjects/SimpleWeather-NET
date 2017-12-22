using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using SimpleWeather.Utils;
using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;
#if WINDOWS_UWP
using SimpleWeather.UWP.Controls;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.Web;
using Windows.Web.Http;
#elif __ANDROID__
using Android.Widget;
using System.Net;
using System.Net.Http;
using SimpleWeather.Droid;
#endif

namespace SimpleWeather.WeatherData
{
    public class WeatherDataLoader
    {
        private IWeatherLoadedListener callback;
        private IWeatherErrorListener errorCallback;

        private LocationData location = null;
        private Weather weather = null;

        public WeatherDataLoader(IWeatherLoadedListener listener, LocationData location)
        {
            callback = listener;
            this.location = location;
        }

        public WeatherDataLoader(IWeatherLoadedListener listener, IWeatherErrorListener errorListener, LocationData location)
        {
            callback = listener;
            errorCallback = errorListener;
            this.location = location;
        }

        public void SetWeatherLoadedListener(IWeatherLoadedListener listener)
        {
            callback = listener;
        }

        public void SetWeatherErrorListener(IWeatherErrorListener listener)
        {
            errorCallback = listener;
        }

        private async Task GetWeatherData()
        {
            string queryAPI = null;
            Uri weatherURL = null;
            string locale = "EN";

#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages.First();
            var culture = new System.Globalization.CultureInfo(userlang);
#else
            var culture = System.Globalization.CultureInfo.CurrentCulture;
#endif
            locale = WeatherUtils.LocaleToWUCode(culture.TwoLetterISOLanguageName, culture.Name);

            if (Settings.API == Settings.API_Yahoo)
                locale = "EN";

            if (Settings.API == Settings.API_WUnderground)
            {
                queryAPI = "http://api.wunderground.com/api/" + Settings.API_KEY + "/astronomy/conditions/forecast10day/hourly/lang:" + locale;
                string options = ".json";
                weatherURL = new Uri(queryAPI + location.query + options);
            }
            else if (Settings.API == Settings.API_Yahoo && int.TryParse(location.query, out int woeid))
            {
                queryAPI = "https://query.yahooapis.com/v1/public/yql?q=";
                string query = "select * from weather.forecast where woeid=\""
                    + woeid + "\" and u='F'&format=json";
                weatherURL = new Uri(queryAPI + query);
            }
            else if (Settings.API == Settings.API_Yahoo)
            {
                queryAPI = "https://query.yahooapis.com/v1/public/yql?q=";
                string query = "select * from weather.forecast where woeid in (select woeid from geo.places(1) where text=\""
                    + location.query + "\") and u='F'&format=json";
                weatherURL = new Uri(queryAPI + query);
            }

            HttpClient webClient = new HttpClient();
            WeatherException wEx = null;
            bool loadedSavedData = false;

            try
            {
                // Get response
                HttpResponseMessage response = await webClient.GetAsync(weatherURL);
                response.EnsureSuccessStatusCode();
                Stream contentStream = null;
#if WINDOWS_UWP
                contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
#elif __ANDROID__
                contentStream = await response.Content.ReadAsStreamAsync();
#endif
                // Reset exception
                wEx = null;

                // Load weather
                if (Settings.API == Settings.API_WUnderground)
                {
                    WeatherUnderground.Rootobject root = null;
                    await Task.Run(() =>
                    {
                        root = JSONParser.Deserializer<WeatherUnderground.Rootobject>(contentStream);
                    });

                    // Check for errors
                    if (root.response.error != null)
                    {
                        switch (root.response.error.type)
                        {
                            case "querynotfound":
                                wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
                                break;
                            case "keynotfound":
                                wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                                break;
                            default:
                                break;
                        }
                    }

                    weather = new Weather(root);
                }
                else
                {
                    WeatherYahoo.Rootobject root = null;
                    await Task.Run(() =>
                    {
                        root = JSONParser.Deserializer<WeatherYahoo.Rootobject>(contentStream);
                    });

                    weather = new Weather(root);
                }
            }
            catch (Exception ex)
            {
                weather = null;
#if WINDOWS_UWP
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
#elif __ANDROID__
                if (ex is WebException || ex is HttpRequestException)
#endif
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                }

                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            // Load old data if available and we can't get new data
            if (weather == null)
            {
                loadedSavedData = await LoadSavedWeatherData(true);
            }
            else if (weather != null)
            {
                weather.locale = locale;
                SaveWeatherData();
            }

            // End Stream
            webClient.Dispose();

            // Throw exception if we're unable to get any weather data
            if (weather == null && wEx != null)
            {
                throw wEx;
            }
            else if (weather == null && wEx == null)
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
            }
            else if (weather != null && wEx != null && loadedSavedData)
            {
                throw wEx;
            }
        }

        public async Task LoadWeatherData(bool forceRefresh)
        {
            if (forceRefresh)
            {
                try
                {
                    await GetWeatherData();
                }
                catch (WeatherException wEx)
                {
                    errorCallback?.OnWeatherError(wEx);
                }
            }
            else
                await LoadWeatherData();

            callback?.OnWeatherLoaded(location, weather);
        }

        private async Task LoadWeatherData()
        {
            /*
             * If unable to retrieve saved data, data is old, or units don't match
             * Refresh weather data
            */

            bool gotData = await LoadSavedWeatherData();

            if (!gotData)
            {
                try
                {
                    if (weather != null && weather.source != Settings.API)
                        await UpdateQuery();

                    await GetWeatherData();
                }
                catch (WeatherException wEx)
                {
                    errorCallback?.OnWeatherError(wEx);
                }
            }
        }

        private async Task UpdateQuery()
        {
            string coord = string.Format("{0},{1}", weather.location.latitude, weather.location.longitude);
            var qview = await GeopositionQuery.GetLocation(new WeatherUtils.Coordinate(coord));

            if (String.IsNullOrEmpty(qview.LocationQuery))
            {
                if (Settings.API == Settings.API_WUnderground)
                    location.query = string.Format("/q/{0}", coord);
                else if (Settings.API == Settings.API_Yahoo)
                    location.query = string.Format("({0})", coord);
            }
            else
                location.query = qview.LocationQuery;
        }

        private async Task<bool> LoadSavedWeatherData(bool _override)
        {
            if (_override)
            {
                // Load weather data
                try
                {
                    weather = await Settings.GetWeatherData(location.query);
                }
                catch (Exception ex)
                {
                    weather = null;
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }

#if WINDOWS_UWP
                var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages.First();
                var culture = new System.Globalization.CultureInfo(userlang);
#else
                var culture = System.Globalization.CultureInfo.CurrentCulture;
#endif
                var locale = WeatherUtils.LocaleToWUCode(culture.TwoLetterISOLanguageName, culture.Name);

                if (weather == null || !weather.IsValid() || weather.source != Settings.API || weather.locale != locale)
                    return false;

                return true;
            }
            else
                return await LoadSavedWeatherData();
        }

        private async Task<bool> LoadSavedWeatherData()
        {
            // Load weather data
            try
            {
                weather = await Settings.GetWeatherData(location.query);
            }
            catch (Exception ex)
            {
                weather = null;
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages.First();
            var culture = new System.Globalization.CultureInfo(userlang);
#else
            var culture = System.Globalization.CultureInfo.CurrentCulture;
#endif
            var locale = WeatherUtils.LocaleToWUCode(culture.TwoLetterISOLanguageName, culture.Name);

            if (weather == null || !weather.IsValid() || weather.source != Settings.API || weather.locale != locale)
                return false;

            // Weather data expiration
            if (!int.TryParse(weather.ttl, out int ttl))
                ttl = 60;

            // Check file age
            DateTimeOffset updateTime = weather.update_time;

            TimeSpan span = DateTimeOffset.Now - updateTime;
            if (span.TotalMinutes < ttl)
                return true;
            else
                return false;
        }

        private void SaveWeatherData()
        {
            // Save location query
            weather.query = location.query;

            Settings.SaveWeatherData(weather);
        }

        public Weather GetWeather()
        {
            return weather;
        }
    }

    public static class WeatherLoaderTask
    {
        private static Weather weather = null;

        public static async Task<Weather> GetWeather(string location_query)
        {
            string queryAPI = null;
            Uri weatherURL = null;
            string locale = "EN";

#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages.First();
            var culture = new System.Globalization.CultureInfo(userlang);
#else
            var culture = System.Globalization.CultureInfo.CurrentCulture;
#endif
            locale = WeatherUtils.LocaleToWUCode(culture.TwoLetterISOLanguageName, culture.Name);

            if (String.IsNullOrWhiteSpace(locale))
                locale = "EN";

            if (Settings.API == Settings.API_WUnderground)
            {
                queryAPI = "http://api.wunderground.com/api/" + Settings.API_KEY + "/astronomy/conditions/forecast10day/hourly/lang:" + locale;
                string options = ".json";
                weatherURL = new Uri(queryAPI + location_query + options);
            }
            else if (Settings.API == Settings.API_Yahoo && int.TryParse(location_query, out int woeid))
            {
                queryAPI = "https://query.yahooapis.com/v1/public/yql?q=";
                string query = "select * from weather.forecast where woeid=\""
                    + woeid + "\" and u='F'&format=json";
                weatherURL = new Uri(queryAPI + query);
            }
            else if (Settings.API == Settings.API_Yahoo)
            {
                queryAPI = "https://query.yahooapis.com/v1/public/yql?q=";
                string query = "select * from weather.forecast where woeid in (select woeid from geo.places(1) where text=\""
                    + location_query + "\") and u='F'&format=json";
                weatherURL = new Uri(queryAPI + query);
            }

            HttpClient webClient = new HttpClient();
            WeatherException wEx = null;

            try
            {
                // Get response
                HttpResponseMessage response = await webClient.GetAsync(weatherURL);
                response.EnsureSuccessStatusCode();
                Stream contentStream = null;
#if WINDOWS_UWP
                contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
#elif __ANDROID__
                contentStream = await response.Content.ReadAsStreamAsync();
#endif
                // Reset exception
                wEx = null;

                // Load weather
                if (Settings.API == Settings.API_WUnderground)
                {
                    WeatherUnderground.Rootobject root = null;
                    await Task.Run(() =>
                    {
                        root = JSONParser.Deserializer<WeatherUnderground.Rootobject>(contentStream);
                    });

                    // Check for errors
                    if (root.response.error != null)
                    {
                        switch (root.response.error.type)
                        {
                            case "querynotfound":
                                wEx = new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
                                break;
                            case "keynotfound":
                                wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                                break;
                            default:
                                break;
                        }
                    }

                    weather = new Weather(root);
                }
                else
                {
                    WeatherYahoo.Rootobject root = null;
                    await Task.Run(() =>
                    {
                        root = JSONParser.Deserializer<WeatherYahoo.Rootobject>(contentStream);
                    });
                    weather = new Weather(root);
                }
            }
            catch (Exception ex)
            {
                weather = null;
#if WINDOWS_UWP
                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
#elif __ANDROID__
                if (ex is WebException || ex is HttpRequestException)
#endif
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                }

                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            // End Stream
            webClient.Dispose();

            if (weather == null)
            {
                if (wEx == null)
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather);

#if WINDOWS_UWP
                await Toast.ShowToastAsync(wEx.Message, ToastDuration.Short);
#elif __ANDROID__
                Toast.MakeText(App.Context, wEx.Message, ToastLength.Short).Show();
#endif
            }
            else
            {
                weather.locale = locale;
                weather.query = location_query;
            }

            return weather;
        }
    }
}
