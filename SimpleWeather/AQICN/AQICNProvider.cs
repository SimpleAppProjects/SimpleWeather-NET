using SimpleWeather.Keys;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace SimpleWeather.AQICN
{
    public class AQICNProvider : IAirQualityProvider
    {
        private const String QUERY_URL = "https://api.waqi.info/feed/geo:{0};{1}/?token={2}";
        public Task<AirQuality> GetAirQualityData(LocationData location)
        {
            return Task.Run(async () =>
            {
                AirQuality aqiData = null;

                string key = APIKeys.GetAQICNKey();
                if (String.IsNullOrWhiteSpace(key))
                    return null;

                Uri queryURL = new Uri(string.Format(QUERY_URL, location.latitude, location.longitude, key));

                try
                {
                    // Connect to webstream
                    HttpClient webClient = SimpleLibrary.WebClient;
                    var request = new HttpRequestMessage(HttpMethod.Get, queryURL);

                    using (request)
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                    {
                        response.EnsureSuccessStatusCode();
                        Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                        // Load data
                        var root = JSONParser.Deserializer<Rootobject>(contentStream);

                        aqiData = new AirQuality(root);
                    }
                }
                catch (Exception ex)
                {
                    aqiData = null;
                    Logger.WriteLine(LoggerLevel.Error, ex, "AQICNProvider: error getting air quality data");
                }

                return aqiData;
            });
        }
    }
}
