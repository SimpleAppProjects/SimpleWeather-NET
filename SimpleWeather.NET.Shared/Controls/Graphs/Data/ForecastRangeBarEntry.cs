#if WINDOWS
using Color = Windows.UI.Color;
#endif

namespace SimpleWeather.NET.Controls.Graphs
{
    public class ForecastRangeBarEntry : GraphEntry
    {
        public YEntryData HiTempData { get; set; }
        public YEntryData LoTempData { get; set; }
        public int? PoP { get; set; } = null;
        public (Color hiColor, Color loColor)? FillColors { get; set; } = null;

        public ForecastRangeBarEntry() { }

        public ForecastRangeBarEntry(
            string label,
            YEntryData hiTempData,
            YEntryData loTempData,
            int? pop = null,
            string weatherIcon = null)
        {
            this.XLabel = label;
            this.HiTempData = hiTempData;
            this.LoTempData = loTempData;
            this.PoP = pop;
            this.XWeatherIcon = weatherIcon;
        }

        public void SetFillColors(Color hiColor, Color loColor)
        {
            this.FillColors = (hiColor, loColor);
        }

        public void SetFillColors(Color color)
        {
            this.FillColors = (color, color);
        }
    }
}
