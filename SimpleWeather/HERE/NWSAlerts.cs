using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.HERE
{
    public class Nwsalerts
    {
        public Warning[] warning { get; set; }
        public Watch[] watch { get; set; }
    }

    public class Warning
    {
        public string type { get; set; }
        public string description { get; set; }
        public int severity { get; set; }
        public string message { get; set; }
        //public County[] county { get; set; }
        //public object[] location { get; set; }
        public string name { get; set; }
        public DateTime validFromTimeLocal { get; set; }
        public DateTime validUntilTimeLocal { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }

    /*
    public class County
    {
        public string value { get; set; }
        public string country { get; set; }
        public string countryName { get; set; }
        public string state { get; set; }
        public string stateName { get; set; }
        public string name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }
    */

    public class Watch
    {
        public string type { get; set; }
        public string description { get; set; }
        public int severity { get; set; }
        public string message { get; set; }
        //public Zone[] zone { get; set; }
        //public object[] location { get; set; }
        public string name { get; set; }
        public DateTime validFromTimeLocal { get; set; }
        public DateTime validUntilTimeLocal { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }

    /*
    public class Zone
    {
        public string value { get; set; }
        public string country { get; set; }
        public string countryName { get; set; }
        public string state { get; set; }
        public string stateName { get; set; }
        public string name { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
    }
    */
}
