using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS_UWP
using Windows.ApplicationModel;
using Windows.Web;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
#elif __ANDROID__
using Android.App;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
#endif

namespace SimpleWeather.NWS
{
    public class NWSAlertProvider : IWeatherAlertProvider
    {
        public async Task<List<WeatherAlert>> GetAlerts(LocationData location)
        {
            List<WeatherAlert> alerts = null;

            string queryAPI = null;
            Uri weatherURL = null;

            queryAPI = "https://api.weather.gov/alerts/active?point={0},{1}";
            weatherURL = new Uri(string.Format(queryAPI, location.latitude, location.longitude));

            try
            {
                // Connect to webstream
                HttpClient webClient = new HttpClient();
#if WINDOWS_UWP
                var version = string.Format("v{0}.{1}.{2}",
                    Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

                webClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/ld+json"));
                webClient.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue("SimpleWeather (thewizrd.dev@gmail.com)", version));
#elif __ANDROID__
                var packageInfo = Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, 0);
                var version = string.Format("v{0}", packageInfo.VersionName);

                webClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/ld+json"));
                webClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", String.Format("SimpleWeather (thewizrd.dev@gmail.com) {0}", version));
#endif
                HttpResponseMessage response = await webClient.GetAsync(weatherURL);
                response.EnsureSuccessStatusCode();
                Stream contentStream = null;
#if WINDOWS_UWP
                contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
#elif __ANDROID__
                contentStream = await response.Content.ReadAsStreamAsync();
#endif
                // End Stream
                webClient.Dispose();

                // Load data
                alerts = new List<WeatherAlert>();

                AlertRootobject root = JSONParser.Deserializer<AlertRootobject>(contentStream);

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
                Debug.WriteLine(ex.StackTrace);
            }

            if (alerts == null)
                alerts = new List<WeatherAlert>();

            return alerts;
        }
    }
}
