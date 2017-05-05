using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using SimpleWeather.Utils;

namespace SimpleWeather.WeatherYahoo
{
    public class WeatherDataLoader
    {
        private WeatherLoadedListener callback;
        private string location = null;
        private Weather weather = null;
        private static StorageFolder appDataFolder = ApplicationData.Current.LocalFolder;
        private StorageFile weatherFile;
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
                await loadSavedWeatherData(weatherFile, true);
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
            string tz = weather.lastBuildDate.Substring(weather.lastBuildDate.Length - 3);
            DateTime there = DateTime.ParseExact(weather.lastBuildDate,
                "ddd, dd MMM yyyy hh:mm tt " + tz, null);
            TimeSpan offset = there - utc;

            weather.location.offset = TimeSpan.Parse(string.Format("{0}:{1}", offset.Hours, offset.Minutes));
        }

        public async Task loadWeatherData(bool forceRefresh)
        {
            if (weatherFile == null)
                weatherFile = await appDataFolder.CreateFileAsync("weather" + locationIdx + ".json", CreationCollisionOption.OpenIfExists);

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
            if (weatherFile == null)
                weatherFile = await appDataFolder.CreateFileAsync("weather" + locationIdx + ".json", CreationCollisionOption.OpenIfExists);

            /*
             * If unable to retrieve saved data, data is old, or units don't match
             * Refresh weather data
            */

            bool gotData = await loadSavedWeatherData(weatherFile);

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

        private async Task<bool> loadSavedWeatherData(StorageFile file, bool _override)
        {
            if (_override)
            {
                FileInfo fileInfo = new FileInfo(file.Path);

                if (!fileInfo.Exists || fileInfo.Length == 0)
                {
                    return false;
                }

                // Load weather data
                weather = (Weather)JSONParser.Deserializer(await FileUtils.ReadFile(weatherFile), typeof(Weather));

                if (weather == null)
                    return false;

                return true;
            }
            else
                return await loadSavedWeatherData(file);
        }

        private async Task<bool> loadSavedWeatherData(StorageFile file)
        {
            FileInfo fileInfo = new FileInfo(file.Path);

            if (!fileInfo.Exists || fileInfo.Length == 0)
            {
                return false;
            }

            // Load weather data
            weather = (Weather)JSONParser.Deserializer(await FileUtils.ReadFile(weatherFile), typeof(Weather));

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
            if (weatherFile == null)
                weatherFile = await appDataFolder.CreateFileAsync("weather" + locationIdx + ".json", CreationCollisionOption.OpenIfExists);

            JSONParser.Serializer(weather, weatherFile);
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

            return weather;
        }

        private static void saveTimeZone()
        {
            // Now
            DateTime utc = DateTime.ParseExact(weather.created,
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", null);

            // There
            string tz = weather.lastBuildDate.Substring(weather.lastBuildDate.Length - 3);
            DateTime there = DateTime.ParseExact(weather.lastBuildDate,
                "ddd, dd MMM yyyy hh:mm tt " + tz, null);
            TimeSpan offset = there - utc;

            weather.location.offset = TimeSpan.Parse(string.Format("{0}:{1}", offset.Hours, offset.Minutes));
        }
    }
}
