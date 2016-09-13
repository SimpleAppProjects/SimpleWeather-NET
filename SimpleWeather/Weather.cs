using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather
{
    public class Weather
    {
        public string lastBuildDate;
        public Units units;
        public Location location;
        public Wind wind;
        public Atmosphere atmosphere;
        public Astronomy astronomy;
        public Condition condition;
        public Forecast[] forecasts;

        public Weather(Rootobject root)
        {
            lastBuildDate = root.query.created;
            units = root.query.results.channel.units;

            location = root.query.results.channel.location;
            location.lat = root.query.results.channel.item.lat;
            location._long = root.query.results.channel.item._long;
            location.description = root.query.results.channel.description;

            wind = root.query.results.channel.wind;
            atmosphere = root.query.results.channel.atmosphere;
            astronomy = root.query.results.channel.astronomy;
            condition = root.query.results.channel.item.condition;
            forecasts = root.query.results.channel.item.forecast;
        }
    }

    [DataContract]
    public class Rootobject
    {
        [DataMember]
        public Query query { get; set; }
    }

    [DataContract]
    public class Query
    {
        [DataMember]
        public int count { get; set; }
        [DataMember]
        public string created { get; set; }
        [DataMember]
        public string lang { get; set; }
        [DataMember]
        public Results results { get; set; }
    }

    [DataContract]
    public class Results
    {
        [DataMember]
        public Channel channel { get; set; }
    }

    [DataContract]
    public class Channel
    {
        [DataMember]
        public Units units { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string link { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string language { get; set; }
        [DataMember]
        public string lastBuildDate { get; set; }
        [DataMember]
        public string ttl { get; set; }
        [DataMember]
        public Location location { get; set; }
        [DataMember]
        public Wind wind { get; set; }
        [DataMember]
        public Atmosphere atmosphere { get; set; }
        [DataMember]
        public Astronomy astronomy { get; set; }
        [DataMember]
        public Image image { get; set; }
        [DataMember]
        public Item item { get; set; }
    }

    [DataContract]
    public class Units
    {
        [DataMember]
        public string distance { get; set; }
        [DataMember]
        public string pressure { get; set; }
        [DataMember]
        public string speed { get; set; }
        [DataMember]
        public string temperature { get; set; }
    }

    [DataContract]
    public class Location
    {
        [DataMember]
        public string city { get; set; }
        [DataMember]
        public string country { get; set; }
        [DataMember]
        public string region { get; set; }

        public string lat { get; set; }
        public string _long { get; set; }
        public string description { get; set; }
    }

    [DataContract]
    public class Wind
    {
        [DataMember]
        public string chill { get; set; }
        [DataMember]
        public string direction { get; set; }
        [DataMember]
        public string speed { get; set; }
    }

    [DataContract]
    public class Atmosphere
    {
        [DataMember]
        public string humidity { get; set; }
        [DataMember]
        public string pressure { get; set; }
        [DataMember]
        public string rising { get; set; }
        [DataMember]
        public string visibility { get; set; }
    }

    [DataContract]
    public class Astronomy
    {
        [DataMember]
        public string sunrise { get; set; }
        [DataMember]
        public string sunset { get; set; }
    }

    [DataContract]
    public class Image
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string width { get; set; }
        [DataMember]
        public string height { get; set; }
        [DataMember]
        public string link { get; set; }
        [DataMember]
        public string url { get; set; }
    }

    [DataContract]
    public class Item
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string lat { get; set; }
        [DataMember(Name = "long")]
        public string _long { get; set; }
        [DataMember]
        public string link { get; set; }
        [DataMember]
        public string pubDate { get; set; }
        [DataMember]
        public Condition condition { get; set; }
        [DataMember]
        public Forecast[] forecast { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public Guid guid { get; set; }
    }

    [DataContract]
    public class Condition
    {
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string date { get; set; }
        [DataMember]
        public string temp { get; set; }
        [DataMember]
        public string text { get; set; }
    }

    [DataContract]
    public class Guid
    {
        [DataMember]
        public string isPermaLink { get; set; }
    }

    [DataContract]
    public class Forecast
    {
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string date { get; set; }
        [DataMember]
        public string day { get; set; }
        [DataMember]
        public string high { get; set; }
        [DataMember]
        public string low { get; set; }
        [DataMember]
        public string text { get; set; }
    }

    public class Coordinate
    {
        private double lat = 0;
        private double _long = 0;

        public Coordinate(string coordinatePair)
        {
            setCoordinate(coordinatePair);
        }

        public Coordinate(double latitude, double longitude)
        {
            lat = latitude;
            _long = longitude;
        }

        public void setCoordinate(string coordinatePair)
        {
            string[] coord = coordinatePair.Split(',');
            lat = double.Parse(coord[0]);
            _long = double.Parse(coord[1]);
        }

        public override string ToString()
        {
            return "(" + lat + ", " + _long + ")";
        }
    }
}