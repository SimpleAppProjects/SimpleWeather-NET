using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using Windows.Web.Http;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.WeatherUnderground
{
    public static class AutoCompleteQuery
    {
        private static string queryAPI = "http://autocomplete.wunderground.com/aq?query=";
        private static string options = "&h=0&cities=1";

        public static async Task<List<AC_Location>> getLocations(string query)
        {
            Uri queryURL = new Uri(queryAPI + query + options);

            // Connect to webstream
            HttpClient webClient = new HttpClient();
            HttpResponseMessage response = await webClient.GetAsync(queryURL);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            byte[] buff = Encoding.UTF8.GetBytes(content);

            // Write array/buffer to memorystream
            MemoryStream memStream = new MemoryStream();
            memStream.Write(buff, 0, buff.Length);
            memStream.Seek(0, 0);

            // End Stream
            webClient.Dispose();

            // Load data
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(AC_Rootobject));
            List<AC_Location> locationResults = new List<AC_Location>();

            try
            {
                AC_Rootobject root = (AC_Rootobject)deserializer.ReadObject(memStream);

                foreach (AC_RESULT result in root.RESULTS)
                {
                    // Filter: only store city results
                    if (result.type != "city")
                        continue;

                    AC_Location location = new AC_Location(result);
                    locationResults.Add(location);
                }
            }
            catch (Exception e)
            {
                /* TODO: DEBUG - remove logging */
                System.Diagnostics.Debug.WriteLine(e.HResult + ": " + e.Message);
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            return locationResults;
        }
    }

    public class AC_Rootobject
    {
        public AC_RESULT[] RESULTS { get; set; }
    }

    public class AC_RESULT
    {
        public string name { get; set; }
        public string type { get; set; }
        public string c { get; set; }
        public string zmw { get; set; }
        public string tz { get; set; }
        public string tzs { get; set; }
        public string l { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
    }

    public class AC_Location
    {
        public string name;
        public string c { get; set; }
        public string zmw { get; set; }
        public string l { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }

        public AC_Location(AC_RESULT result)
        {
            name = result.name;
            c = result.c;
            zmw = result.zmw;
            l = result.l;
            lat = result.lat;
            lon = result.lon;
        }
    }
}