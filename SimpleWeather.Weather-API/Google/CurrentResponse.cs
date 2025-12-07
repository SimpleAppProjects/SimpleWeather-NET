namespace SimpleWeather.Weather_API.Google;

public class CurrentResponse
{
    public WindChill windChill { get; set; }
    public CurrentConditionsHistory currentConditionsHistory { get; set; }
    public Visibility visibility { get; set; }
    public AirPressure airPressure { get; set; }
    public int? thunderstormProbability { get; set; }
    public HeatIndex heatIndex { get; set; }
    public int? cloudCover { get; set; }
    public TimeZone timeZone { get; set; }
    public DewPoint dewPoint { get; set; }
    public FeelsLikeTemperature feelsLikeTemperature { get; set; }
    public string currentTime { get; set; }
    public Precipitation precipitation { get; set; }
    public Temperature temperature { get; set; }
    public int? relativeHumidity { get; set; }
    public bool? isDaytime { get; set; }
    public WeatherCondition weatherCondition { get; set; }
    public float? uvIndex { get; set; }
    public Wind wind { get; set; }
}

public class TemperatureChange
{
    public string unit { get; set; }
    public float? degrees { get; set; }
}

public class CurrentConditionsHistory
{
    public MaxTemperature maxTemperature { get; set; }
    public MinTemperature minTemperature { get; set; }
    public Qpf qpf { get; set; }
    public TemperatureChange temperatureChange { get; set; }
}