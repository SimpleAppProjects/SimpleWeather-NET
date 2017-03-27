using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using SimpleWeather.Utils;

namespace SimpleWeather.WeatherUnderground
{
    public class WeatherDataLoader
    {
        private string location_query = null;
        private Weather weather = null;
        private static StorageFolder appDataFolder = ApplicationData.Current.LocalFolder;
        private StorageFile weatherFile;
        private int locationIdx = 0;

        public WeatherDataLoader(string query, int idx)
        {
            location_query = query;
            locationIdx = idx;
        }

        private async Task<WeatherUtils.ErrorStatus> getWeatherData()
        {
            string queryAPI = "http://api.wunderground.com/api/" + Settings.API_KEY + "/astronomy/conditions/forecast10day";
            string options = ".json";
            Uri weatherURL = new Uri(queryAPI + location_query + options);

            HttpClient webClient = new HttpClient();
            MemoryStream memStream = new MemoryStream();
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Rootobject));
            int counter = 0;
            WeatherUtils.ErrorStatus ret = WeatherUtils.ErrorStatus.UNKNOWN;

            do
            {
                try
                {
                    // Get response
                    HttpResponseMessage response = await webClient.GetAsync(weatherURL);
                    response.EnsureSuccessStatusCode();
                    IBuffer buff = await response.Content.ReadAsBufferAsync();

                    // Write array/buffer to memorystream
                    memStream.SetLength(0);
                    await memStream.AsOutputStream().WriteAsync(buff);
                    memStream.Seek(0, 0);

                    // Load weather
                    Rootobject root = (Rootobject)deserializer.ReadObject(memStream);

                    // Check for errors
                    if (root.response.error != null)
                    {
                        switch (root.response.error.type)
                        {
                            case "querynotfound":
                                ret = WeatherUtils.ErrorStatus.QUERYNOTFOUND;
                                break;
                            case "keynotfound":
                                ret = WeatherUtils.ErrorStatus.INVALIDAPIKEY;
                                break;
                            default:
                                break;
                        }
                        break;
                    }

                    weather = new Weather(root);
                }
                catch (Exception ex)
                {
                    weather = null;
                    if (Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
                    {
                        ret = WeatherUtils.ErrorStatus.NETWORKERROR;
                        break;
                    }
                }

                // If we can't load data, delay and try again
                if (weather == null)
                    await Task.Delay(1000);

                counter++;
            } while (weather == null && counter < 5);

            // Load old data if available and we can't get new data
            if (weather == null)
            {
                await loadSavedWeatherData(weatherFile, true);
            }

            // End Stream
            webClient.Dispose();

            await memStream.FlushAsync();
            memStream.Dispose();

            if (weather == null && ret == WeatherUtils.ErrorStatus.UNKNOWN)
            {
                ret = WeatherUtils.ErrorStatus.NOWEATHER;
            }
            else if (weather != null)
            {
                saveWeatherData();
                ret = WeatherUtils.ErrorStatus.SUCCESS;
            }

            return ret;
        }

        public async Task<WeatherUtils.ErrorStatus> loadWeatherData(bool forceRefresh)
        {
            WeatherUtils.ErrorStatus ret = WeatherUtils.ErrorStatus.UNKNOWN;

            if (weatherFile == null)
                weatherFile = await appDataFolder.CreateFileAsync("weather" + locationIdx + ".json", CreationCollisionOption.OpenIfExists);

            if (forceRefresh)
                ret = await getWeatherData();
            else
                ret = await loadWeatherData();

            return ret;
        }

        public async Task<WeatherUtils.ErrorStatus> loadWeatherData()
        {
            WeatherUtils.ErrorStatus ret = WeatherUtils.ErrorStatus.UNKNOWN;

            if (weatherFile == null)
                weatherFile = await appDataFolder.CreateFileAsync("weather" + locationIdx + ".json", CreationCollisionOption.OpenIfExists);

            /*
             * If unable to retrieve saved data, data is old, or units don't match
             * Refresh weather data
            */

            bool gotData = await loadSavedWeatherData(weatherFile);

            if (!gotData)
            {
                ret = await getWeatherData();
            }
            else
                ret = WeatherUtils.ErrorStatus.SUCCESS;

            return ret;
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

                while (FileUtils.IsFileLocked(weatherFile))
                {
                    await Task.Delay(100);
                }

                // Load weather data
                using (FileRandomAccessStream fileStream = (await weatherFile.OpenAsync(FileAccessMode.Read)) as FileRandomAccessStream)
                {
                    DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(typeof(Weather));
                    MemoryStream memStream = new MemoryStream();
                    fileStream.AsStreamForRead().CopyTo(memStream);
                    memStream.Seek(0, 0);

                    weather = (Weather)deSerializer.ReadObject(memStream);

                    await fileStream.AsStream().FlushAsync();
                    fileStream.Dispose();
                    await memStream.FlushAsync();
                    memStream.Dispose();
                }

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

            while (FileUtils.IsFileLocked(weatherFile))
            {
                await Task.Delay(100);
            }

            // Load weather data
            using (FileRandomAccessStream fileStream = (await weatherFile.OpenAsync(FileAccessMode.Read)) as FileRandomAccessStream)
            {
                DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(typeof(Weather));
                MemoryStream memStream = new MemoryStream();
                fileStream.AsStreamForRead().CopyTo(memStream);
                memStream.Seek(0, 0);

                weather = (Weather)deSerializer.ReadObject(memStream);

                await fileStream.AsStream().FlushAsync();
                fileStream.Dispose();
                await memStream.FlushAsync();
                memStream.Dispose();
            }

            if (weather == null)
                return false;

            // Weather data expiration
            int ttl = 60;

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
            if (weatherFile == null)
                weatherFile = await appDataFolder.CreateFileAsync("weather" + locationIdx + ".json", CreationCollisionOption.OpenIfExists);

            while (FileUtils.IsFileLocked(weatherFile))
            {
                await Task.Delay(100);
            }

            using (FileRandomAccessStream fileStream = (await weatherFile.OpenAsync(FileAccessMode.ReadWrite)) as FileRandomAccessStream)
            {
                MemoryStream memStream = new MemoryStream();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Weather));
                serializer.WriteObject(memStream, weather);
                fileStream.Size = 0;
                memStream.WriteTo(fileStream.AsStream());

                await memStream.FlushAsync();
                memStream.Dispose();
                await fileStream.AsStream().FlushAsync();
                fileStream.Dispose();
            }
        }

        public Weather getWeather()
        {
            return weather;
        }
    }
}
