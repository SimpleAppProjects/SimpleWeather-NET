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
using Windows.ApplicationModel;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace SimpleWeather.AQICN
{
    public class AQICNProvider : IAirQualityProvider
    {
        private const String QUERY_URL = "https://api.waqi.info/feed/geo:{0:0.####};{1:0.####}/?token={2}";
        private const int MAX_ATTEMPTS = 2;

        public Task<AirQuality> GetAirQualityData(LocationData location)
        {
            return Task.Run(async () =>
            {
                AirQuality aqiData = null;

                string key = APIKeys.GetAQICNKey();
                if (String.IsNullOrWhiteSpace(key))
                    return null;

                try
                {
                    Uri queryURL = new Uri(string.Format(CultureInfo.InvariantCulture, QUERY_URL, location.latitude, location.longitude, key));

                    for (int i = 0; i < MAX_ATTEMPTS; i++)
                    {
                        // Connect to webstream
                        HttpClient webClient = SimpleLibrary.GetInstance().WebClient;
                        var request = new HttpRequestMessage(HttpMethod.Get, queryURL);

                        request.Headers.CacheControl.MaxAge = TimeSpan.FromHours(1);

                        using (request)
                        using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                        using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                        {
                            if (response.StatusCode == HttpStatusCode.BadRequest)
                            {
                                break;
                            }
                            else
                            {
                                response.EnsureSuccessStatusCode();
                                Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                                // Load data
                                var root = JSONParser.Deserializer<Rootobject>(contentStream);

                                aqiData = new AirQuality(root);
                            }
                        }

                        if (i < MAX_ATTEMPTS - 1 && aqiData == null)
                        {
                            await Task.Delay(1000);
                        }
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
