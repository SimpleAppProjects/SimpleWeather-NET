using SimpleWeather.Location;
using SimpleWeather.WeatherData;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static partial class APIRequestUtils
    {
        #region System.Net.Http
        /// <summary>
        /// Checks if response was successful; if it was not, throw the appropriate WeatherException
        /// </summary>
        /// <param name="apiID">API ID of API where the request came from</param>
        /// <param name="response">HTTP response</param>
        /// <param name="retryTimeInMs">Time in milliseconds to wait until next request</param>
        /// <exception cref="WeatherException">Error status will correspond to specific error status</exception>
        public static async Task CheckForErrors(this System.Net.Http.HttpResponseMessage response, string apiID, long retryTimeInMs = 60000)
        {
            await CheckForErrors(apiID, response, retryTimeInMs);
        }

        /// <summary>
        /// Checks if response was successful; if it was not, throw the appropriate WeatherException
        /// </summary>
        /// <param name="apiID">API ID of API where the request came from</param>
        /// <param name="response">HTTP response</param>
        /// <param name="retryTimeInMs">Time in milliseconds to wait until next request</param>
        /// <exception cref="WeatherException">Error status will correspond to specific error status</exception>
        public static async Task CheckForErrors(string apiID, System.Net.Http.HttpResponseMessage response, long retryTimeInMs = 60000)
        {
            if (!response.IsSuccessStatusCode)
            {
                switch ((int)response.StatusCode)
                {
                    case (int)System.Net.HttpStatusCode.OK:
                        // ok
                        break;
                    case (int)System.Net.HttpStatusCode.BadRequest:
                        throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather,
                            await response.CreateException());
                    case (int)System.Net.HttpStatusCode.NotFound:
                        throw new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound,
                            await response.CreateException());
                    case 429:
                        await ThrowIfRateLimited(apiID, response, retryTimeInMs);
                        break;
                    case (int)System.Net.HttpStatusCode.InternalServerError:
                        throw new WeatherException(WeatherUtils.ErrorStatus.Unknown,
                            await response.CreateException());
                    default:
                        throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather,
                            await response.CreateException());
                }
            }
        }

        /// <summary>
        /// Checks if response was successful; if it was not, throw the appropriate WeatherException
        /// </summary>
        /// <param name="response">HTTP response</param>
        /// <exception cref="WeatherException">Error status will correspond to specific error status</exception>
        public static async Task CheckForErrors(this WeatherProviderImpl providerImpl, System.Net.Http.HttpResponseMessage response)
        {
            await CheckForErrors(providerImpl.WeatherAPI, response, providerImpl.GetRetryTime());
        }

        /// <summary>
        /// Checks if response was successful; if it was not, throw the appropriate WeatherException
        /// </summary>
        /// <param name="response">HTTP response</param>
        /// <exception cref="WeatherException">Error status will correspond to specific error status</exception>
        public static async Task CheckForErrors(this System.Net.Http.HttpResponseMessage response, WeatherProviderImpl providerImpl)
        {
            await CheckForErrors(providerImpl.WeatherAPI, response, providerImpl.GetRetryTime());
        }

        /// <summary>
        /// Checks if response was successful; if it was not, throw the appropriate WeatherException
        /// </summary>
        /// <param name="response">HTTP response</param>
        /// <exception cref="WeatherException">Error status will correspond to specific error status</exception>
        public static async Task CheckForErrors(this LocationProviderImpl providerImpl, System.Net.Http.HttpResponseMessage response)
        {
            await CheckForErrors(providerImpl.LocationAPI, response, providerImpl.GetRetryTime());
        }

        /// <summary>
        /// Checks if response was successful; if it was not, throw the appropriate WeatherException
        /// </summary>
        /// <param name="response">HTTP response</param>
        /// <exception cref="WeatherException">Error status will correspond to specific error status</exception>
        public static async Task CheckForErrors(this System.Net.Http.HttpResponseMessage response, LocationProviderImpl providerImpl)
        {
            await CheckForErrors(providerImpl.LocationAPI, response, providerImpl.GetRetryTime());
        }

        /// <summary>
        /// Checks if response was successful; if it was not, throw the appropriate WeatherException
        /// </summary>
        /// <param name="apiID">API ID of API where the request came from</param>
        /// <param name="response">HTTP response</param>
        /// <exception cref="WeatherException">Error status will correspond to specific error status</exception>
        public static async Task CheckForErrors(this IRateLimitedRequest @api, string apiID, System.Net.Http.HttpResponseMessage response)
        {
            await CheckForErrors(apiID, response, @api.GetRetryTime());
        }

        /// <summary>
        /// Checks if response was successful; if it was not, throw the appropriate WeatherException
        /// </summary>
        /// <param name="apiID">API ID of API where the request came from</param>
        /// <param name="response">HTTP response</param>
        /// <exception cref="WeatherException">Error status will correspond to specific error status</exception>
        public static async Task CheckForErrors(this System.Net.Http.HttpResponseMessage response, string apiID, IRateLimitedRequest @api)
        {
            await CheckForErrors(apiID, response, @api.GetRetryTime());
        }

        /// <exception cref="WeatherException">Will be thrown if response code is HTTP error code 429 *Too Many Requests*</exception>
        public static async Task ThrowIfRateLimited(string apiID, System.Net.Http.HttpResponseMessage response, long retryTimeInMs = 60000)
        {
            if ((int)response.StatusCode == 429)
            {
                SetNextRetryTime(apiID, retryTimeInMs);
                throw new WeatherException(WeatherUtils.ErrorStatus.NetworkError,
                    await response.CreateException());
            }
        }

        /// <exception cref="WeatherException">Will be thrown if response code is HTTP error code 429 *Too Many Requests*</exception>
        public static async Task ThrowIfRateLimited(this WeatherProviderImpl providerImpl, System.Net.Http.HttpResponseMessage response)
        {
            await ThrowIfRateLimited(providerImpl.WeatherAPI, response, providerImpl.GetRetryTime());
        }

        /// <exception cref="WeatherException">Will be thrown if response code is HTTP error code 429 *Too Many Requests*</exception>
        public static async Task ThrowIfRateLimited(this System.Net.Http.HttpResponseMessage response, WeatherProviderImpl providerImpl)
        {
            await ThrowIfRateLimited(providerImpl.WeatherAPI, response, providerImpl.GetRetryTime());
        }

        /// <exception cref="WeatherException">Will be thrown if response code is HTTP error code 429 *Too Many Requests*</exception>
        public static async Task ThrowIfRateLimited(this LocationProviderImpl providerImpl, System.Net.Http.HttpResponseMessage response)
        {
            await ThrowIfRateLimited(providerImpl.LocationAPI, response, providerImpl.GetRetryTime());
        }

        /// <exception cref="WeatherException">Will be thrown if response code is HTTP error code 429 *Too Many Requests*</exception>
        public static async Task ThrowIfRateLimited(this System.Net.Http.HttpResponseMessage response, LocationProviderImpl providerImpl)
        {
            await ThrowIfRateLimited(providerImpl.LocationAPI, response, providerImpl.GetRetryTime());
        }

        /// <exception cref="WeatherException">Will be thrown if response code is HTTP error code 429 *Too Many Requests*</exception>
        public static async Task ThrowIfRateLimited(this IRateLimitedRequest @api, string apiID, System.Net.Http.HttpResponseMessage response)
        {
            await ThrowIfRateLimited(apiID, response, @api.GetRetryTime());
        }

        public static async Task<Exception> CreateException(this System.Net.Http.HttpResponseMessage response)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var responseCode = (int)response.StatusCode;
            var requestMsg = response.RequestMessage?.ToString();
            var responseMsg = response.ToString();

            var exceptionMsg = new StringBuilder()
                .AppendLine($"HTTP Error {responseCode}")
                .AppendLine($"Request: {requestMsg}")
                .AppendLine($"Response Message: {responseMsg}")
                .AppendLine($"Response: {errorContent}")
                .ToString();

            return new Exception(exceptionMsg);
        }
        #endregion

        /// <summary>
        /// Check if API has been rate limited (HTTP Error 429 occurred recently).
        /// If so, deny API request until time passes
        /// </summary>
        /// <param name="apiID">API ID of API where the request came from</param>
        /// <exception cref="SimpleWeather.Utils.WeatherException">Exception if client is under rate limit</exception>
        public static void CheckRateLimit(string apiID)
        {
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var nextRetryTime = GetNextRetryTime(apiID);

            if (currentTime < nextRetryTime)
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.NetworkError,
                    new Exception($"Rate-limited: currentTime = {currentTime}, nextRetryTime = {nextRetryTime}"));
            }
        }

        /// <summary>
        /// Check if API has been rate limited (HTTP Error 429 occurred recently).
        /// If so, deny API request until time passes
        /// </summary>
        /// <exception cref="WeatherException">Exception if client is under rate limit</exception>
        public static void CheckRateLimit(this WeatherProviderImpl providerImpl)
        {
            CheckRateLimit(providerImpl.WeatherAPI);
        }

        /// <summary>
        /// Check if API has been rate limited (HTTP Error 429 occurred recently).
        /// If so, deny API request until time passes
        /// </summary>
        /// <exception cref="WeatherException">Exception if client is under rate limit</exception>
        public static void CheckRateLimit(this LocationProviderImpl providerImpl)
        {
            CheckRateLimit(providerImpl.LocationAPI);
        }

        private const string KEY_NEXTRETRYTIME = "key_nextretrytime";

        private static string GetRetryTimePrefKey(string apiID)
        {
            return $"{apiID}:{KEY_NEXTRETRYTIME}";
        }

        private static partial long GetNextRetryTime(string apiID);
        private static partial void SetNextRetryTime(string apiID, long retryTimeInMs);

        /// <summary>
        /// Returns random number of milliseconds as the delay offset based on given retry time
        /// <br /><br />
        /// If time >= 12 hours returns random number of minutes between (1 - 60min)
        /// <br />
        /// If time >= 1 hours returns random number of minutes between(1 - 30min)
        /// <br />
        /// If time >= 1 minute returns random number of minutes between(1 - 5min)
        /// <br />
        /// If time >= 30s returns random number of seconds between(5 - 15s)
        /// <br />
        /// If time >= 1s returns random number of seconds between(500 - 5000ms)
        /// </summary>
        private static long GetRandomOffset(long retryTimeInMs)
        {
            var random = new Random();

            /* 12 hours */
            if (retryTimeInMs >= 43200000L)
            {
                return random.Next(1, 60 + 1) * 60 * 1000;
            }
            /* 1 hour */
            else if (retryTimeInMs >= 3600000L)
            {
                return random.Next(1, 30 + 1) * 60 * 1000;
            }
            /* 1 minute */
            else if (retryTimeInMs >= 60000L)
            {
                return random.Next(1, 5 + 1) * 60 * 1000;
            }
            /* 30 seconds */
            else if (retryTimeInMs >= 30000L)
            {
                return random.Next(5, 15 + 1) * 1000;
            }
            else
            {
                return random.Next(500, 5000 + 1);
            }
        }
    }

    public interface IRateLimitedRequest
    {
        /// <summary>
        /// Time in milliseconds to wait until next request (ex. 60 calls / per minute -> 60s wait time)
        /// </summary>
        /// <returns>Retry time in milliseconds</returns>
        long GetRetryTime();
    }
}
