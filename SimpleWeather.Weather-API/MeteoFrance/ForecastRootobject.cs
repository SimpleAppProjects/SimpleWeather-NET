using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SimpleWeather.Weather_API.MeteoFrance
{
    public class ForecastRootobject
    {
        public Position position { get; set; }
        public long updated_on { get; set; }
        public Daily_Forecast[] daily_forecast { get; set; }
        public Forecast[] forecast { get; set; }
        public Probability_Forecast[] probability_forecast { get; set; }
    }

    public class Position
    {
        public float lat { get; set; }
        public float lon { get; set; }
        public int alti { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public string dept { get; set; }
        public int rain_product_available { get; set; }
        public string timezone { get; set; }
        public string insee { get; set; }
        public int bulletin_cote { get; set; }
    }

    public class Daily_Forecast
    {
        public long dt { get; set; }
        public T T { get; set; }
        public Humidity humidity { get; set; }
        public Precipitation precipitation { get; set; }
        public float? uv { get; set; }
        public Weather12h weather12H { get; set; }
        public Sun sun { get; set; }
    }

    public class T
    {
        public float? min { get; set; }
        public float? max { get; set; }
        public float? sea { get; set; }
    }

    public class Humidity
    {
        public int? min { get; set; }
        public int? max { get; set; }
    }

    public class Precipitation
    {
        [DataMember(Name = "24h")]
        [JsonPropertyName("24h")]
        [JsonProperty("24h")]
        public float? _24h { get; set; }
    }

    public class Weather12h
    {
        public string icon { get; set; }
        public string desc { get; set; }
    }

    public class Sun
    {
        public long? rise { get; set; }
        public long? set { get; set; }
    }

    public class Forecast
    {
        public long dt { get; set; }
        public T1 T { get; set; }
        public int? humidity { get; set; }
        public float? sea_level { get; set; }
        public Wind wind { get; set; }
        public Rain rain { get; set; }
        public Snow snow { get; set; }
        public int iso0 { get; set; }
        public string rainsnowlimit { get; set; }
        public int? clouds { get; set; }
        public Weather weather { get; set; }
    }

    public class T1
    {
        public float? value { get; set; }
        public float? windchill { get; set; }
    }

    public class Wind
    {
        public float? speed { get; set; }
        public float? gust { get; set; }
        public int? direction { get; set; }
        public string icon { get; set; }
    }

    public class Rain
    {
        [DataMember(Name = "1h")]
        [JsonPropertyName("1h")]
        [JsonProperty("1h")]
        public float? _1h { get; set; }
        [DataMember(Name = "3h")]
        [JsonPropertyName("3h")]
        [JsonProperty("3h")]
        public float? _3h { get; set; }
        [DataMember(Name = "6h")]
        [JsonPropertyName("6h")]
        [JsonProperty("6h")]
        public float? _6h { get; set; }
    }

    public class Snow
    {
        [DataMember(Name = "1h")]
        [JsonPropertyName("1h")]
        [JsonProperty("1h")]
        public float? _1h { get; set; }
        [DataMember(Name = "3h")]
        [JsonPropertyName("3h")]
        [JsonProperty("3h")]
        public float? _3h { get; set; }
        [DataMember(Name = "6h")]
        [JsonPropertyName("6h")]
        [JsonProperty("6h")]
        public float? _6h { get; set; }
    }

    public class Weather
    {
        public string icon { get; set; }
        public string desc { get; set; }
    }

    public class Probability_Forecast
    {
        public long dt { get; set; }
        public Rain rain { get; set; }
        public Snow snow { get; set; }
        public int? freezing { get; set; }
    }
}