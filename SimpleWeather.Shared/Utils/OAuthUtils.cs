﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SimpleWeather.Utils
{
    public static class OAuthUtils
    {
        /// <summary>
        /// Get query parameters from Uri.
        /// </summary>
        /// <param name="uri">Uri to process.</param>
        /// <returns>Dictionary of query parameters.</returns>
        public static IDictionary<string, string> GetQueryParams(this Uri uri)
        {
            var dict = uri.Query.Remove(0, 1).Split('&').ToDictionary(c => c.Split('=')[0], c => Uri.UnescapeDataString(c.Split('=')[1]));
            return dict;
        }

        /// <summary>
        /// Get absolute Uri.
        /// </summary>
        /// <param name="uri">Uri to process.</param>
        /// <returns>Uri without query string.</returns>
        public static string AbsoluteWithoutQuery(this Uri uri)
        {
            if (string.IsNullOrEmpty(uri.Query))
            {
                return uri.AbsoluteUri;
            }

            return uri.AbsoluteUri.Replace(uri.Query, string.Empty);
        }

        public static string UriEncode(this String s)
        {
            return Uri.EscapeDataString(s).Replace("\\+", "%20");
        }
    }

    public enum OAuthSignatureMethod
    {
        HMAC_SHA1,
        HMAC_SHA256
    }

    public enum HTTPRequestType
    {
        GET,
        POST
    }

    public class OAuthRequest
    {
        private const string cOAuthVersion = "1.0";
        private const string cOAuthSignMethod_SHA1 = "HMAC-SHA1";
        private const string cOAuthSignMethod_SHA256 = "HMAC-SHA256";
        private readonly string ConsumerKey;
        private readonly string ConsumerSecret;
        private readonly OAuthSignatureMethod cOAuthSignMethod;
        private readonly HTTPRequestType cOAuthRequestType;

        public OAuthRequest(String ConsumerKey, String ConsumerSecret, OAuthSignatureMethod signMethod = OAuthSignatureMethod.HMAC_SHA1, HTTPRequestType requestType = HTTPRequestType.GET)
        {
            this.ConsumerKey = ConsumerKey;
            this.ConsumerSecret = ConsumerSecret;
            this.cOAuthSignMethod = signMethod;
            this.cOAuthRequestType = requestType;
        }

        private static string GetTimeStamp()
        {
            TimeSpan lTS = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64(lTS.TotalSeconds).ToString();
        }

        private static string GetNonce()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.Ticks.ToString()));
        }

        // NOTE: whenever the value of a parameter is changed, say cUnitID "u=c" => "location=sunnyvale,ca"
        // The order in lSign needs to be updated, i.e. re-sort lSign
        // Please don't simply change value of any parameter without re-sorting.
        public string GetAuthorizationHeader(Uri url, bool addGrantType = false)
        {
            string lNonce = GetNonce();
            string lTimes = GetTimeStamp();
            string lCKey = string.Concat(ConsumerSecret, "&");
            string lSignMethod = cOAuthSignMethod == OAuthSignatureMethod.HMAC_SHA256 ? cOAuthSignMethod_SHA256 : cOAuthSignMethod_SHA1;

            IDictionary<string, string> oauthParams;

            if (!String.IsNullOrEmpty(url.Query))
                oauthParams = url.GetQueryParams();
            else
                oauthParams = new Dictionary<string, string>();

            if (addGrantType)
            {
                oauthParams.Add("grant_type", "client_credentials");
            }
            oauthParams.Add("oauth_consumer_key", ConsumerKey);
            oauthParams.Add("oauth_nonce", lNonce);
            oauthParams.Add("oauth_signature_method", lSignMethod);
            oauthParams.Add("oauth_timestamp", lTimes);
            oauthParams.Add("oauth_version", cOAuthVersion);

            // Needs to be sorted || // note the sort order !!!
            var oauthParamKeys = oauthParams.Keys.ToList();
            oauthParamKeys.Sort();

            StringBuilder signBuilder = new StringBuilder();
            foreach (String key in oauthParamKeys)
            {
                signBuilder.AppendFormat("{0}={1}&", key.UriEncode(), oauthParams[key].UriEncode());
            }
            signBuilder.Remove(signBuilder.Length - 1, 1);

            string lSign = signBuilder.ToString();

            lSign = string.Concat(
                cOAuthRequestType == HTTPRequestType.GET ? "GET&" : "POST&",
                Uri.EscapeDataString(url.AbsoluteWithoutQuery()), "&", lSign.UriEncode()
            );

            HMAC lHasher;
            if (cOAuthSignMethod == OAuthSignatureMethod.HMAC_SHA256)
            {
                lHasher = new HMACSHA256(Encoding.UTF8.GetBytes(lCKey));
            }
            else
            {
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms
                lHasher = new HMACSHA1(Encoding.UTF8.GetBytes(lCKey));
#pragma warning restore CA5350 // Do Not Use Weak Cryptographic Algorithms
            }

            using (lHasher)
            {
                lSign = Convert.ToBase64String(lHasher.ComputeHash(Encoding.UTF8.GetBytes(lSign)));
            }

            return "OAuth " +
                   "oauth_consumer_key=\"" + ConsumerKey.UriEncode() + "\", " +
                   "oauth_nonce=\"" + lNonce.UriEncode() + "\", " +
                   "oauth_timestamp=\"" + lTimes + "\", " +
                   "oauth_signature_method=\"" + lSignMethod + "\", " +
                   "oauth_signature=\"" + lSign.UriEncode() + "\", " +
                   "oauth_version=\"" + cOAuthVersion + "\"";
        }
    }
}