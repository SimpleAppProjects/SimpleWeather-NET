using SimpleWeather.Keys;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace SimpleWeather.TZDB
{
    internal class TimeZoneData
    {
        [JsonPropertyName("tz_long")]
        public String TZLong { get; set; }
    }

    public class TimeZoneProvider : ITimeZoneProvider
    {
        public Task<string> GetTimeZone(double latitude, double longitude)
        {
            return Task.Run(async () =>
            {
                String tzLong = null;

                try
                {
                    // Get Firebase token
                    var authLink = await Firebase.FirebaseAuthHelper.GetAuthLink();
                    string userToken = authLink.FirebaseToken;
                    string tzAPI = APIKeys.GetTimeZoneAPI();
                    if (String.IsNullOrWhiteSpace(tzAPI) || String.IsNullOrWhiteSpace(userToken))
                        return null;

                    Uri queryURL = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}?lat={1}&lon={2}", tzAPI, latitude, longitude));

                    using (var request = new HttpRequestMessage(HttpMethod.Get, queryURL))
                    {
                        // Add headers to request
                        request.Headers.Authorization = new HttpCredentialsHeaderValue("Bearer", userToken);

                        // Connect to webstream
                        var webClient = SimpleLibrary.WebClient;
                        using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                        using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                        {
                            response.EnsureSuccessStatusCode();
                            Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                            // Load weather
                            var root = await JsonSerializer.DeserializeAsync<TimeZoneData>(contentStream);

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
            });
        }
    }
}
