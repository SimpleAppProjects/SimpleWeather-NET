using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Maui.Controls.Graphs
{
    public class RangeBarGraphData : GraphData<RangeBarGraphDataSet, RangeBarGraphEntry>
    {
        public RangeBarGraphData() : base() { }

        public RangeBarGraphData(RangeBarGraphDataSet set) : base(set) { }

        public void SetDataSet(RangeBarGraphDataSet set)
        {
            DataSets.Clear();
            DataSets.Add(set);
            NotifyDataChanged();
        }

        public RangeBarGraphDataSet GetDataSet()
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
