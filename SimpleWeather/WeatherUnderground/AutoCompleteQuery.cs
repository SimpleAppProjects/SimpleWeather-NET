using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using SimpleWeather.Utils;
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
        public static async Task<List<Controls.LocationQueryViewModel>> GetLocations(string query)
        {
            string queryAPI = "http://autocomplete.wunderground.com/aq?query=";
            string options = "&h=0&cities=1";
            Uri queryURL = new Uri(queryAPI + query + options);
            List<Controls.LocationQueryViewModel> locationResults = null;
            // Limit amount of results shown
            int maxResults = 10;

            try
            {
                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(queryURL);
                response.EnsureSuccessStatusCode();
                System.IO.Stream contentStream = null;
#if WINDOWS_UWP
                contentStream = System.IO.WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
#elif __ANDROID__
                contentStream = await response.Content.ReadAsStreamAsync();
#endif
                // End Stream
                webClient.Dispose();

                // Load data
                locationResults = new List<Controls.LocationQueryViewModel>();

                AC_Rootobject root = JSONParser.Deserializer<AC_Rootobject>(contentStream);

                foreach (AC_RESULT result in root.RESULTS)
                {
                    // Filter: only store city results
                    if (result.type != "city")
                        continue;

                    locationResults.Add(new Controls.LocationQueryViewModel(result));

                    // Limit amount of results
                    maxResults--;
                    if (maxResults <= 0)
                        break;
                }

                // End Stream
                contentStream.Dispose();
            }
            catch (Exception ex)
            {
                locationResults = new List<Controls.LocationQueryViewModel>();
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
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