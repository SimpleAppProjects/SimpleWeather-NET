#if !__IOS__
using CacheCow.Client;
using CacheCow.Client.FileCacheStore;
using Firebase.Auth;

#if !WINUI
using SimpleWeather.Helpers;
#endif
using SimpleWeather.Utils;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using SimpleWeather.Firebase.RemoteConfig;
using NodaTime.TimeZones;

namespace SimpleWeather.Firebase
{
    public sealed partial class FirebaseRemoteConfig : IDisposable
    {
        private const string BASE_URL = "https://firebaseremoteconfig.googleapis.com";

        private readonly string ProjectId;
        private readonly string AppId;
        private readonly string ApiKey;
        private readonly Func<Task<User>> FirebaseUserFactory;

        internal FirebaseRemoteConfig(string projectId, string appId, string apiKey, Func<Task<User>> FirebaseUserFactory)
        {
            this.ProjectId = projectId;
            this.AppId = appId;
            this.ApiKey = apiKey;
            this.FirebaseUserFactory = FirebaseUserFactory;
        }

        private string GetFetchUrl()
        {
            return $"{BASE_URL}/v1/projects/{ProjectId}/namespaces/firebase:fetch?key={ApiKey}";
        }

        public Task<FetchResponse> GetRemoteConfig()
        {
            var webClient = httpClientLazy.Value;

            return Task.Run(async () =>
            {
                try
                {
                    using var request = new HttpRequestMessage(HttpMethod.Post, GetFetchUrl());

                    var user = await FirebaseUserFactory();
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.IfNoneMatch.Add(EntityTagHeaderValue.Any);

                    var requestContent = new Dictionary<string, object>()
                    {
                        { "app_instance_id", user.Uid },
                        { "app_instance_id_token", user.Credential.IdToken },
                        { "app_id", AppId },
                        { "country_code", LocaleUtils.GetDefault()?.TwoLetterISOLanguageName ?? "" },
                        { "language_code", LocaleUtils.GetDefault()?.Name ?? "" },
                        { "time_zone", this.RunCatching(() => TzdbDateTimeZoneSource.Default.GetSystemDefaultId()).GetOrDefault("") },
                        { "analytics_user_properties", FirebaseHelper.GetFirebaseAnalytics().GetUserProperties() }
                    };

                    var requestContentStr = JsonConvert.SerializeObject(requestContent);
                    request.Content = new StringContent(requestContentStr, Encoding.UTF8, "application/json");

                    using var cts = new CancellationTokenSource(10000);
                    using var response = await webClient.SendAsync(request, cts.Token);
                    response.EnsureSuccessStatusCode();

                    response.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                    using var txtReader = new JsonTextReader(new StreamReader(await response.Content.ReadAsStreamAsync()));
                    var responseContent = JsonSerializer.CreateDefault().Deserialize<FetchResponse>(txtReader);

                    return responseContent;
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e, "FirebaseRemoteConfig: Error retrieving config");
                    return null;
                }
            });
        }

        private readonly Lazy<HttpClient> httpClientLazy = new(() =>
        {
#if WINUI
            var cacheFolder = Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path;
            var CacheRoot = System.IO.Path.Combine(cacheFolder, "CacheCow");
#else
            var CacheRoot = System.IO.Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), "CacheCow");
#endif

            return ClientExtensions.CreateClient(new FileStore(CacheRoot) { MinExpiry = TimeSpan.FromDays(7) });
        });

        public void Dispose()
        {
            if (httpClientLazy.IsValueCreated)
            {
                httpClientLazy.Value.Dispose();
            }
        }
    }
}
#endif