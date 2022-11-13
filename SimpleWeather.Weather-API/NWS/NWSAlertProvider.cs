using SimpleWeather.HttpClientExtensions;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using static SimpleWeather.Weather_API.Utils.APIRequestUtils;

namespace SimpleWeather.Weather_API.NWS
{
    public partial class NWSAlertProvider : IWeatherAlertProvider, IRateLimitedRequest
    {
        private const String ALERT_QUERY_URL = "https://api.weather.gov/alerts/active?status=actual&message_type=alert&point={0:0.####},{1:0.####}";

        public long GetRetryTime() => 30000;

        public async Task<ICollection<WeatherAlert>> GetAlerts(SimpleWeather.LocationData.LocationData location)
        {
            ICollection<WeatherAlert> alerts = null;

            try
            {
                CheckRateLimit(WeatherAPI.NWS);

                Uri queryURL = new Uri(string.Format(CultureInfo.InvariantCulture, ALERT_QUERY_URL, location.latitude, location.longitude));

                using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/ld+json"));
                    request.Headers.UserAgent.AddAppUserAgent();

                    // Connect to webstream
                    var webClient = SharedModule.Instance.WebClient;
                    using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                    using (var response = await webClient.SendAsync(request, cts.Token))
                    {
                        await this.CheckForErrors(WeatherAPI.NWS, response);
                        response.EnsureSuccessStatusCode();

                        Stream contentStream = await response.Content.ReadAsStreamAsync();

                        // Load data
                        var root = await JSONParser.DeserializerAsync<AlertRootobject>(contentStream);

                        alerts = this.CreateWeatherAlerts(root);
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