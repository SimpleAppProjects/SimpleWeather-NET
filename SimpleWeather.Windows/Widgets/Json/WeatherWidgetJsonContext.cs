using SimpleWeather.NET.Widgets.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleWeather.NET.Widgets.Json
{
    public static class JsonContextExtensions
    {
        public static JsonSerializerOptions AddWeatherWidgetContexts(this JsonSerializerOptions options)
        {
            options.TypeInfoResolverChain.Add(WeatherWidgetJsonContext.Default);

            return options;
        }
    }

    [JsonSourceGenerationOptions(
        IncludeFields = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(WeatherWidgetData))]
    [JsonSerializable(typeof(WeatherWidgetCustomizeData))]
    public partial class WeatherWidgetJsonContext : JsonSerializerContext
    {
    }
}
