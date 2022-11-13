using System;

namespace SimpleWeather.Weather_API.AccuWeather
{
    public class GeopositionRootobject
    {
        public int? Version { get; set; }
        public string Key { get; set; }
        public string Type { get; set; }
        public int? Rank { get; set; }
        public string LocalizedName { get; set; }
        public string EnglishName { get; set; }
        public string PrimaryPostalCode { get; set; }
        public Region Region { get; set; }
        public Country Country { get; set; }
        public Administrativearea AdministrativeArea { get; set; }
        public Timezone TimeZone { get; set; }
        public Geoposition GeoPosition { get; set; }
        public bool? IsAlias { get; set; }
        public Parentcity ParentCity { get; set; }
        public Supplementaladminarea[] SupplementalAdminAreas { get; set; }
        public string[] DataSets { get; set; }
    }

    public class Region
    {
        public string ID { get; set; }
        public string LocalizedName { get; set; }
        public string EnglishName { get; set; }
    }

    public class Country
    {
        public string ID { get; set; }
        public string LocalizedName { get; set; }
        public string EnglishName { get; set; }
    }

    public class Administrativearea
    {
        public string ID { get; set; }
        public string LocalizedName { get; set; }
        public string EnglishName { get; set; }
        public int? Level { get; set; }
        public string LocalizedType { get; set; }
        public string EnglishType { get; set; }
        public string CountryID { get; set; }
    }

    public class Timezone
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public float? GmtOffset { get; set; }
        public bool? IsDaylightSaving { get; set; }
        public DateTimeOffset NextOffsetChange { get; set; }
    }

    public class Geoposition
    {
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public Elevation Elevation { get; set; }
    }

    public class Elevation
    {
        public GeoMetric Metric { get; set; }
        public GeoImperial Imperial { get; set; }
    }

    public class GeoMetric
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class GeoImperial
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
        public int? UnitType { get; set; }
    }

    public class Parentcity
    {
        public string Key { get; set; }
        public string LocalizedName { get; set; }
        public string EnglishName { get; set; }
    }

    public class Supplementaladminarea
    {
        public int? Level { get; set; }
        public string LocalizedName { get; set; }
        public string EnglishName { get; set; }
    }
}
