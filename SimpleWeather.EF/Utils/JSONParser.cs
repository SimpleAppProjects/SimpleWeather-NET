using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

#if WINDOWS_UWP
using Windows.Storage;
#endif

namespace SimpleWeather.Utils
{
    public static class JSONParser
    {
        private static readonly JsonSerializerOptions DefaultSettings = default;

        public static Object Deserializer(String response, Type type)
        {
            Object obj = null;

            try
            {
                obj = JsonSerializer.Deserialize(response, type, DefaultSettings);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: JSONParser: error deserializing");
            }

            return obj;
        }

        public static Object Deserializer(Stream stream, Type type)
        {
            Object obj = null;

            try
            {
                return JsonSerializer.DeserializeAsync(stream, type, DefaultSettings).Result;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: JSONParser: error deserializing");
            }

            return obj;
        }

        public static T Deserializer<T>(String response)
        {
            return JsonSerializer.Deserialize<T>(response, DefaultSettings);
        }

        public static T Deserializer<T>(Stream stream)
        {
            var obj = default(T);

            try
            {
                return JsonSerializer.DeserializeAsync<T>(stream, DefaultSettings).Result;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: JSONParser: error deserializing");
            }

            return obj;
        }

        public static Task<Object> DeserializerAsync(String response, Type type)
        {
            return Task.Run(() => JsonSerializer.Deserialize(response, type, DefaultSettings));
        }

        public static Task<Object> DeserializerAsync(Stream stream, Type type)
        {
            Object obj = null;

            return Task.Run(async () =>
            {
                try
                {
                    return await JsonSerializer.DeserializeAsync(stream, type, DefaultSettings);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: JSONParser: error deserializing");
                }

                return obj;
            });
        }

        public static Task<T> DeserializerAsync<T>(String response)
        {
            return Task.Run(() => JsonSerializer.Deserialize<T>(response, DefaultSettings));
        }

        public static Task<T> DeserializerAsync<T>(Stream stream)
        {
            var obj = default(T);

            return Task.Run(async () =>
            {
                try
                {
                    return await JsonSerializer.DeserializeAsync<T>(stream, DefaultSettings);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: JSONParser: error deserializing");
                }

                return obj;
            });
        }

#if WINDOWS_UWP
        public static Task<T> DeserializerAsync<T>(StorageFile file)
        {
            return Task.Run(async () =>
            {
                // Wait for file to be free
                while (FileUtils.IsFileLocked(file))
                {
                    await Task.Delay(100).ConfigureAwait(false);
                }

                var obj = default(T);

                try
                {
                    using (var fStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read))
                    {
                        return await JsonSerializer.DeserializeAsync<T>(fStream, DefaultSettings);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: JSONParser: error deserializing or with file");
                    obj = default(T);
                }

                return obj;
            });
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

                    await JsonSerializer.SerializeAsync(fStream, obj, DefaultSettings);
                    await transaction.CommitAsync();
                }
            }).ConfigureAwait(false);
        }
#endif

        public static string Serializer(Object obj, Type type)
        {
            return JsonSerializer.Serialize(obj, type, DefaultSettings);
        }

        public static string Serializer<T>(T obj)
        {
            return JsonSerializer.Serialize<T>(obj, DefaultSettings);
        }

        public static Task<string> SerializerAsync(Object obj, Type type)
        {
            return Task.Run(() => JsonSerializer.Serialize(obj, type, DefaultSettings));
        }

        public static string CustomSerializer<T>(T obj) where T : CustomJsonObject
        {
            using (var writer = new Utf8JsonWriter(new MemoryStream()))
            {
                CustomSerializer(writer, obj);
                var str = writer.ToString();
                return str;
            }
        }

        public static T CustomDeserializer<T>(string json) where T : CustomJsonObject
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
            return CustomDeserializer<T>(reader);
        }

        public static string CustomEnumerableSerializer<T>(System.Collections.Generic.IEnumerable<T> obj) where T : CustomJsonObject
        {
            using (var writer = new Utf8JsonWriter(new MemoryStream()))
            {
                writer.WriteStartArray();
                foreach (var item in obj)
                {
                    CustomSerializer(writer, item);
                }
                writer.WriteEndArray();
                var str = writer.ToString();
                return str;
            };
        }

        public static System.Collections.Generic.List<T> CustomEnumerableDeserializer<T>(string json) where T : CustomJsonObject
        {
            var list = new System.Collections.Generic.List<T>();
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType == JsonTokenType.String)
                    list.Add(CustomDeserializer<T>(reader));
            }
            return list;
        }

        internal static bool CanCustomConvert(Type objectType)
        {
            return objectType.BaseType == typeof(CustomJsonObject);
        }

        internal static T CustomDeserializer<T>(Utf8JsonReader reader) where T : CustomJsonObject
        {
            T obj = null;
            try
            {
                obj = Activator.CreateInstance(typeof(T), true) as T;
                obj.FromJson(reader);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: CustomJsonConverter: error invoking FromJson method");
            }

            return obj;
        }

        internal static void CustomSerializer<T>(Utf8JsonWriter writer, T value) where T : CustomJsonObject
        {
            writer.WriteStringValue(value.ToJson());
        }
    }

    public class CustomJsonConverter<T> : JsonConverter<T> where T : CustomJsonObject
    {
        public override bool CanConvert(Type objectType)
        {
            return JSONParser.CanCustomConvert(objectType);
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JSONParser.CustomDeserializer<T>(reader);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JSONParser.CustomSerializer(writer, value);
        }
    }

    public abstract class CustomJsonObject
    {
        public abstract String ToJson();
        /// <summary>
        /// FromJson
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="JsonException"></exception>
        public abstract void FromJson(Utf8JsonReader reader);
    }
}