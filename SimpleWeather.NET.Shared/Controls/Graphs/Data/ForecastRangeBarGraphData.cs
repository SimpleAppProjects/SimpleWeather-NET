namespace SimpleWeather.NET.Controls.Graphs
{
    public partial class ForecastRangeBarGraphData : GraphData<ForecastRangeBarGraphDataSet, ForecastRangeBarEntry>
    {
        public ForecastRangeBarGraphData() : base() { }

        public ForecastRangeBarGraphData(ForecastRangeBarGraphDataSet set) : base(set) { }

        public ForecastRangeBarGraphData(string label, ForecastRangeBarGraphDataSet set) : base(set)
        {
            this.GraphLabel = label;
        }

        public void SetDataSet(ForecastRangeBarGraphDataSet set)
        {
            DataSets.Clear();
            DataSets.Add(set);
            NotifyDataChanged();
        }

        public ForecastRangeBarGraphDataSet GetDataSet()
        {
            return DataSets?.FirstOrDefault();
        }

        protected override void CalcMinMax(ForecastRangeBarGraphDataSet set)
        {
            if (set.Min == null && set.Max == null)
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
                if (set.Max is float setMax)
                {
                    if (YMax < setMax)
                    {
                        YMax = setMax;
                    }
                }
                else if (YMax < set.YMax)
                {
                    YMax = set.YMax;
                }

                if (set.Min is float setMin)
                {
                    if (YMin > setMin)
                    {
                        YMin = setMin;
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
