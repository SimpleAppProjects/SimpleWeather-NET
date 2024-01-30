using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SimpleWeather.Maui.Updates
{
    public class UpdateInfo
    {
        [JsonPropertyName("version")]
        public int VersionCode { get; set; }
        [JsonPropertyName("updatePriority")]
        public int UpdatePriority { get; set; }
    }
}
