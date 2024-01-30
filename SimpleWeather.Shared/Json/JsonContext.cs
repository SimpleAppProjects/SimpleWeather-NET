using System.Text.Json.Serialization;

namespace SimpleWeather.Json
{
    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false,
        Converters = [typeof(ISO8601DateTimeFormatter), typeof(DateTimeOffsetFormatter)])
    ]
    [JsonSerializable(typeof(LocationData.LocationData))]
    [JsonSerializable(typeof(WeatherData.Weather))]
    [JsonSerializable(typeof(WeatherData.Location))]
    [JsonSerializable(typeof(WeatherData.Forecast))]
    [JsonSerializable(typeof(WeatherData.HourlyForecast))]
    [JsonSerializable(typeof(WeatherData.MinutelyForecast))]
    [JsonSerializable(typeof(WeatherData.TextForecast))]
    [JsonSerializable(typeof(WeatherData.ForecastExtras))]
    [JsonSerializable(typeof(WeatherData.Condition))]
    [JsonSerializable(typeof(WeatherData.Atmosphere))]
    [JsonSerializable(typeof(WeatherData.Astronomy))]
    [JsonSerializable(typeof(WeatherData.Precipitation))]
    [JsonSerializable(typeof(WeatherData.Beaufort))]
    [JsonSerializable(typeof(WeatherData.MoonPhase))]
    [JsonSerializable(typeof(WeatherData.UV))]
    [JsonSerializable(typeof(WeatherData.AirQuality))]
    [JsonSerializable(typeof(WeatherData.Pollen))]
    [JsonSerializable(typeof(WeatherData.WeatherAlert))]
    [JsonSerializable(typeof(WeatherData.Images.Model.ImageData))]
    [JsonSerializable(typeof(RemoteConfig.WeatherProviderConfig))]
    public partial class SharedJsonContext : JsonSerializerContext
    {
    }
}
