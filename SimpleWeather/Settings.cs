using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SimpleWeather
{
    public static class Settings
    {
        public static bool WeatherLoaded { get { return isWeatherLoaded(); } set { setWeatherLoaded(value); } }

        private static StorageFolder appDataFolder = ApplicationData.Current.LocalFolder;
        private static StorageFile locationsFile;

        public static bool useFarenheit()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.ContainsKey("Units") || localSettings.Values["Units"] == null)
            {
                localSettings.Values["Units"] = "F";
                return true;
            }
            else if (localSettings.Values["Units"].Equals("C"))
                return false;

            return true;
        }

        private static bool isWeatherLoaded()
        {
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
            locationsFile = await appDataFolder.CreateFileAsync("locations.json", CreationCollisionOption.OpenIfExists);
            FileInfo fileinfo = new FileInfo(locationsFile.Path);

            if (fileinfo.Length == 0)
                return null;

            List<Coordinate> locations;

            // Load locations
            DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(typeof(List<Coordinate>));
            Stream fileStream = await locationsFile.OpenStreamForReadAsync();

            while (IsFileLocked(locationsFile).Result)
            {
                await Task.Delay(100);
            }

            locations = ((List<Coordinate>)deSerializer.ReadObject(fileStream));
            fileStream.Flush();
            fileStream.Dispose();

            return locations;
        }

        public static async void saveLocations(List<Coordinate> locations)
        {
            MemoryStream memStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Coordinate>));
            serializer.WriteObject(memStream, locations);

            locationsFile = await appDataFolder.CreateFileAsync("locations.json",
                CreationCollisionOption.OpenIfExists);

            while (IsFileLocked(locationsFile).Result)
            {
                await Task.Delay(100);
            }

            FileRandomAccessStream fileStream = (await locationsFile.OpenAsync(FileAccessMode.ReadWrite)) as FileRandomAccessStream;
            fileStream.Size = 0;
            memStream.WriteTo(fileStream.AsStream());

            await fileStream.AsStream().FlushAsync();
            fileStream.Dispose();
            await memStream.FlushAsync();
            memStream.Dispose();
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
