using System;

namespace SimpleWeather.Weather_API.HERE
{
    public class TokenRootobject
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

    internal class Token
    {
        public string access_token { get; set; }
        public DateTime expiration_date { get; set; }
    }
}
