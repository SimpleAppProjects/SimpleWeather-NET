using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.MeteoFrance
{
    public class CurrentsRootobject
    {
        public CurrentsPosition position { get; set; }
        public long updated_on { get; set; }
        public Observation observation { get; set; }
    }

    public class CurrentsPosition
    {
        public float lat { get; set; }
        public float lon { get; set; }
        public string timezone { get; set; }
    }

    public class Observation
    {
        public float T { get; set; }
        public Wind wind { get; set; }
        public Weather weather { get; set; }
    }

    /*
    public class Wind
    {
        public int speed { get; set; }
        public int direction { get; set; }
        public string icon { get; set; }
    }

    public class Weather
    {
        public string icon { get; set; }
        public string desc { get; set; }
    }
    */
}
