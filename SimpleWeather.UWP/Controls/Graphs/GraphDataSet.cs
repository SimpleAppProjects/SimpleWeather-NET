using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Controls.Graphs
{
    public abstract class GraphDataSet<T> : IGraphData where T : GraphEntry
    {
        public List<T> EntryData { get; protected set; }

        public float YMax { get; protected set; } = -float.MaxValue;
        public float YMin { get; protected set; } = float.MaxValue;

        public GraphDataSet()
        {
            EntryData = new List<T>();
        }

        public GraphDataSet(List<T> entries)
        {
            EntryData = entries?.ToList() ?? new List<T>();
            CalcMinMax();
        }

        public void CalcMinMax()
        {
            YMax = -float.MaxValue;
            YMin = float.MaxValue;

            if (EntryData == null || EntryData.Count == 0)
            {
                return;
            }

            foreach (var entry in EntryData)
            {
                CalcMinMax(entry);
            }
        }

        protected abstract void CalcMinMax(T entry);

        public void SetEntries(List<T> entries)
        {
            EntryData = entries?.ToList() ?? new List<T>();
            NotifyDataSetChanged();
        }

        public int DataCount => EntryData?.Count ?? 0;
        public bool IsEmpty => EntryData?.Count == 0;

        public void Clear()
        {
            EntryData?.Clear();
            NotifyDataSetChanged();
        }

        public void AddEntry(T entry)
        {
            CalcMinMax(entry);
            EntryData.Add(entry);
        }

        public bool RemoveEntry(T entry)
        {
            var removed = EntryData.Remove(entry);

            if (removed)
            {
                CalcMinMax();
            }

            return removed;
        }

        public int GetEntryIndex(T entry)
        {
            return EntryData?.IndexOf(entry) ?? 0;
        }

        public T GetEntryForIndex(int index)
        {
            return EntryData[index];
        }

        public void NotifyDataSetChanged()
        {
            CalcMinMax();
        }
    }
}
