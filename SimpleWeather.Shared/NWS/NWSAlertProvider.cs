using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using static SimpleWeather.Utils.APIRequestUtils;

namespace SimpleWeather.NWS
{
    public partial class NWSAlertProvider : IWeatherAlertProvider, IRateLimitedRequest
    {
        private const String ALERT_QUERY_URL = "https://api.weather.gov/alerts/active?status=actual&message_type=alert&point={0:0.####},{1:0.####}";

        public long GetRetryTime() => 30000;

        public async Task<ICollection<WeatherAlert>> GetAlerts(LocationData location)
        {
            ICollection<WeatherAlert> alerts = null;

            try
            {
                CheckRateLimit(WeatherAPI.NWS);

                Uri queryURL = new Uri(string.Format(CultureInfo.InvariantCulture, ALERT_QUERY_URL, location.latitude, location.longitude));

                using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                {
                    request.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/ld+json"));

                    var version = string.Format("v{0}.{1}.{2}",
                        Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

                    request.Headers.UserAgent.Add(new HttpProductInfoHeaderValue("SimpleWeather (thewizrd.dev@gmail.com)", version));

                    // Connect to webstream
                    var webClient = SimpleLibrary.GetInstance().WebClient;
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                    {
                        await this.CheckForErrors(WeatherAPI.NWS, response);
                        response.EnsureSuccessStatusCode();

                        Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                        // Load data
                        var root = await JSONParser.DeserializerAsync<AlertRootobject>(contentStream);

                        alerts = CreateWeatherAlerts(root);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "NWSAlertProvider: error getting weather alert data");
            }

            if (alerts == null)
                alerts = new List<WeatherAlert>();

            return alerts;
        }
    }
}