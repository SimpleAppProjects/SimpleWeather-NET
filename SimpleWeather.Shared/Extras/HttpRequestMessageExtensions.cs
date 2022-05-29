using Microsoft.Extensions.DependencyInjection;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Extras
{
    public static class HttpRequestMessageExtensions
    {
        public static HttpRequestMessage CacheRequestIfNeeded(this HttpRequestMessage request, bool keyRequired, TimeSpan maxAge)
        {
            var ExtrasService = SharedModule.Instance.Services.GetService<IExtrasService>();

            if (ExtrasService.IsEnabled() || (keyRequired && Settings.UsePersonalKey))
            {
                // relax cache rules for the following users
            }
            else
            {
                request.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    MaxAge = maxAge
                };
            }

            return request;
        }
    }
}
