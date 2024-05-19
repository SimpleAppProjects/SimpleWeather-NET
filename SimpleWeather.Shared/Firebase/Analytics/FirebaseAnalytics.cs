using Firebase.Auth;
using Newtonsoft.Json;
using SimpleWeather.Firebase.Analytics;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.Firebase
{
    public sealed class FirebaseAnalytics
    {
        private const string BASE_URL = "https://www.google-analytics.com/mp/collect";

        private readonly string AppId;
        private readonly string ApiSecret;
        private readonly string AppInstanceId;

        private readonly IDictionary<string, string> UserProperties = new Dictionary<string, string>();
        private readonly Func<Task<User>> FirebaseUserFactory;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(10, 10);

        internal FirebaseAnalytics(string appId, string apiSecret, string instanceId, Func<Task<User>> FirebaseUserFactory)
        {
            this.FirebaseUserFactory = FirebaseUserFactory;
            this.AppId = appId;
            this.ApiSecret = apiSecret;
            this.AppInstanceId = instanceId;
        }

        public void SetUserProperty([MaxLength(24)] string key, [MaxLength(36)] string value)
        {
            UserProperties[key] = value;
        }

        public void SetUserProperty([MaxLength(24)] string key, bool value)
        {
            UserProperties[key] = value.ToString();
        }

        public void LogEvent([MaxLength(40)] string eventName, IDictionary<string, string> properties = null)
        {
            QueueEvent(eventName, properties);
        }

        private void QueueEvent(string eventName, IDictionary<string, string> properties = null)
        {
            Task.Run(async () =>
            {
                await _semaphore.WaitAsync();

                try
                {
                    await PostEvent(new Event()
                    {
                        name = eventName,
                        _params = properties
                    });
                }
                finally
                {
                    await ReleaseSemaphoreAfterDelayAsync();
                }
            });
        }

        private async Task PostEvent(Event @event)
        {
            try
            {
                var requestUri = BASE_URL.ToUriBuilderEx()
                    .AppendQueryParameter("firebase_app_id", AppId)
                    .AppendQueryParameter("api_secret", ApiSecret)
                    .BuildUri();

                var user = await FirebaseUserFactory();
                using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

                var requestContent = new Dictionary<string, string>()
                {
                    { "app_instance_id", AppInstanceId },
                    { "user_id", user.Uid },
                    { "events",  JsonConvert.SerializeObject(ImmutableList.Create(@event)) },
                    { "user_properties", GetUserPropertiesJson() }
                };

                request.Content = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");

                using var webClient = new HttpClient();
                using var response = await webClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                // ignore errors
            }
        }

        private string GetUserPropertiesJson()
        {
            var userProperties = UserProperties.ToImmutableDictionary();

            if (userProperties.Count <= 0)
            {
                return "{}";
            }

            return JsonConvert.SerializeObject(userProperties.ToDictionary(entry => entry.Key, entry => new PropertyValue() { value = entry.Value }));
        }

        private async Task ReleaseSemaphoreAfterDelayAsync()
        {
            await Task.Delay(60000);
            _semaphore.Release();
        }
    }
}
