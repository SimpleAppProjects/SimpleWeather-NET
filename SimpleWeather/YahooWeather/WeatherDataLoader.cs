using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace SimpleWeather.WeatherYahoo
{
    public class WeatherDataLoader
    {
        private string location = null;
        private Weather weather = null;
        private static StorageFolder appDataFolder = ApplicationData.Current.LocalFolder;
        private StorageFile weatherFile;
        private int locationIdx = 0;

        public WeatherDataLoader(string Location, int idx)
        {
            location = Location;
            locationIdx = idx;
        }

        public WeatherDataLoader(Geoposition geoPosition, int idx)
        {
            location = "(" + geoPosition.Coordinate.Point.Position.Latitude + ", "
                + geoPosition.Coordinate.Point.Position.Longitude + ")";
            locationIdx = idx;
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

            do
            {
                // Get response
                HttpResponseMessage response = await webClient.GetAsync(weatherURL);
                response.EnsureSuccessStatusCode();
                IBuffer buff = await response.Content.ReadAsBufferAsync();

                // Write array/buffer to memorystream
                memStream.SetLength(0);
                await memStream.AsOutputStream().WriteAsync(buff);
                memStream.Seek(0, 0);

                try
                {
                    // Load weather
                    weather = new Weather((Rootobject)deserializer.ReadObject(memStream));
                }
                catch (Exception e)
                {
                    /* TODO: DEBUG - remove logging */
                    weather = null;
                    System.Diagnostics.Debug.WriteLine(e.HResult + ": " + e.Message);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
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

            saveWeatherData();
        }

        public async Task loadWeatherData(bool forceRefresh)
        {
            if (weatherFile == null)
                weatherFile = await appDataFolder.CreateFileAsync("weather" + locationIdx + ".json", CreationCollisionOption.OpenIfExists);

            if (forceRefresh)
                await getWeatherData();
            else
                await loadWeatherData();
        }

        public async Task loadWeatherData()
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
                await getWeatherData();
            }
        }

        private async Task<bool> loadSavedWeatherData(StorageFile file)
        {
            FileInfo fileInfo = new FileInfo(file.Path);

            if (!fileInfo.Exists || fileInfo.Length == 0)
            {
                return false;
            }

            while (FileUtils.IsFileLocked(weatherFile).GetAwaiter().GetResult())
            {
                await Task.Delay(100);
            }

            // Load weather data
            using (FileRandomAccessStream fileStream = (await weatherFile.OpenAsync(FileAccessMode.Read).AsTask().ConfigureAwait(false)) as FileRandomAccessStream)
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

            // Check ttl
            int ttl = int.Parse(weather.ttl);

            // Check file age
            // ex. "2016-08-22T04:53:07Z"
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            DateTime updateTime = DateTime.ParseExact(weather.lastBuildDate,
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

            while (FileUtils.IsFileLocked(weatherFile).GetAwaiter().GetResult())
            {
                await Task.Delay(100);
            }

            using (FileRandomAccessStream fileStream = (await weatherFile.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false)) as FileRandomAccessStream)
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
