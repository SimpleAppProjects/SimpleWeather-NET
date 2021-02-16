using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls.Graphs;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Controls
{
    public class RangeBarGraphViewModel
    {
        public List<XLabelData> LabelData { get; private set; }
        public List<GraphTemperature> TempData { get; private set; }

        public RangeBarGraphViewModel(ICollection<Forecast> forecasts)
        {
            var isFahrenheit = Units.FAHRENHEIT.Equals(Settings.TemperatureUnit);
            var culture = CultureUtils.UserCulture;

            LabelData = new List<XLabelData>(12);
            TempData = new List<GraphTemperature>(12);

            foreach (var forecast in forecasts)
            {
                var tempData = new GraphTemperature();
                string date = forecast.date.ToString("ddd dd", culture);

                // Temp Data
                var xTemp = new XLabelData(date, forecast.icon, 0);
                if (forecast.high_f.HasValue && forecast.high_c.HasValue)
                {
                    int value = (int)(isFahrenheit ? Math.Round(forecast.high_f.Value) : Math.Round(forecast.high_c.Value));
                    var hiTemp = string.Format(culture, "{0}°", value);
                    tempData.HiTempData = new YEntryData(value, hiTemp);
                }
                if (forecast.low_f.HasValue && forecast.low_c.HasValue)
                {
                    int value = (int)(isFahrenheit ? Math.Round(forecast.low_f.Value) : Math.Round(forecast.low_c.Value));
                    var loTemp = string.Format(culture, "{0}°", value);
                    tempData.LoTempData = new YEntryData(value, loTemp);
                }

                LabelData.Add(xTemp);
                TempData.Add(tempData);
            }
        }
    }
}
