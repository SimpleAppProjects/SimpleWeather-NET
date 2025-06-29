#if __IOS__
using Microsoft.Maui.ApplicationModel;
#endif
using SimpleWeather.HttpClientExtensions;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using static SimpleWeather.Weather_API.Utils.APIRequestUtils;

namespace SimpleWeather.Weather_API.Google
{
    public class GooglePollenProvider : IPollenProvider, IRateLimitedRequest
    {
        private const string QUERY_URL = "https://pollen.googleapis.com/v1/forecast:lookup?location.latitude={0:0.####}&location.longitude={1:0.####}&days=1&languageCode=en&plantsDescription=0&key={2}";
        private const string API_ID = WeatherAPI.Google;

        public long GetRetryTime() => 60000; // 1 min

        public async Task<Pollen> GetPollenData(SimpleWeather.LocationData.LocationData location)
        {
            Pollen pollenData = null;

            var key = DI.Utils.SettingsManager.APIKeys[WeatherAPI.Google_Pollen] ?? APIKeys.GetGPollenKey();

            try
            {
                CheckRateLimit(API_ID);

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                }

                Uri queryURL = new Uri(string.Format(CultureInfo.InvariantCulture, QUERY_URL, location.latitude, location.longitude, key));

                // Connect to webstream
                HttpClient webClient = SharedModule.Instance.WebClient;
                var request = new HttpRequestMessage(HttpMethod.Get, queryURL);

#if __IOS__
                request.Headers.Add("X-Ios-Bundle-Identifier", AppInfo.PackageName);
#endif
                request.Headers.UserAgent.AddAppUserAgent();
                request.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    MaxAge = TimeSpan.FromHours(12)
                };

                using (request)
                using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                using (var response = await webClient.SendAsync(request, cts.Token))
                {
                    await this.CheckForErrors(API_ID, response);
                    response.EnsureSuccessStatusCode();

                    Stream contentStream = await response.Content.ReadAsStreamAsync();

                    // Load data
                    var root = await JSONParser.DeserializerAsync<PollenRootobject>(contentStream);

                    root?.Let(it =>
                    {
                        var pollenTypeInfo = it.dailyInfo?.FirstOrDefault()?.pollenTypeInfo;

                        if (pollenTypeInfo is not null)
                        {
                            pollenData = new Pollen().Apply(p =>
                            {
                                foreach (var t in pollenTypeInfo)
                                {
                                    switch (t?.code)
                                    {
                                        case "TREE":
                                            p.treePollenCount = ToPollenCount(t.indexInfo);
                                            break;
                                        case "GRASS":
                                            p.grassPollenCount = ToPollenCount(t.indexInfo);
                                            break;
                                        case "WEED":
                                            p.ragweedPollenCount = ToPollenCount(t.indexInfo);
                                            break;
                                    }
                                }

                                p.attribution = "Google";
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                pollenData = null;
                Logger.WriteLine(LoggerLevel.Error, ex, "GooglePollenProvider: error getting pollen data");
            }

            return pollenData;
        }

        private static Pollen.PollenCount ToPollenCount(Indexinfo indexinfo)
        {
            return indexinfo?.value switch
            {
                1 or 2 => Pollen.PollenCount.Low,
                3 => Pollen.PollenCount.Moderate,
                4 => Pollen.PollenCount.High,
                5 => Pollen.PollenCount.VeryHigh,
                _ => Pollen.PollenCount.Unknown
            };
        }
    }
}
