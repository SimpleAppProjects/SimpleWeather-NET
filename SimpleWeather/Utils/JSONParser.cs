using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.Utils
{
    public static class JSONParser
    {
        public static Object Deserializer(String response, Type type)
        {
            Object obj = null;

            using (MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(response)))
            {
                memStream.Seek(0, 0);

                DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(type);
                obj = deSerializer.ReadObject(memStream);
            }

            return obj;
        }

        public static void Serializer(Object obj, StorageFile file)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(memStream, obj);
                FileUtils.WriteFile(memStream.ToArray(), file);
                memStream.Dispose();
            }
        }
    }
}
