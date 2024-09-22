namespace SimpleWeather.NET.Controls.Graphs
{
    public class ForecastRangeBarGraphData : GraphData<ForecastRangeBarGraphDataSet, ForecastRangeBarEntry>
    {
        public ForecastRangeBarGraphData() : base() { }

        public ForecastRangeBarGraphData(ForecastRangeBarGraphDataSet set) : base(set) { }

        public void SetDataSet(ForecastRangeBarGraphDataSet set)
        {
            DataSets.Clear();
            DataSets.Add(set);
            NotifyDataChanged();
        }

        public ForecastRangeBarGraphDataSet GetDataSet()
        {
            if (DataSets.Count != 0)
            {
                return DataSets.First();
            }
            else
            {
                return null;
            }
        }
    }
}
