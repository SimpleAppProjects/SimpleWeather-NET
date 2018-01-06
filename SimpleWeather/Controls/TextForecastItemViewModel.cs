using SimpleWeather.Utils;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Controls
{
    public class TextForecastItemViewModel
    {
        private WeatherManager wm;

        public string Title { get; set; }
        public string FctText { get; set; }
        public string WeatherIcon { get; set; }
        public string PoP { get; set; }

        public TextForecastItemViewModel()
        {
            wm = WeatherManager.GetInstance();
        }

        public TextForecastItemViewModel(TextForecast txt_forecast)
        {
            wm = WeatherManager.GetInstance();

            Title = txt_forecast.title;
            WeatherIcon = wm.GetWeatherIcon(txt_forecast.icon);
            FctText = Settings.IsFahrenheit ? txt_forecast.fcttext : txt_forecast.fcttext_metric;
            PoP = txt_forecast.pop + "%";
        }
    }
}
