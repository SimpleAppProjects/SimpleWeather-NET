using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
#if WINDOWS_UWP
using Windows.Storage;
#elif __ANDROID__
using Android.Support.V4.Util;
#endif

namespace SimpleWeather.Utils
{
    public static class JSONParser
    {
        private static readonly JsonSerializerSettings DefaultSettings =
                new JsonSerializerSettings
                { TypeNameHandling = TypeNameHandling.All };

        public static Object Deserializer(String response, Type type)
        {
            Object obj = null;

            try
            {
                obj = JsonConvert.DeserializeObject(response, type, DefaultSettings);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return obj;
        }

        public static Object Deserializer(Stream stream, Type type)
        {
            Object obj = null;
            StreamReader sReader = null;
            JsonReader reader = null;

            try
            {
                sReader = new StreamReader(stream);
                reader = new JsonTextReader(sReader);

                var serializer = JsonSerializer.Create(DefaultSettings);
                obj = serializer.Deserialize(reader, type);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                reader?.Close();
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
            StreamReader sReader = null;
            JsonReader reader = null;

            try
            {
                sReader = new StreamReader(stream);
                reader = new JsonTextReader(sReader);

                var serializer = JsonSerializer.Create(DefaultSettings);
                obj = serializer.Deserialize<T>(reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                reader?.Close();
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
            StreamReader sReader = null;
            JsonReader reader = null;

            return Task.Run(() =>
            {
                try
                {
                    sReader = new StreamReader(stream);
                    reader = new JsonTextReader(sReader);

                    var serializer = JsonSerializer.Create(DefaultSettings);
                    obj = serializer.Deserialize(reader, type);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    reader?.Close();
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
            StreamReader sReader = null;
            JsonReader reader = null;

            return Task.Run(() =>
            {
                try
                {
                    sReader = new StreamReader(stream);
                    reader = new JsonTextReader(sReader);

                    var serializer = JsonSerializer.Create(DefaultSettings);
                    obj = serializer.Deserialize<T>(reader);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    reader?.Close();
                }

                return obj;
            });
        }

#if WINDOWS_UWP
        public static Task<T> DeserializerAsync<T>(StorageFile file)
#elif __ANDROID__
        public static Task<T> DeserializerAsync<T>(Java.IO.File file)
#endif
        {
            return Task.Run(async () =>
            {
                // Wait for file to be free
                while (FileUtils.IsFileLocked(file))
                {
                    await Task.Delay(100).ConfigureAwait(false);
                }

                Stream fStream = null;
                StreamReader sReader = null;
                JsonReader reader = null;
                var obj = default(T);
#if __ANDROID__
                var mFile = new AtomicFile(file);
                try
                {
                    fStream = mFile.OpenRead();
#else
                try
                {
                    fStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read);
#endif
                    sReader = new StreamReader(fStream);
                    reader = new JsonTextReader(sReader);

                    var serializer = JsonSerializer.Create(DefaultSettings);
                    obj = serializer.Deserialize<T>(reader);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    obj = default(T);
                }
                finally
                {
                    reader?.Close();
#if __ANDROID__
                    mFile?.Dispose();
#endif
                }

                return obj;
            });
        }

#if WINDOWS_UWP
        public static void Serializer(Object obj, StorageFile file)
        {
            Task.Run(async () =>
            {
                // Wait for file to be free
                while (FileUtils.IsFileLocked(file))
                {
                    await Task.Delay(100).ConfigureAwait(false);
                }

                using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync())
                using (Stream fStream = transaction.Stream.AsStreamForWrite())
                using (JsonTextWriter writer = new JsonTextWriter(new StreamWriter(fStream)))
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
#elif __ANDROID__
        public static void Serializer(Object obj, Java.IO.File file)
        {
            Task.Run(async () =>
            {
                // Wait for file to be free
                while (FileUtils.IsFileLocked(file))
                {
                    await Task.Delay(100).ConfigureAwait(false);
                }

                var mFile = new AtomicFile(file);
                Stream fStream = null;
                JsonTextWriter writer = null;

                try
                {
                    fStream = mFile.StartWrite();

                    writer = new JsonTextWriter(new StreamWriter(fStream))
                    {
                        CloseOutput = false,
                    };

                    var serializer = JsonSerializer.Create(DefaultSettings);
                    serializer.Serialize(writer, obj);
                    await writer.FlushAsync().ConfigureAwait(false);
                    mFile.FinishWrite(fStream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);

                    if (mFile != null && fStream != null)
                        mFile.FailWrite(fStream);
                }
                finally
                {
                    mFile?.Dispose();
                    writer?.Close();
                }
            }).ConfigureAwait(false);
        }
#endif

        public static string Serializer(Object obj, Type type)
        {
            return JsonConvert.SerializeObject(obj, type, DefaultSettings);
        }

        public static Task<string> SerializerAsync(Object obj, Type type)
        {
            return Task.Run(() => JsonConvert.SerializeObject(obj, type, DefaultSettings));
        }
    }

    public class CustomJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var fromjson = objectType.GetMethod("FromJson", new Type[] { typeof(JsonReader) });
            var tojson = objectType.GetMethod("ToJson");

            return fromjson != null && tojson != null && fromjson.IsStatic && fromjson.ReturnType == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
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
                    Console.WriteLine(ex.StackTrace);
                }

                if (obj != null)
                    return obj;
            }

            throw new JsonSerializationException(string.Format("{0} type does not implement FromJson(string) method", objectType.Name));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var tojson = value.GetType().GetMethod("ToJson");
            if (tojson != null)
                writer.WriteValue((string)tojson.Invoke(value, null));
        }
    }
}
