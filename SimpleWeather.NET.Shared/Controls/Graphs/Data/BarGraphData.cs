namespace SimpleWeather.NET.Controls.Graphs
{
    public class BarGraphData : GraphData<BarGraphDataSet, BarGraphEntry>
    {
        public BarGraphData() : base() { }

        public BarGraphData(BarGraphDataSet set) : base(set) { }

        public BarGraphData(string label, BarGraphDataSet set) : base(set)
        {
            this.GraphLabel = label;
        }

        public void SetDataSet(BarGraphDataSet set)
        {
            DataSets.Clear();
            DataSets.Add(set);
            NotifyDataChanged();
        }

        public BarGraphDataSet GetDataSet()
        {
            return DataSets?.FirstOrDefault();
        }

        protected override void CalcMinMax(BarGraphDataSet set)
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
