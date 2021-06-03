using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.MeteoFrance
{
    public class AlertsRootobject
    {
        public long update_time { get; set; }
        public long end_validity_time { get; set; }
        public string domain_id { get; set; }
        public int color_max { get; set; }
        public Timelap[] timelaps { get; set; }
        public Phenomenons_Items[] phenomenons_items { get; set; }
        public Advice[] advices { get; set; }
        public Consequence[] consequences { get; set; }
        public object max_count_items { get; set; }
        public Comments comments { get; set; }
        public object text { get; set; }
        public object text_avalanche { get; set; }
    }

    public class Comments
    {
        public long begin_time { get; set; }
        public long end_time { get; set; }
        public Text_Bloc_Item[] text_bloc_item { get; set; }
    }

    public class Text_Bloc_Item
    {
        public string title { get; set; }
        public string title_html { get; set; }
        //public object[] text_html { get; set; }
        public string[] text { get; set; }
    }

    public class Timelap
    {
        public int phenomenon_id { get; set; }
        public Timelaps_Items[] timelaps_items { get; set; }
    }

    public class Timelaps_Items
    {
        public long begin_time { get; set; }
        public int color_id { get; set; }
    }

    public class Phenomenons_Items
    {
        public int phenomenon_id { get; set; }
        public int phenomenon_max_color_id { get; set; }
    }

    public class Advice
    {
        public int phenomenon_id { get; set; }
        public int phenomenon_max_color_id { get; set; }
        public object text_advice { get; set; }
    }

    public class Consequence
    {
        public int phenomenon_id { get; set; }
        public int phenomenon_max_color_id { get; set; }
        public object text_consequence { get; set; }
    }
}
