namespace SimpleWeather.NET.Controls.Graphs
{
    public class ForecastRangeBarGraphDataSet : GraphDataSet<ForecastRangeBarEntry>
    {
        public string Label { get; set; } = string.Empty;

        public float? Min { get; private set; } = null;
        public float? Max { get; private set; } = null;

        public ForecastRangeBarGraphDataSet(IEnumerable<ForecastRangeBarEntry> entries) : base(entries)
        {
            this.Label = null;
        }

        public ForecastRangeBarGraphDataSet(string label, IEnumerable<ForecastRangeBarEntry> entries) : base(entries)
        {
            this.Label = label;
        }

        public void SetMinMax(float? min = null, float? max = null)
        {
            this.Min = min;
            this.Max = max;
        }

        protected override void CalcMinMax(ForecastRangeBarEntry entry)
        {
            if (Min == null)
            {
                if (entry.HiTempData?.Y != null && entry.HiTempData.Y < YMin)
                {
                    YMin = entry.HiTempData.Y;
                }

                if (entry.HiTempData?.Y != null && entry.HiTempData.Y > YMax)
                {
                    YMax = entry.HiTempData.Y;
                }
            }

            if (Max == null)
            {
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
}
