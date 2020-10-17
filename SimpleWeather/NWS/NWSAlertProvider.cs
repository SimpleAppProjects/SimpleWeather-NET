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

namespace SimpleWeather.NWS
{
    public class NWSAlertProvider : IWeatherAlertProvider
    {
        private const String ALERT_QUERY_URL = "https://api.weather.gov/alerts/active?status=actual&message_type=alert&point={0:0.####},{1:0.####}";

        public Task<ICollection<WeatherAlert>> GetAlerts(LocationData location)
        {
            return Task.Run<ICollection<WeatherAlert>>(async () =>
            {
                List<WeatherAlert> alerts = null;

                try
                {
                    Uri queryURL = new Uri(string.Format(CultureInfo.InvariantCulture, ALERT_QUERY_URL, location.latitude, location.longitude));
                    using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                    {
                        request.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/ld+json"));

                        // Connect to webstream
                        var webClient = SimpleLibrary.WebClient;
                        using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                        using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                        {
                            response.EnsureSuccessStatusCode();
                            Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                            // Load data
                            var root = JSONParser.Deserializer<AlertRootobject>(contentStream);

                            alerts = new List<WeatherAlert>(root.graph.Length);

                            foreach (AlertGraph result in root.graph)
                            {
                                alerts.Add(new WeatherAlert(result));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    alerts = new List<WeatherAlert>();
                    Logger.WriteLine(LoggerLevel.Error, ex, "NWSAlertProvider: error getting weather alert data");
                }

                if (alerts == null)
                    alerts = new List<WeatherAlert>();

                return alerts;
            });
        }
    }
}