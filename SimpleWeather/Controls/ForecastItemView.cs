using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather
{
    public class ForecastItemView
    {
        public string WeatherIcon { get; set; }
        public string Date { get; set; }
        public string Condition { get; set; }
        public string HiTemp { get; set; }
        public string LoTemp { get; set; }

        public ForecastItemView(Forecast forecast)
        {
            WeatherIcon = WeatherUtils.GetWeatherIcon(forecast.code);
            Date = forecast.date;
            Condition = forecast.text;
            HiTemp = forecast.high + "º ";
            LoTemp = forecast.low + "º";
        }
    }
}
