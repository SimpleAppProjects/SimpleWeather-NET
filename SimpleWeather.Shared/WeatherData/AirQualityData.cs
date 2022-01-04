using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public class AirQualityData
    {
        public AirQuality current { get; set; }
        public List<AirQuality> aqiForecast { get; set; }

        internal AirQualityData() { }
    }
}
