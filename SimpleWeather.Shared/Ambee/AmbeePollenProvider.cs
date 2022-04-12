using SimpleWeather.Keys;
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
using System.Net.Http;
using System.Net.Sockets;
using static SimpleWeather.Utils.APIRequestUtils;
using System.Net.Http.Headers;

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

            var key = Settings.APIKeys[WeatherAPI.Ambee] ?? APIKeys.GetAmbeeKey();
            if (String.IsNullOrWhiteSpace(key)) return null;

            try
            {
                CheckRateLimit(WeatherAPI.Ambee);

                Uri queryURL = new Uri(string.Format(CultureInfo.InvariantCulture, QUERY_URL, location.latitude, location.longitude));

                // Connect to webstream
                HttpClient webClient = SimpleLibrary.GetInstance().WebClient;
                var request = new HttpRequestMessage(HttpMethod.Get, queryURL);

                request.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    MaxAge = TimeSpan.FromHours(6)
                };
                request.Headers.Add("x-api-key", key);

                using (request)
                using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                using (var response = await webClient.SendAsync(request, cts.Token))
                {
                    await this.CheckForErrors(WeatherAPI.Ambee, response);
                    response.EnsureSuccessStatusCode();

                    using var contentStream = await response.Content.ReadAsStreamAsync();

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
