using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SimpleWeather.Utils
{
    public class UriBuilderEx
    {
        private UriBuilder baseUriBuilder { get; }
        private NameValueCollection query { get; set; }

        public UriBuilderEx(string uri)
        {
            baseUriBuilder = new UriBuilder(uri);
            query = HttpUtility.ParseQueryString(baseUriBuilder.Query);
        }

        public UriBuilderEx(Uri uri)
        {
            baseUriBuilder = new UriBuilder(uri);
            query = HttpUtility.ParseQueryString(baseUriBuilder.Query);
        }

        public UriBuilderEx AppendQueryParameter(string key, string value)
        {
            query[key] = value;
            return this;
        }

        public UriBuilderEx AppendPath(string path)
        {
            // Path returns '/' if empty
            if (baseUriBuilder.Path == "/" || baseUriBuilder.Path.EndsWith('/'))
            {
                baseUriBuilder.Path += HttpUtility.UrlEncode(path); 
            }
            else if (path.StartsWith('/'))
            {
                baseUriBuilder.Path += HttpUtility.UrlEncode(path);
            }
            else
            {
                baseUriBuilder.Path += "/" + HttpUtility.UrlEncode(path);
            }

            return this;
        }

        public Uri BuildUri()
        {
            baseUriBuilder.Query = query.ToString();
            return baseUriBuilder.Uri;
        }

        public override string ToString()
        {
            baseUriBuilder.Query = query.ToString();
            return baseUriBuilder.ToString();
        }
    }

    public static class UriBuilderExtensions
    {
        public static UriBuilderEx ToUriBuilderEx(this UriBuilder uriBuilder)
        {
            return new UriBuilderEx(uriBuilder.Uri);
        }

        public static UriBuilderEx ToUriBuilderEx(this string uri)
        {
            return new UriBuilderEx(uri);
        }
    }
}
