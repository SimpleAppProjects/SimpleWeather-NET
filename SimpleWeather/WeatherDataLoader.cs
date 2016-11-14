using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace SimpleWeather
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
            string unit = (Settings.useFarenheit() ? " and u='F'" : " and u='C'");
            string query = "select * from weather.forecast where woeid in (select woeid from geo.places(1) where text=\""
                + location + "\")" + unit + "&format=json";
            Uri weatherURL = new Uri(yahooAPI + query);

            // Connect to webstream
            HttpClient webClient = new HttpClient();
            HttpResponseMessage response = await webClient.GetAsync(weatherURL);
            response.EnsureSuccessStatusCode();
            IBuffer buff = await response.Content.ReadAsBufferAsync();

            // Write array/buffer to memorystream
            MemoryStream memStream = new MemoryStream();
            await memStream.AsOutputStream().WriteAsync(buff);
            memStream.Seek(0, 0);

            // End Stream
            webClient.Dispose();

            // Load weather
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Rootobject));

            try
            {
                weather = new Weather((Rootobject)deserializer.ReadObject(memStream));
            }
            catch (Exception e)
            {
                /* TODO: DEBUG - remove logging */
                weather = null;
                System.Diagnostics.Debug.WriteLine(e.HResult + ": " + e.Message);
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

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
             * If unable to retrieve saved data or data is old
             * Refresh weather data
            */

            bool gotData = await loadSavedWeatherData(weatherFile);

            if (!gotData)
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

            while (IsFileLocked(file).Result)
            {
                await Task.Delay(100);
            }

            // Load weather data
            DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(typeof(Weather));
            Stream fileStream = await file.OpenStreamForReadAsync();

            weather = (Weather)deSerializer.ReadObject(fileStream);
            fileStream.Flush();
            fileStream.Dispose();

            // Check ttl
            int ttl = int.Parse(weather.ttl);

            // Check file age
            // ex. "2016-08-22T04:53:07Z"
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            DateTime updateTime = DateTime.ParseExact(weather.lastBuildDate,
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", provider).ToLocalTime();

            TimeSpan span = DateTime.Now - updateTime;
            if (span.Minutes < ttl)
                return true;
            else
                return false;
        }

        private async void saveWeatherData()
        {
            MemoryStream memStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Weather));
            serializer.WriteObject(memStream, weather);

            weatherFile = await appDataFolder.CreateFileAsync("weather" + locationIdx + ".json",
                CreationCollisionOption.OpenIfExists);

            while (IsFileLocked(weatherFile).Result)
            {
                await Task.Delay(100);
            }

            FileRandomAccessStream fileStream = (await weatherFile.OpenAsync(FileAccessMode.ReadWrite)) as FileRandomAccessStream;
            fileStream.Size = 0;
            memStream.WriteTo(fileStream.AsStream());

            await fileStream.AsStream().FlushAsync();
            fileStream.Dispose();
            await memStream.FlushAsync();
            memStream.Dispose();
        }

        public Weather getWeather()
        {
            return weather;
        }

        private static async Task<bool> IsFileLocked(StorageFile file)
        {
            if (!File.Exists(file.Name))
                return false;

            IRandomAccessStream stream = null;

            try
            {
                stream = await file.OpenAsync(FileAccessMode.Read);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }

            //file is not locked
            return false;
        }
    }
}
