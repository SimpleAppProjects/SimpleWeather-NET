namespace SimpleWeather.NET.Controls.Graphs
{
    public class ForecastRangeBarGraphDataSet : GraphDataSet<ForecastRangeBarEntry>
    {
        public ForecastRangeBarGraphDataSet(IEnumerable<ForecastRangeBarEntry> entries) : base(entries) { }

        protected override void CalcMinMax(ForecastRangeBarEntry entry)
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
