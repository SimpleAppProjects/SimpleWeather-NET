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

        public TextForecastItemViewModel(TextForecast txtForecast)
        {
            Title = txtForecast.title;
            WeatherIcon = txtForecast.icon;
            FctText = Settings.IsFahrenheit ? txtForecast.fcttext : txtForecast.fcttext_metric;
            PoP = txtForecast.pop + "%";
        }
    }
}