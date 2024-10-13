namespace SimpleWeather.NET.Controls.Graphs
{
    public class ForecastRangeBarGraphDataSet
    {
        public List<ForecastRangeBarEntry> EntryData { get; private set; }

        public float YMax { get; private set; } = -float.MaxValue;
        public float YMin { get; private set; } = float.MaxValue;

        public ForecastRangeBarGraphDataSet()
        {
            EntryData = new List<ForecastRangeBarEntry>();
        }

        public ForecastRangeBarGraphDataSet(IEnumerable<ForecastRangeBarEntry> entries)
        {
            EntryData = entries?.ToList() ?? new List<ForecastRangeBarEntry>();
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

        private void CalcMinMax(ForecastRangeBarEntry entry)
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

        public void SetEntries(IEnumerable<ForecastRangeBarEntry> entries)
        {
            EntryData = entries?.ToList() ?? new List<ForecastRangeBarEntry>();
            NotifyDataSetChanged();
        }

        public int DataCount => EntryData?.Count ?? 0;
        public bool IsEmpty => EntryData?.Count == 0;

        public void Clear()
        {
            EntryData?.Clear();
            NotifyDataSetChanged();
        }

        public void AddEntry(ForecastRangeBarEntry entry)
        {
            CalcMinMax(entry);
            EntryData.Add(entry);
        }

        public bool RemoveEntry(ForecastRangeBarEntry entry)
        {
            var removed = EntryData.Remove(entry);

            if (removed)
            {
                CalcMinMax();
            }

            return removed;
        }

        public int GetEntryIndex(ForecastRangeBarEntry entry)
        {
            return EntryData?.IndexOf(entry) ?? 0;
        }

        public ForecastRangeBarEntry GetEntryForIndex(int index)
        {
            return EntryData[index];
        }

        public void NotifyDataSetChanged()
        {
            CalcMinMax();
        }
    }
}
