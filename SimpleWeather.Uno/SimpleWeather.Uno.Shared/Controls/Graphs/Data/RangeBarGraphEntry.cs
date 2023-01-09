using SimpleWeather.SkiaSharp;

namespace SimpleWeather.Uno.Controls.Graphs
{
    public class RangeBarGraphEntry : GraphEntry
    {
        public YEntryData HiTempData { get; set; }
        public YEntryData LoTempData { get; set; }

        public RangeBarGraphEntry() { }

        public RangeBarGraphEntry(string label, YEntryData hiTempData, YEntryData loTempData, SKDrawable icon = null, int iconRotation = 0)
        {
            this.XLabel = label;
            this.HiTempData = hiTempData;
            this.LoTempData = loTempData;
            this.XIcon = icon;
            this.XIconRotation = iconRotation;
        }
    }
}
