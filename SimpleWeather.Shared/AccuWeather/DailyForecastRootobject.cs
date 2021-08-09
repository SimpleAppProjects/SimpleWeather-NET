using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.AccuWeather
{
    public class DailyForecastRootobject
    {
        public Headline Headline { get; set; }
        public Dailyforecast[] DailyForecasts { get; set; }
    }

    public class Headline
    {
        public DateTimeOffset EffectiveDate { get; set; }
        public long? EffectiveEpochDate { get; set; }
        public int? Severity { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public long? EndEpochDate { get; set; }
        public string MobileLink { get; set; }
        public string Link { get; set; }
    }

    public class Dailyforecast
    {
        public DateTimeOffset Date { get; set; }
        public long? EpochDate { get; set; }
        public Sun Sun { get; set; }
        public Moon Moon { get; set; }
        public DailyTemperature Temperature { get; set; }
        public DailyRealfeeltemperature RealFeelTemperature { get; set; }
        public DailyRealfeeltemperatureshade RealFeelTemperatureShade { get; set; }
        public float? HoursOfSun { get; set; }
        public Degreedaysummary DegreeDaySummary { get; set; }
        public Airandpollen[] AirAndPollen { get; set; }
        public Day Day { get; set; }
        public Night Night { get; set; }
        public string[] Sources { get; set; }
        public string MobileLink { get; set; }
        public string Link { get; set; }
    }

    public class Sun
    {
        public DateTimeOffset? Rise { get; set; }
        public long? EpochRise { get; set; }
        public DateTimeOffset? Set { get; set; }
        public long? EpochSet { get; set; }
    }

    public class Moon
    {
        public DateTimeOffset? Rise { get; set; }
        public long? EpochRise { get; set; }
        public DateTimeOffset? Set { get; set; }
        public long? EpochSet { get; set; }
        public string Phase { get; set; }
        public int? Age { get; set; }
    }

    public class DailyTemperature
    {
        public Minimum Minimum { get; set; }
        public Maximum Maximum { get; set; }
    }

    public class Minimum
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Maximum
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class DailyRealfeeltemperature
    {
        public Minimum Minimum { get; set; }
        public Maximum Maximum { get; set; }
    }

    public class DailyRealfeeltemperatureshade
    {
        public Minimum Minimum { get; set; }
        public Maximum Maximum { get; set; }
    }

    public class Degreedaysummary
    {
        public Heating Heating { get; set; }
        public Cooling Cooling { get; set; }
    }

    public class Heating
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Cooling
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Day
    {
        public int? Icon { get; set; }
        public string IconPhrase { get; set; }
        public bool? HasPrecipitation { get; set; }
        public string ShortPhrase { get; set; }
        public string LongPhrase { get; set; }
        public int? PrecipitationProbability { get; set; }
        public int? ThunderstormProbability { get; set; }
        public int? RainProbability { get; set; }
        public int? SnowProbability { get; set; }
        public int? IceProbability { get; set; }
        public Wind Wind { get; set; }
        public Windgust WindGust { get; set; }
        public Totalliquid TotalLiquid { get; set; }
        public Rain Rain { get; set; }
        public Snow Snow { get; set; }
        public Ice Ice { get; set; }
        public float? HoursOfPrecipitation { get; set; }
        public float? HoursOfRain { get; set; }
        public float? HoursOfSnow { get; set; }
        public float? HoursOfIce { get; set; }
        public float? CloudCover { get; set; }
        public string PrecipitationType { get; set; }
        public string PrecipitationIntensity { get; set; }
    }

    public class Wind
    {
        public Speed Speed { get; set; }
        public Direction Direction { get; set; }
    }

    public class Speed
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Direction
    {
        public float? Degrees { get; set; }
        public string Localized { get; set; }
        public string English { get; set; }
    }

    public class Windgust
    {
        public Speed Speed { get; set; }
        public Direction Direction { get; set; }
    }

    public class Totalliquid
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Rain
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Snow
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Ice
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Night
    {
        public int? Icon { get; set; }
        public string IconPhrase { get; set; }
        public bool? HasPrecipitation { get; set; }
        public string ShortPhrase { get; set; }
        public string LongPhrase { get; set; }
        public int? PrecipitationProbability { get; set; }
        public int? ThunderstormProbability { get; set; }
        public int? RainProbability { get; set; }
        public int? SnowProbability { get; set; }
        public int? IceProbability { get; set; }
        public Wind Wind { get; set; }
        public Windgust WindGust { get; set; }
        public Totalliquid TotalLiquid { get; set; }
        public Rain Rain { get; set; }
        public Snow Snow { get; set; }
        public Ice Ice { get; set; }
        public float? HoursOfPrecipitation { get; set; }
        public float? HoursOfRain { get; set; }
        public float? HoursOfSnow { get; set; }
        public float? HoursOfIce { get; set; }
        public float? CloudCover { get; set; }
        public string PrecipitationType { get; set; }
        public string PrecipitationIntensity { get; set; }
    }

    public class Airandpollen
    {
        public string Name { get; set; }
        public int? Value { get; set; }
        public string Category { get; set; }
        public int? CategoryValue { get; set; }
        public string Type { get; set; }
    }
}
