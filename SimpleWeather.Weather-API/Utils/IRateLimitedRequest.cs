namespace SimpleWeather.Weather_API.Utils
{
    public interface IRateLimitedRequest
    {
        /// <summary>
        /// Time in milliseconds to wait until next request (ex. 60 calls / per minute -> 60s wait time)
        /// </summary>
        /// <returns>Retry time in milliseconds</returns>
        long GetRetryTime();
    }
}
