using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using static SimpleWeather.Utils.APIRequestUtils;

namespace SimpleWeather.Ambee
{
    public class AmbeePollenProvider : IPollenProvider, IRateLimitedRequest
    {
        private const string QUERY_URL = "https://api.ambeedata.com/latest/pollen/by-lat-lng?lat={0:0.####}&lng={1:0.####}";

        public long GetRetryTime()
        {
            return 43200000L; // 12 hrs
        }

        public async Task<Pollen> GetPollenData(LocationData location)
        {
            Pollen pollenData = null;

            var key = DevSettingsEnabler.GetAPIKey(WeatherAPI.Ambee);
            if (String.IsNullOrWhiteSpace(key)) return null;

            try
            {
                CheckRateLimit(WeatherAPI.Ambee);

                Uri queryURL = new Uri(string.Format(CultureInfo.InvariantCulture, QUERY_URL, location.latitude, location.longitude));

                // Connect to webstream
                HttpClient webClient = SimpleLibrary.GetInstance().WebClient;
                var request = new HttpRequestMessage(HttpMethod.Get, queryURL);

                request.Headers.CacheControl.MaxAge = TimeSpan.FromHours(6);
                request.Headers.Add("x-api-key", key);

                using (request)
                using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                {
                    this.CheckForErrors(WeatherAPI.Ambee, response.StatusCode);
                    response.EnsureSuccessStatusCode();

                    using var contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                    // Load data
                    var root = await JSONParser.DeserializerAsync<Rootobject>(contentStream);

                    pollenData = new Pollen
                    {
                        treePollenCount = root.data[0].Risk.tree_pollen switch
                        {
                            "Low" => Pollen.PollenCount.Low,
                            "Moderate" => Pollen.PollenCount.Moderate,
                            "High" => Pollen.PollenCount.High,
                            "Very High" => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown,
                        },
                        grassPollenCount = root.data[0].Risk.grass_pollen switch
                        {
                            "Low" => Pollen.PollenCount.Low,
                            "Moderate" => Pollen.PollenCount.Moderate,
                            "High" => Pollen.PollenCount.High,
                            "Very High" => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown,
                        },
                        ragweedPollenCount = root.data[0].Risk.weed_pollen switch
                        {
                            "Low" => Pollen.PollenCount.Low,
                            "Moderate" => Pollen.PollenCount.Moderate,
                            "High" => Pollen.PollenCount.High,
                            "Very High" => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown,
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                pollenData = null;
                Logger.WriteLine(LoggerLevel.Error, ex, "AmbeePollenProvider: error getting pollen data");
            }

            return pollenData;
        }
    }
}
