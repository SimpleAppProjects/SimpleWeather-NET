using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.AccuWeather
{
    public class HourlyForecastRootobject
    {
        public HourlyItem[] Items { get; set; }
    }

    public class HourlyItem
    {
        public DateTimeOffset DateTime { get; set; }
        public long? EpochDateTime { get; set; }
        public int? WeatherIcon { get; set; }
        public string IconPhrase { get; set; }
        public bool? HasPrecipitation { get; set; }
        public bool? IsDaylight { get; set; }
        public Temperature Temperature { get; set; }
        public Realfeeltemperature RealFeelTemperature { get; set; }
        public Wetbulbtemperature WetBulbTemperature { get; set; }
        public Dewpoint DewPoint { get; set; }
        public Wind Wind { get; set; }
        public Windgust WindGust { get; set; }
        public int? RelativeHumidity { get; set; }
        public int? IndoorRelativeHumidity { get; set; }
        public Visibility Visibility { get; set; }
        public Ceiling Ceiling { get; set; }
        public float? UVIndex { get; set; }
        public string UVIndexText { get; set; }
        public int? PrecipitationProbability { get; set; }
        public int? RainProbability { get; set; }
        public int? SnowProbability { get; set; }
        public int? IceProbability { get; set; }
        public Totalliquid TotalLiquid { get; set; }
        public Rain Rain { get; set; }
        public Snow Snow { get; set; }
        public Ice Ice { get; set; }
        public int? CloudCover { get; set; }
        public string MobileLink { get; set; }
        public string Link { get; set; }
    }

    public class Temperature
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Realfeeltemperature
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Wetbulbtemperature
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Dewpoint
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Visibility
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Ceiling
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }
}
