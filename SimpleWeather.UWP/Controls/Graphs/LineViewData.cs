using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Controls.Graphs
{
    public class LineViewData : GraphData<LineDataSeries, LineGraphEntry>
    {
        public LineViewData(List<LineDataSeries> sets) : base(sets) { }
        public LineViewData(string label, List<LineDataSeries> sets) : base(sets)
        {
            this.GraphLabel = label;
        }

        protected override void CalcMinMax(LineDataSeries set)
        {
            if (set.SeriesMin == null && set.SeriesMax == null)
            {
                if (YMax < set.YMax)
                {
                    YMax = set.YMax;
                }
                if (YMin > set.YMin)
                {
                    YMin = set.YMin;
                }
            }
            else
            {
                if (set.SeriesMax.HasValue && YMax < set.SeriesMax)
                {
                    YMax = set.SeriesMax.Value;
                }
                if (set.SeriesMin.HasValue && YMin > set.SeriesMin)
                {
                    YMin = set.SeriesMin.Value;
                }
            }
        }
    }
}
