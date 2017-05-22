using System;
using System.Threading.Tasks;
using System.IO;
using SimpleWeather.Utils;
using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;
#if WINDOWS_UWP
using Windows.Storage.Streams;
using Windows.Web.Http;
#elif __ANDROID__
using Android.Widget;
using System.Net.Http;
using SimpleWeather.Droid;
#endif

namespace SimpleWeather.WeatherData
{
    public class WeatherDataLoader
    {
        private WeatherLoadedListener callback;
        private string location_query = null;
        private Weather weather = null;
        private int locationIdx = 0;

        public WeatherDataLoader(WeatherLoadedListener listener, string query, int idx)
        {
            callback = listener;
            location_query = query;
            locationIdx = idx;
        }

        public void setWeatherLoadedListener(WeatherLoadedListener listener)
        {
            callback = listener;
        }

        private async Task getWeatherData()
        {
            string queryAPI = null;
            Uri weatherURL = null;

            if (Settings.API == "WUnderground")
            {
                queryAPI = "http://api.wunderground.com/api/" + Settings.API_KEY + "/astronomy/conditions/forecast10day";
                string options = ".json";
                weatherURL = new Uri(queryAPI + location_query + options);
            }
            else
            {
                queryAPI = "https://query.yahooapis.com/v1/public/yql?q=";
                string query = "select * from weather.forecast where woeid=\""
                    + location_query + "\" and u='F'&format=json";
                weatherURL = new Uri(queryAPI + query);
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
                    if (Settings.API == "WUnderground")
                    {
                        WeatherUnderground.Rootobject root = null;
                        await Task.Run(() =>
                        {
                            root = (WeatherUnderground.Rootobject)JsonConvert.DeserializeObject(content, typeof(WeatherUnderground.Rootobject));
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
                            root = (WeatherYahoo.Rootobject)JsonConvert.DeserializeObject(content, typeof(WeatherYahoo.Rootobject));
                        });
                    }
                }
                catch (Exception ex)
                {
                    weather = null;
#if WINDOWS_UWP
                    if (Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
#elif __ANDROID__
                    if (ex is System.Net.WebException)
#endif
                    {
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NETWORKERROR);
                        break;
                    }
                }

                // If we can't load data, delay and try again
                if (weather == null)
                    await Task.Delay(1000);

                counter++;
            } while (weather == null && counter < 10);

            // Load old data if available and we can't get new data
            if (weather == null)
            {
                await loadSavedWeatherData(true);
            }
            else if (weather != null)
            {
                saveWeatherData();
            }

            // End Stream
            webClient.Dispose();

            if (weather == null && wEx != null)
            {
                throw wEx;
            }
            else if (weather == null && wEx == null)
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.NOWEATHER);
            }
        }

        public async Task loadWeatherData(bool forceRefresh)
        {
            if (forceRefresh)
            {
                try
                {
                    await getWeatherData();
                }
                catch (WeatherException wEx)
                {
#if WINDOWS_UWP
                    await Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(
                        Windows.UI.Core.CoreDispatcherPriority.Normal,
                        async () => await new Windows.UI.Popups.MessageDialog(wEx.Message).ShowAsync());
#elif __ANDROID__
                    Toast.MakeText(App.Context, wEx.Message, ToastLength.Short).Show();
#endif
                }
            }
            else
                await loadWeatherData();

            callback.onWeatherLoaded(locationIdx, weather);
        }

        private async Task loadWeatherData()
        {
            /*
             * If unable to retrieve saved data, data is old, or units don't match
             * Refresh weather data
            */

            bool gotData = await loadSavedWeatherData();

            if (!gotData)
            {
                try
                {
                    await getWeatherData();
                }
                catch (WeatherException wEx)
                {
#if WINDOWS_UWP
                    await Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(
                        Windows.UI.Core.CoreDispatcherPriority.Normal,
                        async () => await new Windows.UI.Popups.MessageDialog(wEx.Message).ShowAsync());
#elif __ANDROID__
                    Toast.MakeText(App.Context, wEx.Message, ToastLength.Short).Show();
#endif
                }
            }
        }

        private async Task<bool> loadSavedWeatherData(bool _override)
        {
            if (_override)
            {
                // Load weather data
                try
                {
                    weather = (await Settings.getWeatherData())[locationIdx] as Weather;
                }
                catch (Exception)
                {
                    weather = null;
                }

                if (weather == null)
                    return false;

                return true;
            }
            else
                return await loadSavedWeatherData();
        }

        private async Task<bool> loadSavedWeatherData()
        {
            // Load weather data
            try
            {
                weather = (await Settings.getWeatherData())[locationIdx] as Weather;
            }
            catch (Exception)
            {
                weather = null;
            }

            if (weather == null)
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

        private async void saveWeatherData()
        {
            OrderedDictionary weatherData = await Settings.getWeatherData();
            if (locationIdx > weatherData.Count - 1)
                weatherData.Insert(locationIdx, location_query, weather);
            else
                weatherData[locationIdx] = weather;
            Settings.saveWeatherData(weatherData);
        }

        public Weather getWeather()
        {
            return weather;
        }
    }

    public static class WeatherLoaderTask
    {
        private static Weather weather = null;

        public static async Task<Weather> getWeather(string location_query)
        {
            string queryAPI = null;
            Uri weatherURL = null;

            if (Settings.API == "WUnderground")
            {
                queryAPI = "http://api.wunderground.com/api/" + Settings.API_KEY + "/astronomy/conditions/forecast10day";
                string options = ".json";
                weatherURL = new Uri(queryAPI + location_query + options);
            }
            else
            {
                queryAPI = "https://query.yahooapis.com/v1/public/yql?q=";
                string query = "select * from weather.forecast where woeid=\""
                    + location_query + "\" and u='F'&format=json";
                weatherURL = new Uri(queryAPI + query);
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
                    if (Settings.API == "WUnderground")
                    {
                        WeatherUnderground.Rootobject root = null;
                        await Task.Run(() => 
                        {
                            root = (WeatherUnderground.Rootobject)JsonConvert.DeserializeObject(content, typeof(WeatherUnderground.Rootobject));
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
                            root = (WeatherYahoo.Rootobject)JsonConvert.DeserializeObject(content, typeof(WeatherYahoo.Rootobject));
                        });
                        weather = new Weather(root);
                    }
                }
                catch (Exception ex)
                {
                    weather = null;
#if WINDOWS_UWP
                    if (Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
#elif __ANDROID__
                    if (ex is System.Net.WebException)
#endif
                    {
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NETWORKERROR);
                        break;
                    }
                }

                // If we can't load data, delay and try again
                if (weather == null)
                    await Task.Delay(1000);

                counter++;
            } while (weather == null && counter < 10);

            // End Stream
            webClient.Dispose();

            if (weather == null)
            {
                if (wEx == null)
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NOWEATHER);

#if WINDOWS_UWP
                await Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                    async () => await new Windows.UI.Popups.MessageDialog(wEx.Message).ShowAsync());
#elif __ANDROID__
                Toast.MakeText(App.Context, wEx.Message, ToastLength.Short).Show();
#endif
            }

            return weather;
        }
    }
}
