using SimpleWeather.SkiaSharp;

namespace SimpleWeather.NET.Controls.Graphs
{
    public class BarGraphEntry : GraphEntry
    {
        public YEntryData EntryData { get; set; }

        public Color? FillColor { get; set; }

        public BarGraphEntry() { }

        public BarGraphEntry(string label, YEntryData entryData, SKDrawable icon = null, int iconRotation = 0)
        {
            this.XLabel = label;
            this.EntryData = entryData;
            this.XIcon = icon;
            this.XIconRotation = iconRotation;
        }
    }
}
