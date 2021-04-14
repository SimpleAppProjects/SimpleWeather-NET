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
        private const int MAX_ATTEMPTS = 2;

        public async Task<ICollection<WeatherAlert>> GetAlerts(LocationData location)
        {
            List<WeatherAlert> alerts = null;

            try
            {
                Uri queryURL = new Uri(string.Format(CultureInfo.InvariantCulture, ALERT_QUERY_URL, location.latitude, location.longitude));

                for (int i = 0; i < MAX_ATTEMPTS; i++)
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                    {
                        request.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/ld+json"));

                        try
                        {
                            // Connect to webstream
                            var webClient = SimpleLibrary.GetInstance().WebClient;
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
                                    var root = await JSONParser.DeserializerAsync<AlertRootobject>(contentStream);

                                    alerts = new List<WeatherAlert>(root.graph.Length);

                                    foreach (AlertGraph result in root.graph)
                                    {
                                        alerts.Add(new WeatherAlert(result));
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    if (i < MAX_ATTEMPTS - 1 && alerts == null)
                    {
                        await Task.Delay(1000);
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