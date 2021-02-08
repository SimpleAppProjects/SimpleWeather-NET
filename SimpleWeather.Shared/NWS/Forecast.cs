using System;
using System.Runtime.Serialization;

namespace SimpleWeather.NWS.Observation
{
    public class ForecastRootobject
    {
        public string operationalMode { get; set; }
        public string srsName { get; set; }
        public DateTimeOffset creationDate { get; set; }
        public string creationDateLocal { get; set; }
        public string productionCenter { get; set; }
        public string credit { get; set; }
        public string moreInformation { get; set; }
        public Location location { get; set; }
        public Time time { get; set; }
        public Data data { get; set; }
        public Currentobservation currentobservation { get; set; }
    }

    public class Location
    {
        public string region { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string elevation { get; set; }
        public string wfo { get; set; }
        public string timezone { get; set; }
        public string areaDescription { get; set; }
        public string radar { get; set; }
        public string zone { get; set; }
        public string county { get; set; }
        public string firezone { get; set; }
        public string metar { get; set; }
    }

    public class Time
    {
        public string layoutKey { get; set; }
        public string[] startPeriodName { get; set; }
        public DateTimeOffset[] startValidTime { get; set; }
        public string[] tempLabel { get; set; }
    }

    public class Data
    {
        public string[] temperature { get; set; }
        public string[] pop { get; set; }
        public string[] weather { get; set; }
        public string[] iconLink { get; set; }
        public string[] hazard { get; set; }
        public string[] hazardUrl { get; set; }
        public string[] text { get; set; }
    }

    public class Currentobservation
    {
        public string id { get; set; }
        public string name { get; set; }
        public string elev { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string Date { get; set; }
        public string Temp { get; set; }
        public string Dewp { get; set; }
        public string Relh { get; set; }
        public string Winds { get; set; }
        public string Windd { get; set; }
        public string Gust { get; set; }
        public string Weather { get; set; }
        public string Weatherimage { get; set; }
        public string Visibility { get; set; }
        public string Altimeter { get; set; }
        public string SLP { get; set; }
        public string timezone { get; set; }
        public string state { get; set; }
        public string WindChill { get; set; }
    }

}