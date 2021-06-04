using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.OpenWeather.OneCall
{
    public class AirPollutionRootobject
    {
        public Coord coord { get; set; }
        public List[] list { get; set; }
    }

    public class Coord
    {
        public float lon { get; set; }
        public float lat { get; set; }
    }

    public class List
    {
        public Main main { get; set; }
        public Components components { get; set; }
        public long dt { get; set; }
    }

    public class Main
    {
        public int aqi { get; set; }
    }

    public class Components
    {
        public double co { get; set; }
        public double no { get; set; }
        public double no2 { get; set; }
        public double o3 { get; set; }
        public double so2 { get; set; }
        public double pm2_5 { get; set; }
        public double pm10 { get; set; }
        public double nh3 { get; set; }
    }
}
