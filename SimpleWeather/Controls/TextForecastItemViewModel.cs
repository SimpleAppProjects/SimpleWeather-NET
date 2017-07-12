using SimpleWeather.Utils;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Controls
{
    public class TextForecastItemViewModel
    {
        public string Title { get; set; }
        public string FctText { get; set; }
        public string WeatherIcon { get; set; }
        public string PoP { get; set; }

        public TextForecastItemViewModel()
        {
        }

        public TextForecastItemViewModel(TextForecast txt_forecast)
        {
            Title = txt_forecast.title;
            WeatherIcon = WeatherUtils.GetWeatherIcon(txt_forecast.icon);
            FctText = (Settings.Unit == Settings.Fahrenheit ?
                txt_forecast.fcttext : txt_forecast.fcttext_metric);
            PoP = txt_forecast.pop + "%";
        }
    }
}
