using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SimpleWeather.Maui.Updates
{
    public class UpdateInfo
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("versionCode")]
        public long VersionCode { get; set; }
        [JsonPropertyName("updatePriority")]
        public int UpdatePriority { get; set; }
    }
}
