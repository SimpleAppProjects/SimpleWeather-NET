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
using static SimpleWeather.Utils.APIRequestUtils;

namespace SimpleWeather.AQICN
{
    public sealed class AQICNProvider : IAirQualityProvider, IRateLimitedRequest
    {
        private const String QUERY_URL = "https://api.waqi.info/feed/geo:{0:0.####};{1:0.####}/?token={2}";

        private const string API_ID = "waqi";

        public long GetRetryTime() => 1000;

        public async Task<AirQuality> GetAirQualityData(LocationData location)
        {
            AQICNData aqiData = null;

            string key = APIKeys.GetAQICNKey();
            if (String.IsNullOrWhiteSpace(key))
                return null;

            try
            {
                CheckRateLimit(API_ID);

                Uri queryURL = new Uri(string.Format(CultureInfo.InvariantCulture, QUERY_URL, location.latitude, location.longitude, key));

                // Connect to webstream
                HttpClient webClient = SimpleLibrary.GetInstance().WebClient;
                var request = new HttpRequestMessage(HttpMethod.Get, queryURL);

                var version = string.Format("v{0}.{1}.{2}",
                    Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

                request.Headers.UserAgent.Add(new HttpProductInfoHeaderValue("SimpleWeather (thewizrd.dev@gmail.com)", version));
                request.Headers.CacheControl.MaxAge = TimeSpan.FromHours(1);

                using (request)
                using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                {
                    this.CheckForErrors(API_ID, response.StatusCode);
                    response.EnsureSuccessStatusCode();

                    Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                    // Load data
                    var root = await JSONParser.DeserializerAsync<Rootobject>(contentStream);

                    aqiData = new AQICNData(root);
                }
            }
            catch (Exception ex)
            {
                aqiData = null;
                Logger.WriteLine(LoggerLevel.Error, ex, "AQICNProvider: error getting air quality data");
            }

            return aqiData;
        }
    }
}
