namespace SimpleWeather.NET.Controls.Graphs
{
    public partial class BarGraphDataSet : GraphDataSet<BarGraphEntry>
    {
        public string Label { get; set; } = string.Empty;

        public float? Min { get; private set; } = null;
        public float? Max { get; private set; } = null;

        public BarGraphDataSet(IEnumerable<BarGraphEntry> entries) : base(entries)
        {
            this.Label = null;
        }

        public BarGraphDataSet(string label, IEnumerable<BarGraphEntry> entries) : base(entries)
        {
            this.Label = label;
        }

        public void SetMinMax(float? min = null, float? max = null)
        {
            this.Min = min;
            this.Max = max;
        }

        protected override void CalcMinMax(BarGraphEntry entry)
        {
            if (Min == null)
            {
                if (entry.EntryData?.Y != null && entry.EntryData.Y < YMin)
                {
                    YMin = entry.EntryData.Y;
                }
            }

            if (Max == null)
            {
                if (entry.EntryData?.Y != null && entry.EntryData.Y > YMax)
                {
                    YMax = entry.EntryData.Y;
                }
            }
        }
    }
}
