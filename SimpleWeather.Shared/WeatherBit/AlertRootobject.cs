using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherBit
{
    public class AlertRootobject
    {
        public string country_code { get; set; }
        public float lon { get; set; }
        public string timezone { get; set; }
        public float lat { get; set; }
        public Alert[] alerts { get; set; }
        public string city_name { get; set; }
        public string state_code { get; set; }
    }

    /*
    public class Alert
    {
        public string[] regions { get; set; }
        public string ends_utc { get; set; }
        public string effective_local { get; set; }
        public string onset_utc { get; set; }
        public string expires_local { get; set; }
        public string expires_utc { get; set; }
        public string ends_local { get; set; }
        public string uri { get; set; }
        public string onset_local { get; set; }
        public string effective_utc { get; set; }
        public string severity { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }
    */
}
