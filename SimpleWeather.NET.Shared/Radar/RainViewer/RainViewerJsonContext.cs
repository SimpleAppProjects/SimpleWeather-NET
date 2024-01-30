using System.Text.Json.Serialization;

namespace SimpleWeather.NET.Radar.RainViewer
{
    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)
    ]
    [JsonSerializable(typeof(Rootobject))]
    [JsonSerializable(typeof(Radar))]
    [JsonSerializable(typeof(RadarItem))]
    [JsonSerializable(typeof(Satellite))]
    [JsonSerializable(typeof(Infrared))]
    public partial class RainViewerJsonContext : JsonSerializerContext
    {
    }
}
