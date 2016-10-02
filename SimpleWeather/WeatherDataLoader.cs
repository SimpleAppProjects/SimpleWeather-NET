using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace SimpleWeather
{
    public class WeatherDataLoader
    {
        private string location = null;
        private Weather weather = null;
        private StorageFolder appDataFolder = ApplicationData.Current.LocalFolder;
        private StorageFile weatherFile;

        public WeatherDataLoader(string Location)
        {
            location = Location;
        }

        public WeatherDataLoader(Geoposition geoPosition)
        {
            location = "(" + geoPosition.Coordinate.Point.Position.Latitude + ", "
                + geoPosition.Coordinate.Point.Position.Longitude + ")";
        }

        private async Task getWeatherData()
        {
            string yahooAPI = "https://query.yahooapis.com/v1/public/yql?q=";
            string query = "select * from weather.forecast where woeid in (select woeid from geo.places(1) where text=\""
                + location + "\")&format=json";
            Uri weatherURL = new Uri(yahooAPI + query);

            // Connect to webstream
            Windows.Web.Http.HttpClient webClient = new Windows.Web.Http.HttpClient();
            Windows.Web.Http.HttpResponseMessage response = await webClient.GetAsync(weatherURL);
            response.EnsureSuccessStatusCode();
            Windows.Storage.Streams.IBuffer fileBuff = await response.Content.ReadAsBufferAsync();

            // Write array/buffer to json file
            await FileIO.WriteBufferAsync(weatherFile, fileBuff);

            // End Stream
            webClient.Dispose();

            // Load weather
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Rootobject));
            Stream fileStream = await weatherFile.OpenStreamForReadAsync();
            weather = new Weather((Rootobject)serializer.ReadObject(fileStream));
        }

        public async Task loadWeatherData(bool forceRefresh)
        {
            weatherFile = await appDataFolder.CreateFileAsync("weather.json",
                CreationCollisionOption.OpenIfExists);
            FileInfo weatherFileInfo = new FileInfo(weatherFile.Path);
            TimeSpan span = DateTime.Now - weatherFileInfo.LastWriteTime;

            /* TODO: make span check an app property setting */
            if (forceRefresh || (span.TotalHours > 6) || (weatherFileInfo.Length == 0))
            {
                await getWeatherData();
            }
            else if (weather == null)
            {
                // Load weather from file
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Rootobject));
                Stream fileStream = await weatherFile.OpenStreamForReadAsync();
                weather = new Weather((Rootobject)serializer.ReadObject(fileStream));
            }
        }

        public Weather getWeather()
        {
            return weather;
        }
    }
}
