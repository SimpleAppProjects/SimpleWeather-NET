using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.HttpClientExtensions
{
    public partial class RetryPolicyHandler : DelegatingHandler
    {
        private static readonly IReadOnlySet<HttpStatusCode> RETRYABLE_STATUS = new HashSet<HttpStatusCode>()
        {
            HttpStatusCode.InternalServerError,
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout
        };

        private const int DEFAULT_RETRY_COUNT = 2;
        private const int DEFAULT_RETRY_DELAY_MS = 300;

        private const string HEADER_RETRY_COUNT = "X-Retry-Count";

        protected RetryPolicyHandler() : base() { }

        protected RetryPolicyHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response != null)
            {
                var retryDelay = Math.Min(response.Headers?.RetryAfter?.Delta?.TotalMilliseconds ?? DEFAULT_RETRY_DELAY_MS, 10000);
                var retryCount = GetHeaderValue(request.Headers, HEADER_RETRY_COUNT)?.TryParseInt() ?? DEFAULT_RETRY_COUNT;

                var tryCount = 0;

                while ((!response.IsSuccessStatusCode && RETRYABLE_STATUS.Contains(response.StatusCode)) && tryCount < retryCount)
                {
                    try { response.Dispose(); } catch { }

                    var expDelay = retryDelay * Math.Pow(2, tryCount);
                    tryCount++;

                    try
                    {
                        await Task.Delay((int)expDelay, cancellationToken);

                        response = await base.SendAsync(request, cancellationToken);

#if UNIT_TEST
                        response?.Headers?.Add(HEADER_RETRY_COUNT, [tryCount.ToInvariantString()]);
#endif

                        Logger.Debug(nameof(RetryPolicyHandler), $"Retried request: tryCount = {tryCount} | host: {request.RequestUri?.Host} | statusCode: {response.StatusCode}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(nameof(RetryPolicyHandler), ex);
                    }
                }
            }

            return response;
        }

        private static string GetHeaderValue(HttpHeaders headers, string key)
        {
            if (headers.TryGetValues(key, out var values))
            {
                return values?.FirstOrDefault();
            }

            return null;
        }
    }
}
