using SimpleWeather.Firebase;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Json;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.Weather_API.Utils;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static SimpleWeather.Weather_API.Utils.APIRequestUtils;

namespace SimpleWeather.Weather_API.TZDB
{
    public class TimeZoneProviderImpl : ITimeZoneProvider, IRateLimitedRequest
    {
        private const string API_ID = "tzdb";

        public long GetRetryTime() => 60000;

        public async Task<string> GetTimeZone(double latitude, double longitude)
        {
            string tzLong = null;

            try
            {
                // Get Firebase token
                var userToken = await FirebaseHelper.GetAccessToken();

                string tzAPI = APIKeys.GetTimeZoneAPI();
                if (string.IsNullOrWhiteSpace(tzAPI) || string.IsNullOrWhiteSpace(userToken))
                    return null;

                CheckRateLimit(API_ID);

                Uri queryURL = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}?lat={1:0.####}&lon={2:0.####}", tzAPI, latitude, longitude));

                using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                {
                    // Add headers to request
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(1)
                    };

                    // Connect to webstream
                    var webClient = SharedModule.Instance.WebClient;
                    using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                    using (var response = await webClient.SendAsync(request, cts.Token))
                    {
                        await CheckForErrors(API_ID, response);
                        response.EnsureSuccessStatusCode();

                        Stream contentStream = await response.Content.ReadAsStreamAsync();

                        // Load weather
                        var root = await JsonSerializer.DeserializeAsync(contentStream, TZDBJsonContext.Default.TimeZoneData);

                        tzLong = root.TZLong;
                    }
                }
            }
            catch (Exception ex)
            {
                tzLong = null;
                Logger.WriteLine(LoggerLevel.Error, ex, "TimeZoneProvider: error getting time zone data");
            }

            return tzLong;
        }
    }
}
