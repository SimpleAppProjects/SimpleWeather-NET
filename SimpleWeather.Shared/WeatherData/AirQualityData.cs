using System.Collections.Generic;

namespace SimpleWeather.WeatherData
{
    public class AirQualityData
    {
        public AirQuality current { get; set; }
        public List<AirQuality> aqiForecast { get; set; }

        public AirQualityData() { }
    }
}
