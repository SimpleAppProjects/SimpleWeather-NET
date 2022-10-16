using System.Runtime.Serialization;

namespace SimpleWeather.WeatherData.Auth
{
    public class ProviderApiKey : ProviderKey
    {
        [DataMember(Name = "key")]
        public string Key { get; set; } = string.Empty;

        public ProviderApiKey() : base() { }
        public ProviderApiKey(string apikey) : base()
        {
            this.Key = apikey;
        }

        public override void FromString(string input)
        {
            this.Key = input;
        }

        public override string ToString()
        {
            return Key;
        }
    }
}
