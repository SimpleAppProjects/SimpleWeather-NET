using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using SimpleWeather.Utils;
using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;
#if WINDOWS_UWP
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
        private string location_query = null;
        private Weather weather = null;
        private int locationIdx = 0;

        public WeatherDataLoader(IWeatherLoadedListener listener, string query, int idx)
        {
            callback = listener;
            location_query = query;
            locationIdx = idx;
        }

        public void SetWeatherLoadedListener(IWeatherLoadedListener listener)
        {
            callback = listener;
        }

        private async Task GetWeatherData()
        {
            string queryAPI = null;
            Uri weatherURL = null;
            int maxRetries = 0;

            if (Settings.API == Settings.API_WUnderground)
            {
                queryAPI = "http://api.wunderground.com/api/" + Settings.API_KEY + "/astronomy/conditions/forecast10day/hourly";
                string options = ".json";
                weatherURL = new Uri(queryAPI + location_query + options);

                // Don't force retries since it counts against calls per minute
                maxRetries = 0;
            }
            else
            {
                queryAPI = "https://query.yahooapis.com/v1/public/yql?q=";
                string query = "select * from weather.forecast where woeid=\""
                    + location_query + "\" and u='F'&format=json";
                weatherURL = new Uri(queryAPI + query);

                // Allow more retries, since its known to fail sometimes
                maxRetries = 2;
            }

            HttpClient webClient = new HttpClient();
            int counter = 0;
            WeatherException wEx = null;

            do
            {
                try
                {
                    // Get response
                    HttpResponseMessage response = await webClient.GetAsync(weatherURL);
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync();
                    // Reset exception
                    wEx = null;

                    // Load weather
                    if (Settings.API == Settings.API_WUnderground)
                    {
                        WeatherUnderground.Rootobject root = null;
                        await Task.Run(() =>
                        {
                            root = JSONParser.Deserializer<WeatherUnderground.Rootobject>(content);
                        });

                        // Check for errors
                        if (root.response.error != null)
                        {
                            switch (root.response.error.type)
                            {
                                case "querynotfound":
                                    wEx = new WeatherException(WeatherUtils.ErrorStatus.QUERYNOTFOUND);
                                    break;
                                case "keynotfound":
                                    wEx = new WeatherException(WeatherUtils.ErrorStatus.INVALIDAPIKEY);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }

                        weather = new Weather(root);
                    }
                    else
                    {
                        WeatherYahoo.Rootobject root = null;
                        await Task.Run(() =>
                        {
                            root = JSONParser.Deserializer<WeatherYahoo.Rootobject>(content);
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
                    if (ex is WebException)
#endif
                    {
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NETWORKERROR);
                        break;
                    }

                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }

                // If we can't load data, delay and try again
                if (weather == null)
                    await Task.Delay(1000);

                counter++;
            } while (weather == null && counter < maxRetries);

            // Load old data if available and we can't get new data
            if (weather == null)
            {
                await LoadSavedWeatherData(true);
            }
            else if (weather != null)
            {
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
                throw new WeatherException(WeatherUtils.ErrorStatus.NOWEATHER);
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
#if WINDOWS_UWP
                    await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        async () => await new MessageDialog(wEx.Message).ShowAsync());
#elif __ANDROID__
                    Toast.MakeText(App.Context, wEx.Message, ToastLength.Short).Show();
#endif
                }
            }
            else
                await LoadWeatherData();

            callback.OnWeatherLoaded(locationIdx, weather);
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
                    await GetWeatherData();
                }
                catch (WeatherException wEx)
                {
#if WINDOWS_UWP
                    await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        async () => await new MessageDialog(wEx.Message).ShowAsync());
#elif __ANDROID__
                    Toast.MakeText(App.Context, wEx.Message, ToastLength.Short).Show();
#endif
                }
            }
        }

        private async Task<bool> LoadSavedWeatherData(bool _override)
        {
            if (_override)
            {
                // Load weather data
                try
                {
                    weather = (await Settings.GetWeatherData())[locationIdx] as Weather;
                }
                catch (Exception ex)
                {
                    weather = null;
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }

                if (weather == null || weather.query != location_query)
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
                weather = (await Settings.GetWeatherData())[locationIdx] as Weather;
            }
            catch (Exception ex)
            {
                weather = null;
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            if (weather == null || weather.query != location_query)
                return false;

            // Weather data expiration
            int ttl = int.Parse(weather.ttl);

            // Check file age
            DateTime updateTime = weather.update_time;

            TimeSpan span = DateTime.Now - updateTime;
            if (span.TotalMinutes < ttl)
                return true;
            else
                return false;
        }

        private async void SaveWeatherData()
        {
            // Save location query
            weather.query = location_query;

            OrderedDictionary weatherData = await Settings.GetWeatherData();
            if (locationIdx > weatherData.Count - 1)
                weatherData.Insert(locationIdx, location_query, weather);
            else
            {
                if (weatherData.Keys.Cast<string>().ElementAt(locationIdx) != location_query)
                {
                    // Update key if it differs
                    weatherData.RemoveAt(locationIdx);
                    weatherData.Insert(locationIdx, location_query, weather);
                }
                else
                    weatherData[locationIdx] = weather;
            }
            Settings.SaveWeatherData();
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
            int maxRetries = 0;

            if (Settings.API == Settings.API_WUnderground)
            {
                queryAPI = "http://api.wunderground.com/api/" + Settings.API_KEY + "/astronomy/conditions/forecast10day/hourly";
                string options = ".json";
                weatherURL = new Uri(queryAPI + location_query + options);

                // Don't force retries since it counts against calls per minute
                maxRetries = 0;
            }
            else
            {
                queryAPI = "https://query.yahooapis.com/v1/public/yql?q=";
                string query = "select * from weather.forecast where woeid=\""
                    + location_query + "\" and u='F'&format=json";
                weatherURL = new Uri(queryAPI + query);

                // Allow more retries, since its known to fail sometimes
                maxRetries = 2;
            }

            HttpClient webClient = new HttpClient();
            int counter = 0;
            WeatherException wEx = null;

            do
            {
                try
                {
                    // Get response
                    HttpResponseMessage response = await webClient.GetAsync(weatherURL);
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync();
                    // Reset exception
                    wEx = null;

                    // Load weather
                    if (Settings.API == Settings.API_WUnderground)
                    {
                        WeatherUnderground.Rootobject root = null;
                        await Task.Run(() => 
                        {
                            root = JSONParser.Deserializer<WeatherUnderground.Rootobject>(content);
                        });

                        // Check for errors
                        if (root.response.error != null)
                        {
                            switch (root.response.error.type)
                            {
                                case "querynotfound":
                                    wEx = new WeatherException(WeatherUtils.ErrorStatus.QUERYNOTFOUND);
                                    break;
                                case "keynotfound":
                                    wEx = new WeatherException(WeatherUtils.ErrorStatus.INVALIDAPIKEY);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }

                        weather = new Weather(root);
                    }
                    else
                    {
                        WeatherYahoo.Rootobject root = null;
                        await Task.Run(() =>
                        {
                            root = JSONParser.Deserializer<WeatherYahoo.Rootobject>(content);
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
                    if (ex is WebException)
#endif
                    {
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NETWORKERROR);
                        break;
                    }

                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }

                // If we can't load data, delay and try again
                if (weather == null)
                    await Task.Delay(1000);

                counter++;
            } while (weather == null && counter < maxRetries);

            // End Stream
            webClient.Dispose();

            if (weather == null)
            {
                if (wEx == null)
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NOWEATHER);

#if WINDOWS_UWP
                await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => await new MessageDialog(wEx.Message).ShowAsync());
#elif __ANDROID__
                Toast.MakeText(App.Context, wEx.Message, ToastLength.Short).Show();
#endif
            }
            else
                weather.query = location_query;

            return weather;
        }
    }
}
