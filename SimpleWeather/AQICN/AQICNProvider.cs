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
        public Task<AirQuality> GetAirQualityData(LocationData location)
        {
            return Task.Run(async () =>
            {
                AirQuality aqiData = null;

                string queryAPI = null;
                Uri queryURL = null;

                string key = APIKeys.GetAQICNKey();
                if (String.IsNullOrWhiteSpace(key))
                    return null;

                queryAPI = "https://api.waqi.info/feed/geo:{0};{1}/?token={2}";
                queryURL = new Uri(string.Format(queryAPI, location.latitude, location.longitude, key));

                try
                {
                    CancellationTokenSource cts = new CancellationTokenSource(Settings.READ_TIMEOUT);

                    // Connect to webstream
                    HttpClient webClient = new HttpClient();

                    var version = string.Format("v{0}.{1}.{2}",
                        Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

                    webClient.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue("SimpleWeather (thewizrd.dev@gmail.com)", version));

                    HttpResponseMessage response = await AsyncTask.RunAsync(webClient.GetAsync(queryURL).AsTask(cts.Token));
                    response.EnsureSuccessStatusCode();
                    Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
                    // End Stream
                    webClient.Dispose();
                    cts.Dispose();

                    // Load data
                    var root = JSONParser.Deserializer<Rootobject>(contentStream);

                    aqiData = new AirQuality(root);

                    // End Stream
                    if (contentStream != null)
                        contentStream.Dispose();
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
