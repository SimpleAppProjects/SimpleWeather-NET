using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Uno.Controls.Graphs
{
    public class RangeBarGraphDataSet : GraphDataSet<RangeBarGraphEntry>
    {
        public RangeBarGraphDataSet(IEnumerable<RangeBarGraphEntry> entries) : base(entries) { }

        protected override void CalcMinMax(RangeBarGraphEntry entry)
        {
            if (entry.HiTempData?.Y != null && entry.HiTempData.Y < YMin)
            {
                YMin = entry.HiTempData.Y;
            }

            if (entry.HiTempData?.Y != null && entry.HiTempData.Y > YMax)
            {
                YMax = entry.HiTempData.Y;
            }

            if (entry.LoTempData?.Y != null && entry.LoTempData.Y < YMin)
            {
                YMin = entry.LoTempData.Y;
            }

            if (entry.LoTempData?.Y != null && entry.LoTempData.Y > YMax)
            {
                YMax = entry.LoTempData.Y;
            }
        }
    }
}
