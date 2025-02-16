using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.Radar
{
    public class GeocodeRootobject
    {
        public GeocodeMeta meta { get; set; }
        public GeocodeAddress[] addresses { get; set; }
    }

    public class GeocodeMeta
    {
        public int code { get; set; }
    }

    public class GeocodeAddress
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
        public Geometry geometry { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string countryFlag { get; set; }
        public string county { get; set; }
        public string confidence { get; set; }
        public string borough { get; set; }
        public string city { get; set; }
        public string number { get; set; }
        public string neighborhood { get; set; }
        public string postalCode { get; set; }
        public string stateCode { get; set; }
        public string state { get; set; }
        public string street { get; set; }
        public string layer { get; set; }
        public string formattedAddress { get; set; }
        public string addressLabel { get; set; }
        public GeocodeTimezone timeZone { get; set; }
    }

    public class GeocodeTimezone
    {
        public string id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public DateTimeOffset currentTime { get; set; }
        public int utcOffset { get; set; }
        public int dstOffset { get; set; }
    }
}
