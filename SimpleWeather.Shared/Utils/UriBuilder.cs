﻿using System;
using System.Web;

namespace SimpleWeather.Utils
{
    public class UriBuilderEx
    {
        private UriBuilder BaseUriBuilder { get; }

        public UriBuilderEx(string uri)
        {
            BaseUriBuilder = new UriBuilder(uri);
        }

        public UriBuilderEx(Uri uri)
        {
            BaseUriBuilder = new UriBuilder(uri);
        }

        public UriBuilderEx AppendQueryParameter(string key, string value, bool encode = true)
        {
            string encodedParameter = (encode ? HttpUtility.UrlEncode(key) : key) + "="
                + (encode ? HttpUtility.UrlEncode(value) : value);

            if (BaseUriBuilder.Query == null)
            {
                BaseUriBuilder.Query = encodedParameter;
            }

            string oldQuery = BaseUriBuilder.Query;
            if (oldQuery?.Length == 0)
            {
                BaseUriBuilder.Query = encodedParameter;
            }
            else
            {
                BaseUriBuilder.Query = oldQuery + "&" + encodedParameter;
            }

            return this;
        }

        public UriBuilderEx AppendPath(string path, bool encode = true)
        {
            string encodedPath = encode ? HttpUtility.UrlEncode(path) : path;

            // Path returns '/' if empty
            if (BaseUriBuilder.Path == "/" || BaseUriBuilder.Path.EndsWith("/"))
            {
                BaseUriBuilder.Path += encodedPath;
            }
            else if (path.StartsWith("/"))
            {
                BaseUriBuilder.Path += encodedPath;
            }
            else
            {
                BaseUriBuilder.Path += "/" + encodedPath;
            }

            return this;
        }

        public Uri BuildUri()
        {
            return BaseUriBuilder.Uri;
        }

        public override string ToString()
        {
            return BaseUriBuilder.ToString();
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
