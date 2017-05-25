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
        public static Object Deserializer(String response, Type type)
        {
            return JsonConvert.DeserializeObject(response, type,
                new JsonSerializerSettings()
                { TypeNameHandling = TypeNameHandling.All });
        }

        public static T Deserializer<T>(String response)
        {
            return JsonConvert.DeserializeObject<T>(response,
                new JsonSerializerSettings()
                { TypeNameHandling = TypeNameHandling.All });
        }

        public static async Task<Object> DeserializerAsync(String response, Type type)
        {
            return await Task.Run(() => 
            {
                return JsonConvert.DeserializeObject(response, type,
                    new JsonSerializerSettings()
                    { TypeNameHandling = TypeNameHandling.All });
            });
        }

        public static async Task<T> DeserializerAsync<T>(String response)
        {
            return await Task.Run(() =>
            {
                return JsonConvert.DeserializeObject<T>(response,
                    new JsonSerializerSettings()
                    { TypeNameHandling = TypeNameHandling.All });
            });
        }

        #if WINDOWS_UWP
        public static void Serializer(Object obj, StorageFile file)
#elif __ANDROID__
        public static void Serializer(Object obj, File file)
#endif
        {
            Task.Run(() => 
            {
                String data = JsonConvert.SerializeObject(obj,
                    new JsonSerializerSettings()
                    { TypeNameHandling = TypeNameHandling.All });
                FileUtils.WriteFile(data, file);
            });
        }

        public static string Serializer(Object obj, Type type)
        {
            return JsonConvert.SerializeObject(obj, type,
                new JsonSerializerSettings()
                { TypeNameHandling = TypeNameHandling.All });
        }

        public static async Task<string> SerializerAsync(Object obj, Type type)
        {
            return await Task.Run(() => 
            {
                return JsonConvert.SerializeObject(obj, type,
                    new JsonSerializerSettings()
                    { TypeNameHandling = TypeNameHandling.All });
            });
        }
    }
}
