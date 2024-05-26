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
#if false
        private const string BASE_URL = "https://www.google-analytics.com/debug/mp/collect";
#else
        private const string BASE_URL = "https://www.google-analytics.com/mp/collect";
#endif

        private readonly string MeasurementID;
        private readonly string ApiSecret;

        private readonly IDictionary<string, string> UserProperties = new Dictionary<string, string>();
        private readonly Func<Task<User>> FirebaseUserFactory;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(10, 10);

        internal FirebaseAnalytics(string measurementId, string apiSecret, Func<Task<User>> FirebaseUserFactory)
        {
            this.FirebaseUserFactory = FirebaseUserFactory;
            this.MeasurementID = measurementId;
            this.ApiSecret = apiSecret;
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
                        _params = (properties?.ToDictionary(entry => entry.Key, entry => entry.Value as object) ?? [])?.Let(d =>
                        {
#if DEBUG
                            d.Add("debug_mode", true);
#endif
                            d.Add("engagement_time_msec", 1000);
                            return d;
                        }),
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
                    .AppendQueryParameter("measurement_id", MeasurementID)
                    .AppendQueryParameter("api_secret", ApiSecret)
                    .BuildUri();

                var user = await FirebaseUserFactory();
                using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

                var requestContent = new Dictionary<string, object>()
                {
                    { "client_id", "SimpleWeather.NET" },
                    { "user_id", user.Uid },
                    { "events",  ImmutableList.Create(@event) },
                    { "user_properties", GetAnalyticsUserProperties() }
                };

                var requestContentStr = JsonConvert.SerializeObject(requestContent);
                request.Content = new StringContent(requestContentStr, Encoding.UTF8, "application/json");

                using var webClient = new HttpClient();
                using var response = await webClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

#if false
                var stream = await response.Content.ReadAsStreamAsync();

                var validationRoot = JSONParser.Deserializer<ValidationRootobject>(stream);

                if (validationRoot.validationMessages.Length > 0)
                {
                    validationRoot.validationMessages.ForEach(msg =>
                    {
                        Logger.WriteLine(LoggerLevel.Warn, $"FirebaseAnalytics: validation error | code: {msg.validationCode} | path: {msg.fieldPath} | desc: {msg.description}");
                    });
                }
#endif
            }
            catch
            {
                // ignore errors
            }
        }

        private object GetAnalyticsUserProperties()
        {
            var userProperties = UserProperties.ToImmutableDictionary();

            if (userProperties.Count <= 0)
            {
                return new object();
            }

            return userProperties.ToDictionary(entry => entry.Key, entry => new PropertyValue() { value = entry.Value });
        }

        internal IDictionary<string, string> GetUserProperties()
        {
            return UserProperties.ToImmutableDictionary();
        }

        private async Task ReleaseSemaphoreAfterDelayAsync()
        {
            await Task.Delay(60000);
            _semaphore.Release();
        }
    }
}
