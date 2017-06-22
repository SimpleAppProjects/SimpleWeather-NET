using SimpleWeather.Utils;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Controls
{
    public class ForecastItemViewModel
    {
        public string WeatherIcon { get; set; }
        public string Date { get; set; }
        public string Condition { get; set; }
        public string HiTemp { get; set; }
        public string LoTemp { get; set; }

        public ForecastItemViewModel()
        {
        }

        public ForecastItemViewModel(Forecast forecast)
        {
            WeatherIcon = WeatherUtils.GetWeatherIcon(forecast.icon);
            Date = forecast.date.ToString("dddd dd");
            Condition = forecast.condition;
            HiTemp = (Settings.Unit == "F" ?
                forecast.high_f : forecast.high_c) + "º ";
            LoTemp = (Settings.Unit == "F" ?
                forecast.low_f : forecast.low_c) + "º ";
        }
    }
}
