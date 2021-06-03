using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.MeteoFrance
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
        public float? _1h { get; set; }
        public float? _3h { get; set; }
        public float? _6h { get; set; }
    }

    public class Snow
    {
        public float? _1h { get; set; }
        public float? _3h { get; set; }
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
        public Rain1 rain { get; set; }
        public Snow1 snow { get; set; }
        public int? freezing { get; set; }
    }

    public class Rain1
    {
        public int? _3h { get; set; }
        public int? _6h { get; set; }
    }

    public class Snow1
    {
        public int? _3h { get; set; }
        public int? _6h { get; set; }
    }
}