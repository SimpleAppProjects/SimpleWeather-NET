using System;
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
    }

    public class OAuthRequest
    {
        private const string cOAuthVersion = "1.0";
        private const string cOAuthSignMethod = "HMAC-SHA1";
        private readonly string ConsumerKey;
        private readonly string ConsumerSecret;

        public OAuthRequest(String ConsumerKey, String ConsumerSecret)
        {
            this.ConsumerKey = ConsumerKey;
            this.ConsumerSecret = ConsumerSecret;
        }

        private static string GetTimeStamp()
        {
            TimeSpan lTS = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64(lTS.TotalSeconds).ToString();
        }

        private static string GetNonce()
        {
            return Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
        }

        // NOTE: whenever the value of a parameter is changed, say cUnitID "u=c" => "location=sunnyvale,ca"
        // The order in lSign needs to be updated, i.e. re-sort lSign
        // Please don't simply change value of any parameter without re-sorting.
        public string GetAuthorizationHeader(Uri url)
        {
            string lNonce = GetNonce();
            string lTimes = GetTimeStamp();
            string lCKey = string.Concat(ConsumerSecret, "&");

            IDictionary<string, string> oauthParams;

            if (!String.IsNullOrEmpty(url.Query))
                oauthParams = url.GetQueryParams();
            else
                oauthParams = new Dictionary<string, string>();

            oauthParams.Add("oauth_consumer_key", ConsumerKey);
            oauthParams.Add("oauth_nonce", lNonce);
            oauthParams.Add("oauth_signature_method", cOAuthSignMethod);
            oauthParams.Add("oauth_timestamp", lTimes);
            oauthParams.Add("oauth_version", cOAuthVersion);

            // Needs to be sorted || // note the sort order !!!
            var oauthParamKeys = oauthParams.Keys.ToList();
            oauthParamKeys.Sort();

            StringBuilder signBuilder = new StringBuilder();
            foreach (String key in oauthParamKeys)
            {
                signBuilder.AppendFormat("{0}={1}&", key, oauthParams[key]);
            }
            signBuilder.Remove(signBuilder.Length - 1, 1);

            string lSign = signBuilder.ToString();

            lSign = string.Concat(
             "GET&", Uri.EscapeDataString(url.AbsoluteWithoutQuery()), "&", Uri.EscapeDataString(lSign)
            );

            using (var lHasher = new HMACSHA1(Encoding.ASCII.GetBytes(lCKey)))
            {
                lSign = Convert.ToBase64String(lHasher.ComputeHash(Encoding.ASCII.GetBytes(lSign)));
            }

            return "OAuth " +
                   "oauth_consumer_key=\"" + ConsumerKey + "\", " +
                   "oauth_nonce=\"" + lNonce + "\", " +
                   "oauth_timestamp=\"" + lTimes + "\", " +
                   "oauth_signature_method=\"" + cOAuthSignMethod + "\", " +
                   "oauth_signature=\"" + lSign + "\", " +
                   "oauth_version=\"" + cOAuthVersion + "\"";
        }
    }
}
