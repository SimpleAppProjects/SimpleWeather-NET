using SimpleWeather.SkiaSharp;

namespace SimpleWeather.UWP.Controls.Graphs
{
    public class LineGraphEntry : GraphEntry
    {
        public YEntryData YEntryData { get; set; }

        public LineGraphEntry(string label, YEntryData yEntryData, SKDrawable icon = null, int iconRotation = 0)
        {
            this.XLabel = label;
            this.YEntryData = yEntryData;
            this.XIcon = icon;
            this.XIconRotation = iconRotation;
        }
    }
}
