using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Controls.Graphs
{
    public abstract class GraphData<T, E> : IGraphData where T : GraphDataSet<E> where E : GraphEntry
    {
        public List<T> DataSets { get; protected set; }

        public float YMax { get; protected set; } = -float.MaxValue;
        public float YMin { get; protected set; } = float.MaxValue;

        public string GraphLabel { get; protected set; } = string.Empty;

        public GraphData()
        {
            DataSets = new List<T>();
        }

        public GraphData(params T[] sets)
        {
            DataSets = sets?.ToList() ?? new List<T>();
            NotifyDataChanged();
        }

        public GraphData(IEnumerable<T> sets)
        {
            DataSets = sets?.ToList() ?? new List<T>();
            NotifyDataChanged();
        }

        public void NotifyDataChanged()
        {
            CalcMinMax();
        }

        protected virtual void CalcMinMax()
        {
            YMax = -float.MaxValue;
            YMin = float.MaxValue;

            if (DataSets == null || DataSets.Count == 0)
            {
                return;
            }

            foreach (var set in DataSets)
            {
                CalcMinMax(set);
            }
        }

        public int DataCount => DataSets?.Count ?? 0;
        public bool IsEmpty => DataSets?.Count == 0;

        public T GetDataSetByIndex(int index)
        {
            if (IsEmpty || index >= DataCount)
            {
                return null;
            }

            return DataSets[index];
        }

        public void AddDataSet(T set)
        {
            CalcMinMax(set);
            DataSets.Add(set);
        }

        public bool RemoveDataSet(T set)
        {
            var removed = DataSets.Remove(set);

            if (removed)
            {
                NotifyDataChanged();
            }

            return removed;
        }

        protected virtual void CalcMinMax(T set)
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

        public void Clear()
        {
            DataSets?.Clear();
            NotifyDataChanged();
        }

        public int GetMaxDataSetLabelCount()
        {
            var count = 0;

            foreach (var set in DataSets)
            {
                if (set.DataCount > count)
                {
                    count = set.DataCount;
                }
            }

            return count;
        }

        // Note: All sets share the same set of labels and icons for the x-axis
        public List<E> DataLabels
        {
            get
            {
                var set = DataSets.FirstOrDefault();
                return set?.EntryData ?? new List<E>();
            }
        }
    }
}
