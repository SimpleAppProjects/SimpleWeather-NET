using System;

namespace SimpleWeather.Weather_API.AccuWeather
{
    public class CurrentRootobject
    {
        public CurrentsItem[] Items { get; set; }
    }

    public class CurrentsItem
    {
        public DateTimeOffset LocalObservationDateTime { get; set; }
        public long? EpochTime { get; set; }
        public string WeatherText { get; set; }
        public int? WeatherIcon { get; set; }
        public bool? HasPrecipitation { get; set; }
        public string PrecipitationType { get; set; }
        public bool? IsDayTime { get; set; }
        public CurrentTemperature Temperature { get; set; }
        public CurrentRealfeeltemperature RealFeelTemperature { get; set; }
        public CurrentRealfeeltemperatureshade RealFeelTemperatureShade { get; set; }
        public int? RelativeHumidity { get; set; }
        public int? IndoorRelativeHumidity { get; set; }
        public CurrentDewpoint DewPoint { get; set; }
        public CurrentWind Wind { get; set; }
        public CurrentWindgust WindGust { get; set; }
        public float? UVIndex { get; set; }
        public string UVIndexText { get; set; }
        public CurrentVisibility Visibility { get; set; }
        public string ObstructionsToVisibility { get; set; }
        public int? CloudCover { get; set; }
        public CurrentCeiling Ceiling { get; set; }
        public Pressure Pressure { get; set; }
        public Pressuretendency PressureTendency { get; set; }
        public Past24hourtemperaturedeparture Past24HourTemperatureDeparture { get; set; }
        public Apparenttemperature ApparentTemperature { get; set; }
        public Windchilltemperature WindChillTemperature { get; set; }
        public CurrentWetbulbtemperature WetBulbTemperature { get; set; }
        public Precip1hr Precip1hr { get; set; }
        public Precipitationsummary PrecipitationSummary { get; set; }
        public Temperaturesummary TemperatureSummary { get; set; }
        public string MobileLink { get; set; }
        public string Link { get; set; }
    }

    public class CurrentTemperature
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class CurrentRealfeeltemperature
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class CurrentRealfeeltemperatureshade
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class CurrentDewpoint
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class CurrentWind
    {
        public CurrentDirection Direction { get; set; }
        public CurrentSpeed Speed { get; set; }
    }

    public class CurrentDirection
    {
        public int? Degrees { get; set; }
        public string Localized { get; set; }
        public string English { get; set; }
    }

    public class CurrentSpeed
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class CurrentWindgust
    {
        public CurrentSpeed Speed { get; set; }
    }

    public class CurrentVisibility
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class CurrentCeiling
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Pressure
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Pressuretendency
    {
        public string LocalizedText { get; set; }
        public string Code { get; set; }
    }

    public class Past24hourtemperaturedeparture
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Apparenttemperature
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Windchilltemperature
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class CurrentWetbulbtemperature
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Precip1hr
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Precipitationsummary
    {
        public Precipitation Precipitation { get; set; }
        public Pasthour PastHour { get; set; }
        public Past3hours Past3Hours { get; set; }
        public Past6hours Past6Hours { get; set; }
        public Past9hours Past9Hours { get; set; }
        public Past12hours Past12Hours { get; set; }
        public Past18hours Past18Hours { get; set; }
        public Past24hours Past24Hours { get; set; }
    }

    public class Precipitation
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Pasthour
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Past3hours
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Past6hours
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Past9hours
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Past12hours
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Past18hours
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Past24hours
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Temperaturesummary
    {
        public Past6hourrange Past6HourRange { get; set; }
        public Past12hourrange Past12HourRange { get; set; }
        public Past24hourrange Past24HourRange { get; set; }
    }

    public class Past6hourrange
    {
        public CurrentMinimum Minimum { get; set; }
        public CurrentMaximum Maximum { get; set; }
    }

    public class CurrentMinimum
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class CurrentMaximum
    {
        public Metric Metric { get; set; }
        public Imperial Imperial { get; set; }
    }

    public class Past12hourrange
    {
        public CurrentMinimum Minimum { get; set; }
        public CurrentMaximum Maximum { get; set; }
    }

    public class Past24hourrange
    {
        public CurrentMinimum Minimum { get; set; }
        public CurrentMaximum Maximum { get; set; }
    }

    public class Metric
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Imperial
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }
}
