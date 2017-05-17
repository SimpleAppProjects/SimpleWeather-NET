using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using Windows.Storage.Streams;
using Windows.Web.Http;
using SimpleWeather.Utils;
using System.Collections.Specialized;

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
            MemoryStream memStream = new MemoryStream();
            DataContractJsonSerializer deserializer = null;
            int counter = 0;
            WeatherException wEx = null;

            do
            {
                try
                {
                    // Get response
                    HttpResponseMessage response = await webClient.GetAsync(weatherURL);
                    response.EnsureSuccessStatusCode();
                    IBuffer buff = await response.Content.ReadAsBufferAsync();
                    // Reset exception
                    wEx = null;

                    // Write array/buffer to memorystream
                    memStream.SetLength(0);
                    await memStream.AsOutputStream().WriteAsync(buff);
                    memStream.Seek(0, 0);

                    // Load weather
                    if (Settings.API == "WUnderground")
                    {
                        deserializer = new DataContractJsonSerializer(typeof(WeatherUnderground.Rootobject));
                        WeatherUnderground.Rootobject root = (WeatherUnderground.Rootobject)deserializer.ReadObject(memStream);

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
                        deserializer = new DataContractJsonSerializer(typeof(WeatherYahoo.Rootobject));
                        WeatherYahoo.Rootobject root = (WeatherYahoo.Rootobject)deserializer.ReadObject(memStream);
                        weather = new Weather(root);
                    }
                }
                catch (Exception ex)
                {
                    weather = null;
                    if (Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
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

            await memStream.FlushAsync();
            memStream.Dispose();

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
                    await Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(
                        Windows.UI.Core.CoreDispatcherPriority.Normal,
                        async () => await new Windows.UI.Popups.MessageDialog(wEx.Message).ShowAsync());
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
                    await Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(
                        Windows.UI.Core.CoreDispatcherPriority.Normal,
                        async () => await new Windows.UI.Popups.MessageDialog(wEx.Message).ShowAsync());
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
            MemoryStream memStream = new MemoryStream();
            DataContractJsonSerializer deserializer = null;
            int counter = 0;
            WeatherException wEx = null;

            do
            {
                try
                {
                    // Get response
                    HttpResponseMessage response = await webClient.GetAsync(weatherURL);
                    response.EnsureSuccessStatusCode();
                    IBuffer buff = await response.Content.ReadAsBufferAsync();
                    // Reset exception
                    wEx = null;

                    // Write array/buffer to memorystream
                    memStream.SetLength(0);
                    await memStream.AsOutputStream().WriteAsync(buff);
                    memStream.Seek(0, 0);

                    // Load weather
                    if (Settings.API == "WUnderground")
                    {
                        deserializer = new DataContractJsonSerializer(typeof(WeatherUnderground.Rootobject));
                        WeatherUnderground.Rootobject root = (WeatherUnderground.Rootobject)deserializer.ReadObject(memStream);

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
                        deserializer = new DataContractJsonSerializer(typeof(WeatherYahoo.Rootobject));
                        WeatherYahoo.Rootobject root = (WeatherYahoo.Rootobject)deserializer.ReadObject(memStream);
                        weather = new Weather(root);
                    }
                }
                catch (Exception ex)
                {
                    weather = null;
                    if (Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
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

            await memStream.FlushAsync();
            memStream.Dispose();

            if (weather == null)
            {
                if (wEx == null)
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NOWEATHER);

                await Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                    async () => await new Windows.UI.Popups.MessageDialog(wEx.Message).ShowAsync());
            }

            return weather;
        }
    }
}
