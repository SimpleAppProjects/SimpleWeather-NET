using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Ambee
{
    public class Rootobject
    {
        public string message { get; set; }
        public float lat { get; set; }
        public float lng { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public Count Count { get; set; }
        public Risk Risk { get; set; }
        public DateTimeOffset updatedAt { get; set; }
    }

    public class Count
    {
        public int grass_pollen { get; set; }
        public int tree_pollen { get; set; }
        public int weed_pollen { get; set; }
    }

    public class Risk
    {
        public string grass_pollen { get; set; }
        public string tree_pollen { get; set; }
        public string weed_pollen { get; set; }
    }
}
