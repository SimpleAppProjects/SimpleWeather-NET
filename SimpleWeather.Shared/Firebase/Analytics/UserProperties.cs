using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SimpleWeather.Firebase.Analytics
{
    public class PropertyValue
    {
        [DataMember(Name = "value", IsRequired = true)]
        [JsonPropertyName("value")]
        [JsonProperty("value")]
        public string value { get; set; }
    }
}
