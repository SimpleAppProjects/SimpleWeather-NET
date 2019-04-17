using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Web;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace SimpleWeather.NWS
{
    public class NWSAlertProvider : IWeatherAlertProvider
    {
        public async Task<List<WeatherAlert>> GetAlerts(LocationData location)
        {
            List<WeatherAlert> alerts = null;

            string queryAPI = null;
            Uri queryURL = null;

            queryAPI = "https://api.weather.gov/alerts/active?point={0},{1}";
            queryURL = new Uri(string.Format(queryAPI, location.latitude, location.longitude));

            try
            {
                CancellationTokenSource cts = new CancellationTokenSource(Settings.READ_TIMEOUT);

                // Connect to webstream
                HttpClient webClient = new HttpClient();

                var version = string.Format("v{0}.{1}.{2}",
                    Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

                webClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/ld+json"));
                webClient.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue("SimpleWeather (thewizrd.dev@gmail.com)", version));

                HttpResponseMessage response = await webClient.GetAsync(queryURL).AsTask(cts.Token);
                response.EnsureSuccessStatusCode();
                Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
                // End Stream
                webClient.Dispose();

                // Load data
                alerts = new List<WeatherAlert>();

                var root = JSONParser.Deserializer<AlertRootobject>(contentStream);

                foreach (Graph result in root.graph)
                {
                    alerts.Add(new WeatherAlert(result));
                }

                // End Stream
                if (contentStream != null)
                    contentStream.Dispose();
            }
            catch (Exception ex)
            {
                alerts = new List<WeatherAlert>();
                Logger.WriteLine(LoggerLevel.Error, ex, "NWSAlertProvider: error getting weather alert data");
            }

            if (alerts == null)
                alerts = new List<WeatherAlert>();

            return alerts;
        }
    }
}
