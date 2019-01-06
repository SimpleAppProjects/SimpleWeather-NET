using System;

namespace SimpleWeather.HERE
{
    public class Geo_Rootobject
    {
        public Response response { get; set; }
    }

    public class Response
    {
        public Metainfo metaInfo { get; set; }
        public View[] view { get; set; }
    }

    public class Metainfo
    {
        public DateTime timestamp { get; set; }
        public string nextPageInformation { get; set; }
    }

    public class View
    {
        public Result[] result { get; set; }
        public int viewId { get; set; }
    }

    public class Result
    {
        public float relevance { get; set; }
        public float distance { get; set; }
        public string matchLevel { get; set; }
        public Matchquality matchQuality { get; set; }
        public string matchType { get; set; }
        public GeoLocation location { get; set; }
    }

    public class Matchquality
    {
        public float country { get; set; }
        public float state { get; set; }
        public float county { get; set; }
        public float city { get; set; }
        public float district { get; set; }
        public float[] street { get; set; }
        public float houseNumber { get; set; }
        public float postalCode { get; set; }
    }

    public class GeoLocation
    {
        public string locationId { get; set; }
        public string locationType { get; set; }
        public Displayposition displayPosition { get; set; }
        public Navigationposition[] navigationPosition { get; set; }
        public Address address { get; set; }
        public Admininfo adminInfo { get; set; }
    }

    public class Displayposition
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public partial class Address
    {
        public string label { get; set; }
        /*
        public string country { get; set; }
        public string state { get; set; }
        public string county { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string street { get; set; }
        public string postalCode { get; set; }
        */
        public string houseNumber { get; set; }
        public Additionaldata[] additionalData { get; set; }
    }

    public class Additionaldata
    {
        public string value { get; set; }
        public string key { get; set; }
    }

    public class Admininfo
    {
        public DateTime localTime { get; set; }
        public string currency { get; set; }
        public string drivingSide { get; set; }
        public string systemOfMeasure { get; set; }
        public Timezone timeZone { get; set; }
    }

    public class Timezone
    {
        public int offset { get; set; }
        public int rawOffset { get; set; }
        public string nameShort { get; set; }
        public string nameLong { get; set; }
        public string nameDstShort { get; set; }
        public string nameDstLong { get; set; }
        public bool inDaylightTime { get; set; }
        public int dstSavings { get; set; }
        public string id { get; set; }
    }

    public class Navigationposition
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
    }
}
