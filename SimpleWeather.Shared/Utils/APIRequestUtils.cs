using SimpleWeather.Location;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace SimpleWeather.Utils
{
    public static partial class APIRequestUtils
    {
        /// <summary>
        /// Checks if response was successful; if it was not, throw the appropriate WeatherException
        /// </summary>
        /// <param name="apiID">API ID of API where the request came from</param>
        /// <param name="responseCode">HTTP response code</param>
        /// <param name="retryTimeInMs">Time in milliseconds to wait until next request</param>
        /// <exception cref="WeatherException">Error status will correspond to specific error status</exception>
        public static void CheckForErrors(string apiID, int responseCode, long retryTimeInMs = 60000)
        {
            switch (responseCode)
            {
                case (int)HttpStatusCode.Ok:
                    // ok
                    break;
                case (int)HttpStatusCode.BadRequest:
                    throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
                case (int)HttpStatusCode.NotFound:
                    throw new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
                case (int)HttpStatusCode.TooManyRequests:
                    ThrowIfRateLimited(apiID, responseCode, retryTimeInMs);
                    break;
                case (int)HttpStatusCode.InternalServerError:
                    throw new WeatherException(WeatherUtils.ErrorStatus.Unknown);
                default:
                    throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
            }
        }

        /// <summary>
        /// Checks if response was successful; if it was not, throw the appropriate WeatherException
        /// </summary>
        /// <param name="apiID">API ID of API where the request came from</param>
        /// <param name="responseCode">HTTP response code</param>
        /// <param name="retryTimeInMs">Time in milliseconds to wait until next request</param>
        /// <exception cref="WeatherException">Error status will correspond to specific error status</exception>
        public static void CheckForErrors(string apiID, HttpStatusCode responseCode, long retryTimeInMs = 60000)
        {
            CheckForErrors(apiID, (int)responseCode, retryTimeInMs);
        }

        /// <summary>
        /// Checks if response was successful; if it was not, throw the appropriate WeatherException
        /// </summary>
        /// <param name="apiID">API ID of API where the request came from</param>
        /// <param name="responseCode">HTTP response code</param>
        /// <param name="retryTimeInMs">Time in milliseconds to wait until next request</param>
        /// <exception cref="WeatherException">Error status will correspond to specific error status</exception>
        public static void CheckForErrors(string apiID, System.Net.HttpStatusCode responseCode, long retryTimeInMs = 60000)
        {
            CheckForErrors(apiID, (int)responseCode, retryTimeInMs);
        }

        /// <summary>
        /// Checks if response was successful; if it was not, throw the appropriate WeatherException
        /// </summary>
        /// <param name="responseCode">HTTP response code</param>
        /// <exception cref="WeatherException">Error status will correspond to specific error status</exception>
        public static void CheckForErrors(this WeatherProviderImpl providerImpl, HttpStatusCode responseCode)
        {
            CheckForErrors(providerImpl.WeatherAPI, responseCode, providerImpl.GetRetryTime());
        }

        /// <summary>
        /// Checks if response was successful; if it was not, throw the appropriate WeatherException
        /// </summary>
        /// <param name="responseCode">HTTP response code</param>
        /// <exception cref="WeatherException">Error status will correspond to specific error status</exception>
        public static void CheckForErrors(this LocationProviderImpl providerImpl, HttpStatusCode responseCode)
        {
            CheckForErrors(providerImpl.LocationAPI, responseCode, providerImpl.GetRetryTime());
        }

        /// <summary>
        /// Checks if response was successful; if it was not, throw the appropriate WeatherException
        /// </summary>
        /// <param name="apiID">API ID of API where the request came from</param>
        /// <param name="responseCode">HTTP response code</param>
        /// <exception cref="WeatherException">Error status will correspond to specific error status</exception>
        public static void CheckForErrors(this IRateLimitedRequest @api, string apiID, HttpStatusCode responseCode)
        {
            CheckForErrors(apiID, responseCode, @api.GetRetryTime());
        }

        /// <exception cref="WeatherException">Will be thrown if response code is HTTP error code 429 *Too Many Requests*</exception>
        public static void ThrowIfRateLimited(string apiID, int responseCode, long retryTimeInMs = 60000)
        {
            if (responseCode == ((int)HttpStatusCode.TooManyRequests))
            {
                SetNextRetryTime(apiID, retryTimeInMs);
                throw new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
            }
        }

        /// <exception cref="WeatherException">Will be thrown if response code is HTTP error code 429 *Too Many Requests*</exception>
        public static void ThrowIfRateLimited(this WeatherProviderImpl providerImpl, HttpStatusCode responseCode)
        {
            ThrowIfRateLimited(providerImpl.WeatherAPI, (int)responseCode, providerImpl.GetRetryTime());
        }

        /// <exception cref="WeatherException">Will be thrown if response code is HTTP error code 429 *Too Many Requests*</exception>
        public static void ThrowIfRateLimited(this LocationProviderImpl providerImpl, HttpStatusCode responseCode)
        {
            ThrowIfRateLimited(providerImpl.LocationAPI, (int)responseCode, providerImpl.GetRetryTime());
        }

        /// <exception cref="WeatherException">Will be thrown if response code is HTTP error code 429 *Too Many Requests*</exception>
        public static void ThrowIfRateLimited(this IRateLimitedRequest @api, string apiID, HttpStatusCode responseCode)
        {
            ThrowIfRateLimited(apiID, (int)responseCode, @api.GetRetryTime());
        }

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
                throw new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
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
