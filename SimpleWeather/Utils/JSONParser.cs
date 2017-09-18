using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
#if WINDOWS_UWP
using Windows.Storage;
#elif __ANDROID__
using Java.IO;
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

        public static Object Deserializer(System.IO.Stream stream, Type type)
        {
            using (System.IO.StreamReader sReader = new System.IO.StreamReader(stream))
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

        public static T Deserializer<T>(System.IO.Stream stream)
        {
            using (System.IO.StreamReader sReader = new System.IO.StreamReader(stream))
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

        public static async Task<Object> DeserializerAsync(System.IO.Stream stream, Type type)
        {
            return await Task.Run(() =>
            {
                using (System.IO.StreamReader sReader = new System.IO.StreamReader(stream))
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

        public static async Task<T> DeserializerAsync<T>(System.IO.Stream stream)
        {
            return await Task.Run(() =>
            {
                using (System.IO.StreamReader sReader = new System.IO.StreamReader(stream))
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
        public static async Task<T> DeserializerAsync<T>(File file)
#endif
        {
            return await Task.Run(async () =>
            {
                // Wait for file to be free
                while (FileUtils.IsFileLocked(file))
                {
                    await Task.Delay(100);
                }

                using (System.IO.FileStream fStream = new System.IO.FileStream(file.Path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                using (System.IO.StreamReader sReader = new System.IO.StreamReader(fStream))
                using (JsonReader reader = new JsonTextReader(sReader))
                {
                    JsonSerializer serializer = JsonSerializer.Create(DefaultSettings);
                    return serializer.Deserialize<T>(reader);
                }
            }).ConfigureAwait(false);
        }

#if WINDOWS_UWP
        public static void Serializer(Object obj, StorageFile file)
#elif __ANDROID__
        public static void Serializer(Object obj, File file)
#endif
        {
            Task.Run(async () =>
            {
                // Wait for file to be free
                while (FileUtils.IsFileLocked(file))
                {
                    await Task.Delay(100);
                }

                using (System.IO.FileStream fStream = new System.IO.FileStream(file.Path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
                using (System.IO.StreamWriter sWriter = new System.IO.StreamWriter(fStream))
                using (JsonTextWriter writer = new JsonTextWriter(sWriter))
                {
                    // Clear file before writing
                    fStream.SetLength(0);

                    JsonSerializer serializer = JsonSerializer.Create(DefaultSettings);
                    serializer.Serialize(writer, obj);
                    await writer.FlushAsync();
                }
            }).ConfigureAwait(false);
        }

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
