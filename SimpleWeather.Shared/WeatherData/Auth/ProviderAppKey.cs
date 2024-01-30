using SimpleWeather.Utils;
using System.Linq;
using System.Text.Json.Serialization;

namespace SimpleWeather.WeatherData.Auth
{
    public class ProviderAppKey : ProviderKey
    {
        [JsonPropertyName("app_id")]
        public string AppID { get; set; } = string.Empty;
        [JsonPropertyName("app_code")]
        public string AppCode { get; set; } = string.Empty;

        public ProviderAppKey() : base() { }
        public ProviderAppKey(string appID, string appCode) : base()
        {
            this.AppID = appID;
            this.AppCode = appCode;
        }

        public override void FromString(string input)
        {
            input?.Split(':')?.Let(split =>
            {
                AppID = split.First();
                AppCode = split.Last();
            });
        }

        public override string ToString()
        {
            return $"${AppID}:${AppCode}";
        }
    }
}
