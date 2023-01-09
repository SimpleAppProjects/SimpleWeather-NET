using System.Net.Http.Headers;
using Windows.ApplicationModel;

namespace SimpleWeather.HttpClientExtensions
{
    public static class HttpClientExtensions
    {
        public static void AddAppUserAgent(this HttpHeaderValueCollection<ProductInfoHeaderValue> UAHeader)
        {
            var version = string.Format("v{0}.{1}.{2}",
                Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

            UAHeader.Add(new ProductInfoHeaderValue("SimpleWeather", version));
            UAHeader.Add(new ProductInfoHeaderValue("(thewizrd.dev@gmail.com)"));
        }
    }
}
