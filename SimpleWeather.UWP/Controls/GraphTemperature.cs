using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls.Graphs;

namespace SimpleWeather.UWP.Controls
{
    public class GraphTemperature
    {
        public YEntryData HiTempData { get; set; }
        public YEntryData LoTempData { get; set; }
        public string TempUnit { get; }

        internal GraphTemperature(bool IsFahrenheit)
        {
            TempUnit = IsFahrenheit ? Settings.Fahrenheit : Settings.Celsius;
        }
    }
}
