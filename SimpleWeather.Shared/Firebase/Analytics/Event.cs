using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SimpleWeather.Firebase.Analytics
{
    public class Event
    {
        [DataMember(Name = "name", IsRequired = true)]
        [JsonPropertyName("name")]
        [JsonProperty("name")]
        public string name { get; set; }
        [DataMember(Name = "params", IsRequired = false)]
        [JsonPropertyName("params")]
        [JsonProperty("params", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> _params { get; set; } = null;
    }
}
