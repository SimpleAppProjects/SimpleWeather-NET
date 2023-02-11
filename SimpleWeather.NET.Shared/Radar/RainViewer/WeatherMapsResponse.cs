using System.Collections.Generic;

namespace SimpleWeather.NET.Radar.RainViewer
{
    public class Rootobject
    {
        public string version { get; set; }
        public int generated { get; set; }
        public string host { get; set; }
        public Radar radar { get; set; }
        public Satellite satellite { get; set; }
    }

    public class Radar
    {
        public List<RadarItem> past { get; set; }
        public List<RadarItem> nowcast { get; set; }
    }

    public class RadarItem
    {
        public int time { get; set; }
        public string path { get; set; }
    }

    public class Satellite
    {
        public List<Infrared> infrared { get; set; }
    }

    public class Infrared
    {
        public int time { get; set; }
        public string path { get; set; }
    }
}