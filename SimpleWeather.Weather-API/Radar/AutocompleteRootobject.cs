using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.Radar
{
    public class AutocompleteRootobject
    {
        public Meta meta { get; set; }
        public Address[] addresses { get; set; }
    }

    public class Meta
    {
        public int code { get; set; }
    }

    public class Address
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
        public Geometry geometry { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string countryFlag { get; set; }
        public string county { get; set; }
        public float distance { get; set; }
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
        public string placeLabel { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public float[] coordinates { get; set; }
    }

}
