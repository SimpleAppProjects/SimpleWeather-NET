namespace SimpleWeather.Weather_API.Google;

using System.Collections.Generic;

public class DailyResponse
{
    public List<ForecastDaysItem> forecastDays { get; set; }
    public TimeZone timeZone { get; set; }
    public string nextPageToken { get; set; }
}

public class FeelsLikeMaxTemperature
{
    public string unit { get; set; }
    public float? degrees { get; set; }
}

public class NighttimeForecast
{
    public Precipitation precipitation { get; set; }
    public int? thunderstormProbability { get; set; }
    public int? relativeHumidity { get; set; }
    public int? cloudCover { get; set; }
    public Interval interval { get; set; }
    public WeatherCondition weatherCondition { get; set; }
    public float? uvIndex { get; set; }
    public Wind wind { get; set; }
}

public class DisplayDate
{
    public int? month { get; set; }
    public int? year { get; set; }
    public int? day { get; set; }
}

public class SunEvents
{
    public string sunsetTime { get; set; }
    public string sunriseTime { get; set; }
}

public class ForecastDaysItem
{
    public DisplayDate displayDate { get; set; }
    public Interval interval { get; set; }
    public DaytimeForecast daytimeForecast { get; set; }
    public MaxTemperature maxTemperature { get; set; }
    public IceThickness iceThickness { get; set; }
    public SunEvents sunEvents { get; set; }
    public MoonEvents moonEvents { get; set; }
    public MinTemperature minTemperature { get; set; }
    public FeelsLikeMinTemperature feelsLikeMinTemperature { get; set; }
    public MaxHeatIndex maxHeatIndex { get; set; }
    public FeelsLikeMaxTemperature feelsLikeMaxTemperature { get; set; }
    public NighttimeForecast nighttimeForecast { get; set; }
}

public class DaytimeForecast
{
    public Interval interval { get; set; }
    public WeatherCondition weatherCondition { get; set; }
    public Precipitation precipitation { get; set; }
    public int? thunderstormProbability { get; set; }
    public int? relativeHumidity { get; set; }
    public int? cloudCover { get; set; }
    public float? uvIndex { get; set; }
    public Wind wind { get; set; }
}

public class FeelsLikeMinTemperature
{
    public string unit { get; set; }
    public float? degrees { get; set; }
}

public class MoonEvents
{
    public List<string> moonriseTimes { get; set; }
    public List<string> moonsetTimes { get; set; }
    public string moonPhase { get; set; }
}

public class MaxHeatIndex
{
    public string unit { get; set; }
    public float? degrees { get; set; }
}