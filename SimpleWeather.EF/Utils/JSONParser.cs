using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Threading.Tasks;

#if WINDOWS_UWP
using Windows.Storage;
#endif

namespace SimpleWeather.Utils
{
    public static class JSONParser
    {
        private static readonly JsonSerializerSettings DefaultSettings =
                new JsonSerializerSettings
                { TypeNameHandling = TypeNameHandling.All, ContractResolver = new DefaultContractResolver() };

        public static Object Deserializer(String response, Type type)
        {
            Object obj = null;

            try
            {
                obj = JsonConvert.DeserializeObject(response, type, DefaultSettings);
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
                using (var sReader = new StreamReader(stream))
                using (var reader = new JsonTextReader(sReader))
                {
                    var serializer = JsonSerializer.Create(DefaultSettings);
                    obj = serializer.Deserialize(reader, type);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: JSONParser: error deserializing");
            }

            return obj;
        }

        public static T Deserializer<T>(String response)
        {
            return JsonConvert.DeserializeObject<T>(response, DefaultSettings);
        }

        public static T Deserializer<T>(Stream stream)
        {
            var obj = default(T);

            try
            {
                using (var sReader = new StreamReader(stream))
                using (var reader = new JsonTextReader(sReader))
                {
                    var serializer = JsonSerializer.Create(DefaultSettings);
                    obj = serializer.Deserialize<T>(reader);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: JSONParser: error deserializing");
            }

            return obj;
        }

        public static Task<Object> DeserializerAsync(String response, Type type)
        {
            return Task.Run(() => JsonConvert.DeserializeObject(response, type, DefaultSettings));
        }

        public static Task<Object> DeserializerAsync(Stream stream, Type type)
        {
            Object obj = null;

            return Task.Run(() =>
            {
                try
                {
                    using (var sReader = new StreamReader(stream))
                    using (var reader = new JsonTextReader(sReader))
                    {
                        var serializer = JsonSerializer.Create(DefaultSettings);
                        obj = serializer.Deserialize(reader, type);
                    }
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
            return Task.Run(() => JsonConvert.DeserializeObject<T>(response, DefaultSettings));
        }

        public static Task<T> DeserializerAsync<T>(Stream stream)
        {
            var obj = default(T);

            return Task.Run(() =>
            {
                try
                {
                    using (var sReader = new StreamReader(stream))
                    using (var reader = new JsonTextReader(sReader))
                    {
                        var serializer = JsonSerializer.Create(DefaultSettings);
                        obj = serializer.Deserialize<T>(reader);
                    }
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
                    using (var sReader = new StreamReader(fStream))
                    using (var reader = new JsonTextReader(sReader))
                    {
                        var serializer = JsonSerializer.Create(DefaultSettings);
                        obj = serializer.Deserialize<T>(reader);
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
                using (var writer = new JsonTextWriter(new StreamWriter(fStream)))
                {
                    // Clear file before writing
                    fStream.SetLength(0);

                    var serializer = JsonSerializer.Create(DefaultSettings);
                    serializer.Serialize(writer, obj);
                    await writer.FlushAsync().ConfigureAwait(false);
                    await transaction.CommitAsync();
                }
            }).ConfigureAwait(false);
        }
#endif

        public static string Serializer(Object obj, Type type)
        {
            return JsonConvert.SerializeObject(obj, type, DefaultSettings);
        }

        public static string Serializer<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, obj.GetType(), DefaultSettings);
        }

        public static Task<string> SerializerAsync(Object obj, Type type)
        {
            return Task.Run(() => JsonConvert.SerializeObject(obj, type, DefaultSettings));
        }

        public static string CustomSerializer<T>(T obj) where T : class
        {
            var objType = typeof(T);

            if (!CanCustomConvert(objType))
                return null;
            else
            {
                using (var writer = new JsonTextWriter(new StringWriter()) { CloseOutput = true })
                {
                    CustomSerializer(writer, obj);
                    return writer.ToString();
                }
            }
        }

        public static T CustomDeserializer<T>(string json) where T : class
        {
            var objType = typeof(T);

            if (!CanCustomConvert(objType))
                return default(T);
            else
            {
                using (var reader = new JsonTextReader(new StringReader(json)) { CloseInput = true })
                {
                    return CustomDeserializer(reader, objType) as T;
                }
            }
        }

        public static string CustomEnumerableSerializer<T>(System.Collections.Generic.IEnumerable<T> obj) where T : class
        {
            var objType = typeof(T);

            if (!CanCustomConvert(objType))
                return null;
            else
            {
                using (var sw = new StringWriter())
                using (var writer = new JsonTextWriter(sw) { CloseOutput = true })
                {
                    writer.WriteStartArray();
                    foreach (var item in obj)
                    {
                        CustomSerializer(writer, item);
                    }
                    writer.WriteEndArray();
                    return writer.ToString();
                };
            }
        }

        public static System.Collections.Generic.List<T> CustomEnumerableDeserializer<T>(string json) where T : class
        {
            var objType = typeof(T);

            if (!CanCustomConvert(objType))
                return null;
            else
            {
                var list = new System.Collections.Generic.List<T>();
                using (var reader = new JsonTextReader(new StringReader(json)) { CloseInput = true })
                {
                    while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                    {
                        if (reader.TokenType == JsonToken.String)
                            list.Add(CustomDeserializer(reader, objType) as T);
                    }
                    return list;
                }
            }
        }

        internal static bool CanCustomConvert(Type objectType)
        {
            var fromjson = objectType.GetMethod("FromJson", new Type[] { typeof(JsonReader) });
            var tojson = objectType.GetMethod("ToJson");

            return fromjson != null && tojson != null && fromjson.IsStatic && fromjson.ReturnType == objectType;
        }

        internal static object CustomDeserializer(JsonReader reader, Type objectType)
        {
            var fromjson = objectType.GetMethod("FromJson", new Type[] { typeof(JsonReader) });
            if (fromjson != null && fromjson.IsStatic && fromjson.ReturnType == objectType)
            {
                object obj = null;
                try
                {
                    obj = fromjson.Invoke(null, new object[] { reader });
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: CustomJsonConverter: error invoking FromJson method");
                }

                if (obj != null)
                    return obj;
            }

            throw new JsonSerializationException(string.Format("{0} type does not implement FromJson(string) method", objectType.Name));
        }

        internal static void CustomSerializer(JsonWriter writer, object value)
        {
            var tojson = value.GetType().GetMethod("ToJson");
            if (tojson != null)
            {
                writer.WriteValue((string)tojson.Invoke(value, null));
            }
            else
            {
                Logger.WriteLine(LoggerLevel.Error, "SimpleWeather: CustomJsonConverter: error invoking ToJson method");
                Logger.WriteLine(LoggerLevel.Error, "SimpleWeather: CustomJsonConverter: object: {0}", value?.ToString());
            }
        }
    }

    public class CustomJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return JSONParser.CanCustomConvert(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return JSONParser.CustomDeserializer(reader, objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JSONParser.CustomSerializer(writer, value);
        }
    }
}