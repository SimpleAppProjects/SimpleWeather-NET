using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.AQICN
{
    public class AQICNData : AirQuality
    {
        public List<Uvi> uvi_forecast { get; set; }

        public AQICNData(Rootobject root)
        {
            index = root?.data?.aqi;
            uvi_forecast = root.data?.forecast?.daily.uvi?.ToList();
            attribution = "World Air Quality Index Project";
        }
    }
}
