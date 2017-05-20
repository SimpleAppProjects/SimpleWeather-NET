using Newtonsoft.Json;
using System;
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
            return JsonConvert.DeserializeObject(response, type);
        }

#if WINDOWS_UWP
        public static void Serializer(Object obj, StorageFile file)
#elif __ANDROID__
        public static void Serializer(Object obj, File file)
#endif
        {
            FileUtils.WriteFile(JsonConvert.SerializeObject(obj), file);
        }
    }
}
