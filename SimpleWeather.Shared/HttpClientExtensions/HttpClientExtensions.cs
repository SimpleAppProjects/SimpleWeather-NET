using System.Net.Http.Headers;
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
    }
}
