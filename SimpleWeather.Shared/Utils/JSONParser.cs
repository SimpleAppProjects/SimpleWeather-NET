using SimpleWeather.Json;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
#if WINDOWS
using Windows.Storage;
#endif

namespace SimpleWeather.Utils
{
    public static partial class JSONParser
    {
        public static readonly JsonSerializerOptions DefaultSettings = new JsonSerializerOptions(JsonSerializerDefaults.General).Apply(opts =>
        {
            opts.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            opts.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            opts.WriteIndented = false;

            opts.Converters.Add(new CustomJsonConverter<LocationData.LocationData>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.Weather>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.Location>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.Forecast>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.HourlyForecast>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.MinutelyForecast>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.TextForecast>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.ForecastExtras>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.Condition>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.Atmosphere>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.Astronomy>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.Precipitation>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.Beaufort>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.MoonPhase>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.UV>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.AirQuality>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.Pollen>());
            opts.Converters.Add(new CustomJsonConverter<WeatherData.WeatherAlert>());

            opts.TypeInfoResolverChain.Insert(0, DataContractResolver.Default);
            opts.TypeInfoResolverChain.Add(SharedJsonContext.Default);
        });
        private static Newtonsoft.Json.JsonSerializerSettings JSONNETDefaultSettings;

        public static T Deserializer<T>(String response)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(response, options: DefaultSettings);
            }
            catch (JsonException formatEx)
            {
                Logger.WriteLine(LoggerLevel.Warn, formatEx, "SimpleWeather: JSONParser: falling back to Newtonsoft.Json");
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response, GetNewtonsoftSettings());
            }
        }

        public static T Deserializer<T>(Stream stream)
        {
            var obj = default(T);

            try
            {
                try
                {
                    obj = JsonSerializer.Deserialize<T>(stream, options: DefaultSettings);
                }
                catch (JsonException formatEx)
                {
                    Logger.WriteLine(LoggerLevel.Warn, formatEx, "SimpleWeather: JSONParser: falling back to Newtonsoft.Json");
                    using var reader = new Newtonsoft.Json.JsonTextReader(new StreamReader(stream));
                    var serializer = Newtonsoft.Json.JsonSerializer.Create(GetNewtonsoftSettings());
                    obj = serializer.Deserialize<T>(reader);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: JSONParser: error deserializing");
            }

            return obj;
        }

        public static Task<T> DeserializerAsync<T>(String response)
        {
            return Task.Run(() => Deserializer<T>(response));
        }

        public static Task<T> DeserializerAsync<T>(Stream stream)
        {
            return Task.Run(() => Deserializer<T>(stream));
        }

#if WINDOWS
        public static T Deserializer<T>(StorageFile file)
        {
            // Wait for file to be free
            while (FileUtils.IsFileLocked(file))
            {
                Task.Delay(100);
            }

            var obj = default(T);

            try
            {
                using var fStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read);

                try
                {
                    obj = JsonSerializer.Deserialize<T>(fStream, options: DefaultSettings);
                }
                catch (JsonException formatEx)
                {
                    Logger.WriteLine(LoggerLevel.Warn, formatEx, "SimpleWeather: JSONParser: falling back to Newtonsoft.Json");
                    using var reader = new Newtonsoft.Json.JsonTextReader(new StreamReader(fStream));
                    var serializer = Newtonsoft.Json.JsonSerializer.Create(GetNewtonsoftSettings());
                    obj = serializer.Deserialize<T>(reader);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: JSONParser: error deserializing or with file");
                obj = default(T);
            }

            return obj;
        }

        public static Task<T> DeserializerAsync<T>(StorageFile file)
        {
            return Task.Run(() => Deserializer<T>(file));
        }

        public static void Serializer(Object obj, StorageFile file)
        {
            Task.Run(async () =>
            {
                // Wait for file to be free
                while (FileUtils.IsFileLocked(file))
                {
                    await Task.Delay(100).ConfigureAwait(false);
                }

                using var transaction = await file.OpenTransactedWriteAsync();
                using var fStream = transaction.Stream.AsStreamForWrite();
                // Clear file before writing
                fStream.SetLength(0);

                try
                {
                    await JsonSerializer.SerializeAsync(fStream, obj, options: DefaultSettings);
                }
                catch (JsonException formatEx)
                {
                    Logger.WriteLine(LoggerLevel.Warn, formatEx, "SimpleWeather: JSONParser: falling back to Newtonsoft.Json");

                    using var writer = new Newtonsoft.Json.JsonTextWriter(new StreamWriter(fStream));
                    fStream.SetLength(0);

                    var serializer = Newtonsoft.Json.JsonSerializer.Create(GetNewtonsoftSettings());
                    serializer.Serialize(writer, obj);
                }
                await transaction.CommitAsync();
            }).ConfigureAwait(false);
        }

        public static Task SerializerAsync(Object obj, StorageFile file)
        {
            return Task.Run(() => Serializer(obj, file));
        }
#endif

        public static string Serializer<T>(T obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj, options: DefaultSettings);
            }
            catch (JsonException formatEx)
            {
                Logger.WriteLine(LoggerLevel.Warn, formatEx, "SimpleWeather: JSONParser: falling back to Newtonsoft.Json");
                return Newtonsoft.Json.JsonConvert.SerializeObject(obj, obj.GetType(), GetNewtonsoftSettings());
            }
        }

        public static Task<string> SerializerAsync<T>(T obj)
        {
            return Task.Run(() => Serializer(obj));
        }

        private static Newtonsoft.Json.JsonSerializerSettings GetNewtonsoftSettings()
        {
            JSONNETDefaultSettings ??= new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None,
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver()
            };

            return JSONNETDefaultSettings;
        }
    }
}