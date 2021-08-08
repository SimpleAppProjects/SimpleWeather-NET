using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.TomorrowIO
{
    public class AlertRootobject
    {
        public AlertData data { get; set; }
    }

    public class AlertData
    {
        public Event[] events { get; set; }
    }

    public class Event
    {
        public string insight { get; set; }
        public DateTimeOffset startTime { get; set; }
        public DateTimeOffset endTime { get; set; }
        public DateTimeOffset updateTime { get; set; }
        public string severity { get; set; }
        public string certainty { get; set; }
        public string urgency { get; set; }
        public Eventvalues eventValues { get; set; }
    }

    public class Eventvalues
    {
        public string origin { get; set; }
        public string title { get; set; }
        public string headline { get; set; }
        public string description { get; set; }
        public Response[] response { get; set; }
        //public string geocode { get; set; }
        //public string geocodeType { get; set; }
        //public string link { get; set; }
        //public Location location { get; set; }
        //public float distance { get; set; }
        //public float direction { get; set; }
    }

    /*
    public class Location
    {
        public string type { get; set; }
        public float[][][][] coordinates { get; set; }
    }
    */

    public class Response
    {
        public string instruction { get; set; }
    }
}
