using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Maui.Controls.Graphs
{
    public abstract class GraphDataSet<T> : IGraphDataSet<T> where T : GraphEntry
    {
        public List<T> EntryData { get; protected set; }

        public float YMax { get; protected set; } = -float.MaxValue;
        public float YMin { get; protected set; } = float.MaxValue;

        public GraphDataSet()
        {
            EntryData = new List<T>();
        }

        public GraphDataSet(IEnumerable<T> entries)
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

        public void SetEntries(IEnumerable<T> entries)
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

        void IGraphDataSet.AddEntry(object? entry)
        {
            AddEntry((T)entry!);
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

        bool IGraphDataSet.RemoveEntry(object? entry)
        {
            if (IsCompatibleObject(entry))
            {
                return RemoveEntry((T)entry!);
            }

            return false;
        }

        public int GetEntryIndex(T entry)
        {
            return EntryData?.IndexOf(entry) ?? 0;
        }

        int IGraphDataSet.GetEntryIndex(object? entry)
        {
            if (IsCompatibleObject(entry))
            {
                return GetEntryIndex((T)entry!);
            }

            return -1;
        }

        public T GetEntryForIndex(int index)
        {
            return EntryData[index];
        }

        object? IGraphDataSet.GetEntryForIndex(int index)
        {
            return GetEntryForIndex(index);
        }

        public void NotifyDataSetChanged()
        {
            CalcMinMax();
        }

        private static bool IsCompatibleObject(object? value)
        {
            // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
            // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
            return (value is T) || (value == null && default(T) == null);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var entry in EntryData)
            {
                yield return entry;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
