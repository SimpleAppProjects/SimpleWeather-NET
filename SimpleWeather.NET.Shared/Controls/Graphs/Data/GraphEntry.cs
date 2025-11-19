using SimpleWeather.SkiaSharp;

namespace SimpleWeather.NET.Controls.Graphs
{
    public abstract class GraphEntry
    {
        public string XLabel { get; set; }
        public SKDrawable XIcon { get; set; }
        public string XWeatherIcon { get; set; }
        public int XIconRotation { get; set; } = 0;

        public override bool Equals(object obj)
        {
            return obj is GraphEntry entry &&
                   XLabel == entry.XLabel &&
                   EqualityComparer<SKDrawable>.Default.Equals(XIcon, entry.XIcon) &&
                   XWeatherIcon == entry.XWeatherIcon &&
                   XIconRotation == entry.XIconRotation;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(XLabel, XIcon, XWeatherIcon, XIconRotation);
        }
    }
}
