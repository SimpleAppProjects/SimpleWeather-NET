using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SimpleWeather
{
    public static class Settings
    {
        public static bool WeatherLoaded { get { return isWeatherLoaded(); } set { setWeatherLoaded(value); } }
        public static string Unit { get { return getTempUnit(); } set { setTempUnit(value); } }

        private static StorageFolder appDataFolder = ApplicationData.Current.LocalFolder;
        private static StorageFile locationsFile;

        private static string Farenheit = "F";
        private static string Celsius = "C";

        private static string getTempUnit()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.ContainsKey("Units") || localSettings.Values["Units"] == null)
            {
                return Farenheit;
            }
            else if (localSettings.Values["Units"].Equals("C"))
                return Celsius;

            return Farenheit;
        }

        private static void setTempUnit(string value)
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            if (value == Celsius)
                ApplicationData.Current.LocalSettings.Values["Units"] = Celsius;
            else
                ApplicationData.Current.LocalSettings.Values["Units"] = Farenheit;
        }

        private static bool isWeatherLoaded()
        {
            if (locationsFile == null)
                locationsFile = appDataFolder.CreateFileAsync("locations.json", CreationCollisionOption.OpenIfExists).AsTask().GetAwaiter().GetResult();

            FileInfo fileinfo = new FileInfo(locationsFile.Path);

            if (fileinfo.Length == 0 || !fileinfo.Exists)
                return false;

            var localSettings = ApplicationData.Current.LocalSettings;

            if (localSettings.Values["weatherLoaded"] == null)
            {
                return false;
            }
            else if (localSettings.Values["weatherLoaded"].Equals("true"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void setWeatherLoaded(bool isLoaded)
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            if (isLoaded)
            {
                localSettings.Values["weatherLoaded"] = "true";
            }
            else
            {
                localSettings.Values["weatherLoaded"] = "false";
            }
        }

        public static async Task<List<Coordinate>> getLocations()
        {
            if (locationsFile == null)
                locationsFile = await appDataFolder.CreateFileAsync("locations.json", CreationCollisionOption.OpenIfExists);

            FileInfo fileinfo = new FileInfo(locationsFile.Path);

            if (fileinfo.Length == 0 || !fileinfo.Exists)
                return null;

            while (FileUtils.IsFileLocked(locationsFile).GetAwaiter().GetResult())
            {
                await Task.Delay(100);
            }

            List<Coordinate> locations;

            // Load locations
            using (FileRandomAccessStream fileStream = (await locationsFile.OpenAsync(FileAccessMode.Read).AsTask().ConfigureAwait(false)) as FileRandomAccessStream)
            {
                DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(typeof(List<Coordinate>));
                MemoryStream memStream = new MemoryStream();
                fileStream.AsStreamForRead().CopyTo(memStream);
                memStream.Seek(0, 0);

                locations = ((List<Coordinate>)deSerializer.ReadObject(memStream));

                await fileStream.AsStream().FlushAsync();
                fileStream.Dispose();
                await memStream.FlushAsync();
                memStream.Dispose();
            }

            return locations;
        }

        public static async void saveLocations(List<Coordinate> locations)
        {
            if (locationsFile == null)
                locationsFile = await appDataFolder.CreateFileAsync("locations.json", CreationCollisionOption.OpenIfExists);

            while (FileUtils.IsFileLocked(locationsFile).GetAwaiter().GetResult())
            {
                await Task.Delay(100);
            }

            using (FileRandomAccessStream fileStream = (await locationsFile.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false)) as FileRandomAccessStream)
            {
                MemoryStream memStream = new MemoryStream();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Coordinate>));
                serializer.WriteObject(memStream, locations);

                fileStream.Size = 0;
                memStream.WriteTo(fileStream.AsStream());

                await memStream.FlushAsync();
                memStream.Dispose();
                await fileStream.AsStream().FlushAsync();
                fileStream.Dispose();
            }
        }
    }
}
