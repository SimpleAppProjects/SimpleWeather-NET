﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.HttpClientExtensions
{
    public partial class CacheFilter : RetryPolicyHandler
    {
        private const string CACHE_CONTROL_NO_CACHE = "no-cache";

        public CacheFilter() :
            base(new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            })
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                string cacheHeaderValue = request.Headers.CacheControl?.ToString();
                var shouldUseCache = !Equals(cacheHeaderValue?.ToLowerInvariant(), CACHE_CONTROL_NO_CACHE.ToLowerInvariant());

                if (!shouldUseCache)
                {
                    return response;
                }

                var hasCacheHeader = !String.IsNullOrWhiteSpace(cacheHeaderValue);

                // Override server cache protocol
                response.Headers.Pragma.Clear();

                if (!hasCacheHeader)
                {
                    // If original response does not contain a Cache-Control header
                    // cache the response for a minimum of 2 min to avoid repeat requests
                    response.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromMinutes(2)
                    };

                }
                else
                {
                    response.Headers.CacheControl = request.Headers.CacheControl;
                }

                return response;
            });
        }
    }
}
