namespace SimpleWeather.Weather_API.Google;

using System.Collections.Generic;

public class HourlyResponse
{
    public TimeZone timeZone { get; set; }
    public List<ForecastHoursItem> forecastHours { get; set; }
    public string nextPageToken { get; set; }
}

public class WetBulbTemperature
{
    public string unit { get; set; }
    public object degrees { get; set; }
}

public class ForecastHoursItem
{
    public DisplayDateTime displayDateTime { get; set; }
    public Temperature temperature { get; set; }
    public Interval interval { get; set; }
    public bool? isDaytime { get; set; }
    public WeatherCondition weatherCondition { get; set; }
    public WindChill windChill { get; set; }
    public IceThickness iceThickness { get; set; }
    public Visibility visibility { get; set; }
    public AirPressure airPressure { get; set; }
    public int? thunderstormProbability { get; set; }
    public HeatIndex heatIndex { get; set; }
    public int? cloudCover { get; set; }
    public DewPoint dewPoint { get; set; }
    public FeelsLikeTemperature feelsLikeTemperature { get; set; }
    public Precipitation precipitation { get; set; }
    public int? relativeHumidity { get; set; }
    public float? uvIndex { get; set; }
    public WetBulbTemperature wetBulbTemperature { get; set; }
    public Wind wind { get; set; }
}

public class DisplayDateTime
{
    public int? hours { get; set; }
    public string utcOffset { get; set; }
    public int? month { get; set; }
    public int? year { get; set; }
    public int? day { get; set; }
}