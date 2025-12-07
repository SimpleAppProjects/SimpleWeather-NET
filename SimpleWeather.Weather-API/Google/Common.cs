namespace SimpleWeather.Weather_API.Google;

public class Direction
{
    public string cardinal { get; set; }
    public int? degrees { get; set; }
}

public class Wind
{
    public Speed speed { get; set; }
    public Direction direction { get; set; }
    public Gust gust { get; set; }
}

public class Speed
{
    public string unit { get; set; }
    public float? value { get; set; }
}

public class Description
{
    public string text { get; set; }
    public string languageCode { get; set; }
}

public class WeatherCondition
{
    public string iconBaseUri { get; set; }
    public Description description { get; set; }
    public string type { get; set; }
}

public class Gust
{
    public string unit { get; set; }
    public float? value { get; set; }
}

public class Probability
{
    public string type { get; set; }
    public int? percent { get; set; }
}

public class Qpf
{
    public string unit { get; set; }
    public float? quantity { get; set; }
}

public class MaxTemperature
{
    public string unit { get; set; }
    public float? degrees { get; set; }
}

public class TimeZone
{
    public string id { get; set; }
}

public class MinTemperature
{
    public string unit { get; set; }
    public float? degrees { get; set; }
}

public class Precipitation
{
    public Probability probability { get; set; }
    public Qpf qpf { get; set; }
}

public class Temperature
{
    public string unit { get; set; }
    public float? degrees { get; set; }
}

public class WindChill
{
    public string unit { get; set; }
    public float? degrees { get; set; }
}

public class FeelsLikeTemperature
{
    public string unit { get; set; }
    public float? degrees { get; set; }
}

public class IceThickness
{
    public string unit { get; set; }
    public float? thickness { get; set; }
}

public class Visibility
{
    public string unit { get; set; }
    public float? distance { get; set; }
}

public class HeatIndex
{
    public string unit { get; set; }
    public float? degrees { get; set; }
}

public class DewPoint
{
    public string unit { get; set; }
    public float? degrees { get; set; }
}

public class Interval
{
    public string startTime { get; set; }
    public string endTime { get; set; }
}

public class AirPressure
{
    public float? meanSeaLevelMillibars { get; set; }
}