using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using Windows.Web.Http;
using System.Collections.Generic;
using System.Text;
using SimpleWeather.Utils;

namespace SimpleWeather.WeatherUnderground
{
    public static class AutoCompleteQuery
    {
        public static async Task<List<Controls.LocationQueryView>> getLocations(string query)
        {
            string queryAPI = "http://autocomplete.wunderground.com/aq?query=";
            string options = "&h=0&cities=1";
            Uri queryURL = new Uri(queryAPI + query + options);
            List<Controls.LocationQueryView> locationResults = null;
            // Limit amount of results shown
            int maxResults = 10;

            try
            {
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
                locationResults = new List<Controls.LocationQueryView>();

                AC_Rootobject root = (AC_Rootobject)deserializer.ReadObject(memStream);

                foreach (AC_RESULT result in root.RESULTS)
                {
                    // Filter: only store city results
                    if (result.type != "city")
                        continue;

                    locationResults.Add(new Controls.LocationQueryView(result));

                    // Limit amount of results
                    maxResults--;
                    if (maxResults <= 0)
                        break;
                }
            }
            catch (Exception)
            {
                locationResults = new List<Controls.LocationQueryView>();
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
}