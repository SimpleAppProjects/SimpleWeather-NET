using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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
