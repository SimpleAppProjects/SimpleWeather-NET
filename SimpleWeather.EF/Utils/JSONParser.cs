using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

#if WINDOWS_UWP
using Windows.Storage;
#endif

namespace SimpleWeather.Utils
{
    public static class JSONParser
    {
        static JSONParser()
        {
            JsonSerializer.SetDefaultResolver(Utf8Json.Resolvers.StandardResolver.AllowPrivate);
        }

        public static T Deserializer<T>(String response)
        {
            return JsonSerializer.Deserialize<T>(response);
        }

        public static T Deserializer<T>(Stream stream)
        {
            var obj = default(T);

            try
            {
                return JsonSerializer.Deserialize<T>(stream);
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
                        return await JsonSerializer.DeserializeAsync<T>(fStream);
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

                    await JsonSerializer.SerializeAsync(fStream, obj);
                    await transaction.CommitAsync();
                }
            }).ConfigureAwait(false);
        }
#endif

        public static string Serializer<T>(T obj)
        {
            return JsonSerializer.ToJsonString(obj);
        }

        public static Task<string> SerializerAsync<T>(T obj)
        {
            return Task.Run(() => Serializer(obj));
        }

        public static string CustomSerializer<T>(T obj) where T : CustomJsonObject
        {
            var writer = new JsonWriter();
            CustomSerializer(ref writer, obj);
            var str = writer.ToString();
            return str;
        }

        public static T CustomDeserializer<T>(string json) where T : CustomJsonObject
        {
            var reader = new JsonReader(Encoding.UTF8.GetBytes(json));
            return CustomDeserializer<T>(ref reader);
        }

        public static string CustomEnumerableSerializer<T>(System.Collections.Generic.IEnumerable<T> obj) where T : CustomJsonObject
        {
            var writer = new JsonWriter();
            writer.WriteBeginArray();
            var itemCount = 0;
            foreach (var item in obj)
            {
                if (itemCount > 0)
                    writer.WriteValueSeparator();
                CustomSerializer(ref writer, item);
                itemCount++;
            }
            writer.WriteEndArray();
            var str = writer.ToString();
            return str;
        }

        public static System.Collections.Generic.List<T> CustomEnumerableDeserializer<T>(string json) where T : CustomJsonObject
        {
            var list = new System.Collections.Generic.List<T>();
            var reader = new JsonReader(Encoding.UTF8.GetBytes(json));
            var count = 0; // managing array-count state in outer(this is count, not index(index is always count - 1)
            while ((reader.ReadIsInArray(ref count)))
            {
                if (reader.GetCurrentJsonToken() == JsonToken.String)
                    list.Add(CustomDeserializer<T>(ref reader));
            }
            return list;
        }

        internal static bool CanCustomConvert(Type objectType)
        {
            return objectType.BaseType == typeof(CustomJsonObject);
        }

        internal static T CustomDeserializer<T>(ref JsonReader reader) where T : CustomJsonObject
        {
            T obj = null;
            try
            {
                obj = Activator.CreateInstance(typeof(T), true) as T;
                obj.FromJson(ref reader);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: CustomJsonConverter: error invoking FromJson method");
            }

            return obj;
        }

        internal static void CustomSerializer<T>(ref JsonWriter writer, T value) where T : CustomJsonObject
        {
            writer.WriteString(value.ToJson());
        }
    }

    public class CustomJsonConverter<T> : IJsonFormatter<T> where T : CustomJsonObject
    {
        public T Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            return JSONParser.CustomDeserializer<T>(ref reader);
        }

        public void Serialize(ref JsonWriter writer, T value, IJsonFormatterResolver formatterResolver)
        {
            JSONParser.CustomSerializer(ref writer, value);
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
        public abstract void FromJson(ref JsonReader reader);
    }
}