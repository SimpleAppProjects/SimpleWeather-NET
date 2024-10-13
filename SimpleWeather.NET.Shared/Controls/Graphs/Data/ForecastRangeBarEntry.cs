using SimpleWeather.SkiaSharp;

namespace SimpleWeather.NET.Controls.Graphs
{
    public class ForecastRangeBarEntry
    {
        public string XLabel { get; set; }
        public string XIcon { get; set; }
        public int XIconRotation { get; set; }
        public YEntryData HiTempData { get; set; }
        public YEntryData LoTempData { get; set; }
        public int? PoP { get; set; } = null;
    }
}
