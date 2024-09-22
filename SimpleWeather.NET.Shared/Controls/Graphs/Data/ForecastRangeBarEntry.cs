using SimpleWeather.SkiaSharp;

namespace SimpleWeather.NET.Controls.Graphs
{
    public class ForecastRangeBarEntry : RangeBarGraphEntry
    {
        public int? PoP { get; set; }

        public ForecastRangeBarEntry() { }

        public ForecastRangeBarEntry(string label, YEntryData hiTempData, YEntryData loTempData, SKDrawable icon = null, int iconRotation = 0, int? pop = null)
            : base(label, hiTempData, loTempData, icon, iconRotation)
        {
            this.PoP = pop;
        }
    }
}
