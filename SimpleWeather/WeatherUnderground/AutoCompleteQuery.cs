using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
#if WINDOWS_UWP
using Windows.Storage.Streams;
using Windows.Web.Http;
#elif __ANDROID__
using System.Net.Http;
#endif

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

                // End Stream
                webClient.Dispose();

                // Load data
                locationResults = new List<Controls.LocationQueryView>();

                AC_Rootobject root = (AC_Rootobject)JsonConvert.DeserializeObject(content, typeof(AC_Rootobject));

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