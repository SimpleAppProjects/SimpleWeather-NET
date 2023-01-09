using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Uno.Controls.Graphs
{
    public class LineViewData : GraphData<LineDataSeries, LineGraphEntry>
    {
        public LineViewData(IEnumerable<LineDataSeries> sets) : base(sets) { }
        public LineViewData(string label, IEnumerable<LineDataSeries> sets) : base(sets)
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
                if (set.SeriesMax.HasValue)
                {
                    if (YMax < set.SeriesMax)
                    {
                        YMax = set.SeriesMax.Value;
                    }
                }
                else if (YMax < set.YMax)
                {
                    YMax = set.YMax;
                }

                if (set.SeriesMin.HasValue)
                {
                    if (YMin > set.SeriesMin)
                    {
                        YMin = set.SeriesMin.Value;
                    }
                }
                else if (YMin > set.YMin)
                {
                    YMin = set.YMin;
                }
            }
        }
    }
}
