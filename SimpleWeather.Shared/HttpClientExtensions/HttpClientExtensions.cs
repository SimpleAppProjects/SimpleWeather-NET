using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using SimpleWeather.Utils;
#if WINUI
using Windows.ApplicationModel;
#else
using Microsoft.Maui.ApplicationModel;
#endif

namespace SimpleWeather.HttpClientExtensions
{
    public static class HttpClientExtensions
    {
        public static void AddAppUserAgent(this HttpHeaderValueCollection<ProductInfoHeaderValue> UAHeader)
        {
#if WINUI
            var versionInfo = Package.Current.Id.Version;
#else
            var versionInfo = AppInfo.Current.Version;
#endif
            var version = string.Format("v{0}.{1}.{2}", versionInfo.Major, versionInfo.Minor, versionInfo.Build);

            UAHeader.Add(new ProductInfoHeaderValue("SimpleWeather", version));
            UAHeader.Add(new ProductInfoHeaderValue("(thewizrd.dev@gmail.com)"));
        }

        public static HttpRequestMessage Clone(this HttpRequestMessage request)
        {
            var clonedRequest = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Version = request.Version,
                VersionPolicy = request.VersionPolicy
            };

            request.Headers.ForEach(header =>
            {
                clonedRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            });

            foreach (var option in request.Options)
            {
                clonedRequest.Options.TryAdd(option.Key, option.Value);
            }

            return clonedRequest;
        }
    }
}