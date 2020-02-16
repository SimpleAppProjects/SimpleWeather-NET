using Newtonsoft.Json;
using SimpleWeather.Location;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.Utils
{
    public static partial class DBMigrations
    {
        public static ConfiguredTaskAwaitable MigrateDataJsonToDB(LocationDBContext locationDB, WeatherDBContext weatherDB)
        {
            return AsyncTask.RunAsync(async () =>
            {
                var appDataFolder = ApplicationData.Current.LocalFolder;

                var locFileInfo = new FileInfo(Path.Combine(appDataFolder.Path, "locations.json"));
                var dataFileInfo = new FileInfo(Path.Combine(appDataFolder.Path, "data.json"));

                if (!locFileInfo.Exists)
                {
                    if (!dataFileInfo.Exists)
                        return; // No data to migrate
                }

                var locDataFile = (await appDataFolder.TryGetItemAsync("locations.json")) as StorageFile;
                var dataFile = (await appDataFolder.TryGetItemAsync("data.json")) as StorageFile;

                List<LocationData> locationData = null;
                if (locDataFile != null && locFileInfo.Exists && locFileInfo.Length > 0)
                {
                    // Migrate to new structure
                    try
                    {
                        locationData = await JSONParser.DeserializerAsync<List<LocationData>>(locDataFile).ConfigureAwait(false);
                    }
                    catch (JsonSerializationException jsEx)
                    {
                        Logger.WriteLine(LoggerLevel.Error, jsEx, "SimpleWeather: DataMigration: location json error");
                        locationData = null;
                    }

                    await Settings.SaveLocationData(locationData);

                    await locDataFile.DeleteAsync();
                    locDataFile = null;
                }

                if (dataFile != null && dataFileInfo.Exists && dataFileInfo.Length > 0)
                {
                    OrderedDictionary oldWeather = null;
                    try
                    {
                        oldWeather = await JSONParser.DeserializerAsync<OrderedDictionary>(dataFile).ConfigureAwait(false);
                    }
                    catch (JsonSerializationException jsEx)
                    {
                        Logger.WriteLine(LoggerLevel.Error, jsEx, "SimpleWeather: DataMigration: weather json error");
                        if (oldWeather == null)
                            oldWeather = new OrderedDictionary();
                    }

                    // Setup location data if N/A
                    if (locationData == null || locationData.Count == 0)
                    {
                        List<LocationData> data = new List<LocationData>(oldWeather.Count);
                        foreach (string query in oldWeather.Keys.Cast<string>())
                        {
                            var weather = oldWeather[query] as Weather;
                            var prov = WeatherManager.GetProvider(weather.source);
                            var qview = await AsyncTask.RunAsync(prov.GetLocation(
                                new WeatherUtils.Coordinate(String.Format("{0},{1}", weather.location.latitude, weather.location.longitude))));

                            LocationData loc = new LocationData(qview);
                            data.Add(loc);
                        }

                        locationData = data;
                        await Settings.SaveLocationData(locationData);
                    }

                    // Add data
                    var list = oldWeather.Values.Cast<Weather>();
                    weatherDB.WeatherData.UpdateRange(list);
                    await weatherDB.SaveChangesAsync();

                    // Delete old files
                    await dataFile.DeleteAsync();
                    dataFile = null;
                }
            });
        }
    }
}