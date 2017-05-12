using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using SimpleWeather.Utils;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SimpleWeather.WeatherYahoo
{
    public class WeatherDataLoader
    {
        private WeatherLoadedListener callback;
        private string location = null;
        private Weather weather = null;
        private int locationIdx = 0;

        public WeatherDataLoader(WeatherLoadedListener listener, string Location, int idx)
        {
            callback = listener;
            location = Location;
            locationIdx = idx;
        }

        public void setWeatherLoadedListener(WeatherLoadedListener listener)
        {
            callback = listener;
        }

        private async Task getWeatherData()
        {
            string yahooAPI = "https://query.yahooapis.com/v1/public/yql?q=";
            string query = "select * from weather.forecast where woeid in (select woeid from geo.places(1) where text=\""
                + location + "\") and u='" + Settings.Unit + "'&format=json";
            Uri weatherURL = new Uri(yahooAPI + query);

            HttpClient webClient = new HttpClient();
            MemoryStream memStream = new MemoryStream();
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Rootobject));
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
                    weather = new Weather((Rootobject)deserializer.ReadObject(memStream));
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
                saveTimeZone();
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

        private void saveTimeZone()
        {
            // Now
            DateTime utc = DateTime.ParseExact(weather.created,
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", null);

            // There
            int AMPMidx = weather.lastBuildDate.LastIndexOf(" AM ");
            if (AMPMidx < 0)
                AMPMidx = weather.lastBuildDate.LastIndexOf(" PM ");

            DateTime there = DateTime.ParseExact(weather.lastBuildDate.Substring(0, AMPMidx + 4),
                "ddd, dd MMM yyyy hh:mm tt ", null);
            TimeSpan offset = there - utc;

            weather.location.offset = TimeSpan.Parse(string.Format("{0}:{1}", offset.Hours, offset.Minutes));
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

            if (!gotData || weather.units.temperature != Settings.Unit)
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

            // Check ttl
            int ttl = int.Parse(weather.ttl);

            // Check file age
            // ex. "2016-08-22T04:53:07Z"
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            DateTime updateTime = DateTime.ParseExact(weather.created,
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", provider).ToLocalTime();

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
                weatherData.Insert(locationIdx, location, weather);
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

        public static async Task<Weather> getWeather(string location)
        {
            string yahooAPI = "https://query.yahooapis.com/v1/public/yql?q=";
            string query = "select * from weather.forecast where woeid in (select woeid from geo.places(1) where text=\""
                + location + "\") and u='" + Settings.Unit + "'&format=json";
            Uri weatherURL = new Uri(yahooAPI + query);

            HttpClient webClient = new HttpClient();
            MemoryStream memStream = new MemoryStream();
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Rootobject));
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
                    weather = new Weather((Rootobject)deserializer.ReadObject(memStream));
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

            if (weather == null && wEx == null)
            {
                wEx = new WeatherException(WeatherUtils.ErrorStatus.NOWEATHER);
                throw wEx;
            }
            else if (weather == null && wEx != null)
            {
                throw wEx;
            }
            else if (weather != null)
            {
                saveTimeZone();
            }

            return weather;
        }

        private static void saveTimeZone()
        {
            // Now
            DateTime utc = DateTime.ParseExact(weather.created,
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", null);

            // There
            int AMPMidx = weather.lastBuildDate.LastIndexOf(" AM ");
            if (AMPMidx < 0)
                AMPMidx = weather.lastBuildDate.LastIndexOf(" PM ");

            DateTime there = DateTime.ParseExact(weather.lastBuildDate.Substring(0, AMPMidx + 4),
                "ddd, dd MMM yyyy hh:mm tt ", null);
            TimeSpan offset = there - utc;

            weather.location.offset = TimeSpan.Parse(string.Format("{0}:{1}", offset.Hours, offset.Minutes));
        }
    }
}
