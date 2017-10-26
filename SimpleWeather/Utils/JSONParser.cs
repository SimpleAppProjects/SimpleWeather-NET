using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
#if WINDOWS_UWP
using Windows.Storage;
#elif __ANDROID__
using Java.IO;
using Android.Support.V4.Util;
#endif

namespace SimpleWeather.Utils
{
    public static class JSONParser
    {
        private static JsonSerializerSettings DefaultSettings =
                new JsonSerializerSettings()
                { TypeNameHandling = TypeNameHandling.All };

        public static Object Deserializer(String response, Type type)
        {
            return JsonConvert.DeserializeObject(response, type, DefaultSettings);
        }

        public static Object Deserializer(Stream stream, Type type)
        {
            using (StreamReader sReader = new StreamReader(stream))
            using (JsonReader reader = new JsonTextReader(sReader))
            {
                JsonSerializer serializer = JsonSerializer.Create(DefaultSettings);
                return serializer.Deserialize(reader, type);
            }
        }

        public static T Deserializer<T>(String response)
        {
            return JsonConvert.DeserializeObject<T>(response, DefaultSettings);
        }

        public static T Deserializer<T>(Stream stream)
        {
            using (StreamReader sReader = new StreamReader(stream))
            using (JsonReader reader = new JsonTextReader(sReader))
            {
                JsonSerializer serializer = JsonSerializer.Create(DefaultSettings);
                return serializer.Deserialize<T>(reader);
            }
        }

        public static async Task<Object> DeserializerAsync(String response, Type type)
        {
            return await Task.Run(() => 
            {
                return JsonConvert.DeserializeObject(response, type, DefaultSettings);
            });
        }

        public static async Task<Object> DeserializerAsync(Stream stream, Type type)
        {
            return await Task.Run(() =>
            {
                using (StreamReader sReader = new StreamReader(stream))
                using (JsonReader reader = new JsonTextReader(sReader))
                {
                    JsonSerializer serializer = JsonSerializer.Create(DefaultSettings);
                    return serializer.Deserialize(reader, type);
                }
            });
        }

        public static async Task<T> DeserializerAsync<T>(String response)
        {
            return await Task.Run(() =>
            {
                return JsonConvert.DeserializeObject<T>(response, DefaultSettings);
            });
        }

        public static async Task<T> DeserializerAsync<T>(Stream stream)
        {
            return await Task.Run(() =>
            {
                using (StreamReader sReader = new StreamReader(stream))
                using (JsonReader reader = new JsonTextReader(sReader))
                {
                    JsonSerializer serializer = JsonSerializer.Create(DefaultSettings);
                    return serializer.Deserialize<T>(reader);
                }
            });
        }

#if WINDOWS_UWP
        public static async Task<T> DeserializerAsync<T>(StorageFile file)
#elif __ANDROID__
        public static async Task<T> DeserializerAsync<T>(Java.IO.File file)
#endif
        {
            return await Task.Run(async () =>
            {
                // Wait for file to be free
                while (FileUtils.IsFileLocked(file))
                {
                    await Task.Delay(100);
                }

#if __ANDROID__
                AtomicFile mFile = new AtomicFile(file);
                using (Stream fStream = mFile.OpenRead())
#else
                using (FileStream fStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read))
#endif
                using (StreamReader sReader = new StreamReader(fStream))
                using (JsonReader reader = new JsonTextReader(sReader))
                {
                    JsonSerializer serializer = JsonSerializer.Create(DefaultSettings);
                    return serializer.Deserialize<T>(reader);
                }
            }).ConfigureAwait(false);
        }

#if WINDOWS_UWP
        public static void Serializer(Object obj, StorageFile file)
        {
            Task.Run(async () =>
            {
                // Wait for file to be free
                while (FileUtils.IsFileLocked(file))
                {
                    await Task.Delay(100);
                }

                using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync())
                using (Stream fStream = transaction.Stream.AsStreamForWrite())
                using (JsonTextWriter writer = new JsonTextWriter(new StreamWriter(fStream)))
                {
                    // Clear file before writing
                    fStream.SetLength(0);

                    JsonSerializer serializer = JsonSerializer.Create(DefaultSettings);
                    serializer.Serialize(writer, obj);
                    await writer.FlushAsync();
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
                    await Task.Delay(100);
                }

                AtomicFile mFile = new AtomicFile(file);
                Stream fStream;

                try
                {
                    fStream = mFile.StartWrite();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.StackTrace);
                    return;
                }

                try
                {
                    JsonTextWriter writer = new JsonTextWriter(new StreamWriter(fStream))
                    {
                        CloseOutput = false,
                    };

                    JsonSerializer serializer = JsonSerializer.Create(DefaultSettings);
                    serializer.Serialize(writer, obj);
                    await writer.FlushAsync();
                    mFile.FinishWrite(fStream);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.StackTrace);
                    mFile.FailWrite(fStream);
                }
            }).ConfigureAwait(false);
        }
#endif

        public static string Serializer(Object obj, Type type)
        {
            return JsonConvert.SerializeObject(obj, type, DefaultSettings);
        }

        public static async Task<string> SerializerAsync(Object obj, Type type)
        {
            return await Task.Run(() => 
            {
                return JsonConvert.SerializeObject(obj, type, DefaultSettings);
            });
        }
    }
}
