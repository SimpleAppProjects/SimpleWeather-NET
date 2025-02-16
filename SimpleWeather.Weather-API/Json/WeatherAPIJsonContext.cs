using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleWeather.Weather_API.Json
{
    public static class JsonContextExtensions
    {
        public static JsonSerializerOptions AddWeatherAPIContexts(this JsonSerializerOptions options)
        {
            options.TypeInfoResolverChain.Add(AccuWeatherJsonContext.Default);
            options.TypeInfoResolverChain.Add(AQICNJsonContext.Default);
            options.TypeInfoResolverChain.Add(BrightSkyJsonContext.Default);
            options.TypeInfoResolverChain.Add(ECCCJsonContext.Default);
            options.TypeInfoResolverChain.Add(HEREAPIJsonContext.Default);
            options.TypeInfoResolverChain.Add(MeteoFranceJsonContext.Default);
            options.TypeInfoResolverChain.Add(MetnoJsonContext.Default);
            options.TypeInfoResolverChain.Add(NWSJsonContext.Default);
            options.TypeInfoResolverChain.Add(OWMJsonContext.Default);
            options.TypeInfoResolverChain.Add(OWMOneCallJsonContext.Default);
            options.TypeInfoResolverChain.Add(RadarApiJsonContext.Default);
            options.TypeInfoResolverChain.Add(TomorrowIoJsonContext.Default);
            options.TypeInfoResolverChain.Add(WeatherApiJsonContext.Default);
            options.TypeInfoResolverChain.Add(WeatherBitJsonContext.Default);
            options.TypeInfoResolverChain.Add(WeatherUnlockedJsonContext.Default);
            options.TypeInfoResolverChain.Add(WeatherKitJsonContext.Default);
            options.TypeInfoResolverChain.Add(TZDBJsonContext.Default);

            return options;
        }
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(AccuWeather.CurrentRootobject))]
    [JsonSerializable(typeof(AccuWeather.DailyForecastRootobject))]
    [JsonSerializable(typeof(AccuWeather.GeopositionRootobject))]
    [JsonSerializable(typeof(AccuWeather.HourlyForecastRootobject))]
    public partial class AccuWeatherJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(AQICN.Rootobject))]
    public partial class AQICNJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(BrightSky.CurrentRootobject))]
    [JsonSerializable(typeof(BrightSky.ForecastRootobject))]
    [JsonSerializable(typeof(BrightSky.AlertsRootobject))]
    public partial class BrightSkyJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(ECCC.LocationsItem))]
    public partial class ECCCJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(HERE.Rootobject))]
    public partial class HEREAPIJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(MeteoFrance.CurrentsRootobject))]
    [JsonSerializable(typeof(MeteoFrance.ForecastRootobject))]
    [JsonSerializable(typeof(MeteoFrance.AlertsRootobject))]
    public partial class MeteoFranceJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(Metno.Rootobject))]
    [JsonSerializable(typeof(Metno.SunRootobject))]
    [JsonSerializable(typeof(Metno.MoonRootobject))]
    public partial class MetnoJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(NWS.AlertRootobject))]
    [JsonSerializable(typeof(NWS.Observation.ForecastRootobject))]
    public partial class NWSJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(OpenWeather.CurrentRootobject))]
    [JsonSerializable(typeof(OpenWeather.ForecastRootobject))]
    public partial class OWMJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(OpenWeather.OneCall.Rootobject))]
    [JsonSerializable(typeof(OpenWeather.OneCall.AirPollutionRootobject))]
    public partial class OWMOneCallJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(Radar.AutocompleteRootobject))]
    [JsonSerializable(typeof(Radar.GeocodeRootobject))]
    public partial class RadarApiJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(TomorrowIO.Rootobject))]
    [JsonSerializable(typeof(TomorrowIO.AlertRootobject))]
    public partial class TomorrowIoJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(WeatherApi.LocationItem))]
    [JsonSerializable(typeof(WeatherApi.ForecastRootobject))]
    public partial class WeatherApiJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(WeatherBit.CurrentRootobject))]
    [JsonSerializable(typeof(WeatherBit.ForecastRootobject))]
    [JsonSerializable(typeof(WeatherBit.HourlyRootobject))]
    [JsonSerializable(typeof(WeatherBit.AlertRootobject))]
    public partial class WeatherBitJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(WeatherUnlocked.CurrentRootobject))]
    [JsonSerializable(typeof(WeatherUnlocked.ForecastRootobject))]
    public partial class WeatherUnlockedJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(WeatherKit.AvailabilityRootobject))]
    [JsonSerializable(typeof(WeatherKit.Weather))]
    public partial class WeatherKitJsonContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false)]
    [JsonSerializable(typeof(TZDB.TimeZoneData))]
    public partial class TZDBJsonContext : JsonSerializerContext
    {
    }
}
