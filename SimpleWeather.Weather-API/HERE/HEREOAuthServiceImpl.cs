using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
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

namespace SimpleWeather.Weather_API.HERE
{
    public sealed class HEREOAuthServiceImpl : IHEREOAuthService
    {
        public const string HERE_OAUTH_URL = "https://account.api.here.com/oauth2/token";
        private const string KEY_TOKEN = "token";

        public async Task<string> GetBearerToken(bool forceRefresh = false)
        {
            if (!forceRefresh)
            {
                var token = await GetTokenFromStorage();
                if (!String.IsNullOrWhiteSpace(token))
                    return token;
                else
                    forceRefresh = true;
            }

            if (forceRefresh)
            {
                try
                {
                    CheckRateLimit(WeatherAPI.Here);

                    var oAuthRequest = new OAuthRequest(APIKeys.GetHERECliID(), APIKeys.GetHERECliSecr(), OAuthSignatureMethod.HMAC_SHA256, HTTPRequestType.POST);

                    using var request = new HttpRequestMessage(HttpMethod.Post, HERE_OAUTH_URL);
                    // Add headers to request
                    var authHeader = oAuthRequest.GetAuthorizationHeader(request.RequestUri, true);
                    request.Headers.Add("Authorization", authHeader);
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        NoCache = true
                    };

                    // Connect to webstream
                    var contentList = new List<KeyValuePair<string, string>>(1)
                    {
                        new KeyValuePair<string, string>("grant_type", "client_credentials")
                    };
                    request.Content = new FormUrlEncodedContent(contentList);

                    var webClient = SharedModule.Instance.WebClient;
                    using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                    using var response = await webClient.SendAsync(request, cts.Token);
                    await response.CheckForErrors(WeatherAPI.Here, 10000);
                    response.EnsureSuccessStatusCode();

                    Stream contentStream = await response.Content.ReadAsStreamAsync();

                    var date = response.Headers.Date.GetValueOrDefault(DateTimeOffset.UtcNow);

                    var tokenRoot = await JSONParser.DeserializerAsync<TokenRootobject>(contentStream);

                    if (tokenRoot != null)
                    {
                        var tokenStr = String.Format(CultureInfo.InvariantCulture, "Bearer {0}", tokenRoot.access_token);

                        // Store token for future operations
                        var token = new Token()
                        {
                            expiration_date = date.UtcDateTime.AddSeconds(tokenRoot.expires_in),
                            access_token = tokenStr
                        };

                        StoreToken(token);

                        return tokenStr;
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e, "HEREOAuthUtils: Error retrieving token");
                }
            }

            return null;
        }

        private async Task<String> GetTokenFromStorage()
        {
            // Shared Settings
            var HERESettingsContainer = new SettingsContainer(WeatherAPI.Here);

            if (HERESettingsContainer.ContainsKey(KEY_TOKEN))
            {
                var tokenJSON = HERESettingsContainer.GetValue<string>(KEY_TOKEN);
                if (tokenJSON != null)
                {
                    var token = await JSONParser.DeserializerAsync<Token>(tokenJSON);

                    // Add buffer before expiration to avoid any auth issues
                    if (token != null && token.expiration_date.AddMinutes(-1.5) > DateTime.UtcNow)
                    {
                        return token.access_token;
                    }
                }
            }

            return null;
        }

        private void StoreToken(Token token)
        {
            // Shared Settings
            var HERESettingsContainer = new SettingsContainer(WeatherAPI.Here);

            Task.Run(() =>
            {
                HERESettingsContainer.SetValue(KEY_TOKEN, JSONParser.Serializer(token));
            });
        }
    }
}