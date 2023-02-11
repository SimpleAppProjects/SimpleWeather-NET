using System.Collections.Generic;

namespace SimpleWeather.NET.Controls.Graphs
{
    public class BarGraphDataSet : GraphDataSet<BarGraphEntry>
    {
        public BarGraphDataSet(IEnumerable<BarGraphEntry> entries) : base(entries) { }

        public void SetMinMax(float? min = null, float? max = null)
        {
            if (min.HasValue)
            {
                this.YMin = min.Value;
            }

            if (max.HasValue)
            {
                this.YMax = max.Value;
            }
        }

        protected override void CalcMinMax(BarGraphEntry entry)
        {
            if (entry.EntryData?.Y != null && entry.EntryData.Y < YMin)
            {
                YMin = entry.EntryData.Y;
            }

            if (entry.EntryData?.Y != null && entry.EntryData.Y > YMax)
            {
                YMax = entry.EntryData.Y;
            }
        }
    }
}
