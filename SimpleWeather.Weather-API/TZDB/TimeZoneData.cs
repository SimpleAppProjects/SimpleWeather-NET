using System.Text.Json.Serialization;

namespace SimpleWeather.Weather_API.TZDB
{
    public class TimeZoneData
    {
        [JsonPropertyName("tz_long")]
        public string TZLong { get; set; }
    }
}
