using System;
using System.IO;
using System.Threading.Tasks;
using Utf8Json;
using Windows.Storage;

namespace SimpleWeather.Utils
{
    public static partial class JSONParser
    {
        // CompositeResolver is singleton helper for use custom resolver.
        // Ofcourse you can also make custom resolver.
        public readonly static IJsonFormatterResolver Resolver =
            new SimpleWeather.Utf8JsonGen.Utf8JsonResolver();
        private static Newtonsoft.Json.JsonSerializerSettings DefaultSettings;

        static JSONParser()
        {
            JsonSerializer.SetDefaultResolver(Resolver);
        }

        public static T Deserializer<T>(String response)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(response);
            }
            catch (FormatterNotRegisteredException formatEx)
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
                    obj = JsonSerializer.Deserialize<T>(stream);
                }
                catch (FormatterNotRegisteredException formatEx)
                {
                    Logger.WriteLine(LoggerLevel.Warn, formatEx, "SimpleWeather: JSONParser: falling back to Newtonsoft.Json");
                    using (var reader = new Newtonsoft.Json.JsonTextReader(new StreamReader(stream)))
                    {
                        var serializer = Newtonsoft.Json.JsonSerializer.Create(DefaultSettings);
                        obj = serializer.Deserialize<T>(reader);
                    }
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
                using (var fStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        obj = JsonSerializer.Deserialize<T>(fStream);
                    }
                    catch (FormatterNotRegisteredException formatEx)
                    {
                        Logger.WriteLine(LoggerLevel.Warn, formatEx, "SimpleWeather: JSONParser: falling back to Newtonsoft.Json");
                        using (var reader = new Newtonsoft.Json.JsonTextReader(new StreamReader(fStream)))
                        {
                            var serializer = Newtonsoft.Json.JsonSerializer.Create(DefaultSettings);
                            obj = serializer.Deserialize<T>(reader);
                        }
                    }
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

                using (var transaction = await file.OpenTransactedWriteAsync())
                using (var fStream = transaction.Stream.AsStreamForWrite())
                {
                    // Clear file before writing
                    fStream.SetLength(0);

                    try
                    {
                        await JsonSerializer.SerializeAsync(fStream, obj);
                    }
                    catch (FormatterNotRegisteredException formatEx)
                    {
                        Logger.WriteLine(LoggerLevel.Warn, formatEx, "SimpleWeather: JSONParser: falling back to Newtonsoft.Json");
                        using (var writer = new Newtonsoft.Json.JsonTextWriter(new StreamWriter(fStream)))
                        {
                            fStream.SetLength(0);
                            var serializer = Newtonsoft.Json.JsonSerializer.Create(DefaultSettings);
                            serializer.Serialize(writer, obj);
                        }
                    }
                    await transaction.CommitAsync();
                }
            }).ConfigureAwait(false);
        }

        public static Task SerializerAsync(Object obj, StorageFile file)
        {
            return Task.Run(() => Serializer(obj, file));
        }

        public static string Serializer<T>(T obj)
        {
            try
            {
                return JsonSerializer.ToJsonString(obj);
            }
            catch (FormatterNotRegisteredException formatEx)
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
            if (DefaultSettings == null)
            {
                DefaultSettings = new Newtonsoft.Json.JsonSerializerSettings
                {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None,
                    ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver()
                };
            }

            return DefaultSettings;
        }
    }
}