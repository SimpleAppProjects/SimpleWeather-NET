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
            string[] date = txt_forecast.title.Split(' ');
            if (date.Length > 1)
                Title = string.Format("{0} {1}", date[0].Substring(0, 3), date[1]);
            else
                Title = date[0].Substring(0, 3);

            WeatherIcon = WeatherUtils.GetWeatherIcon(txt_forecast.icon);
            FctText = (Settings.Unit == Settings.Fahrenheit ?
                txt_forecast.fcttext : txt_forecast.fcttext_metric);
            PoP = txt_forecast.pop + "%";
        }
    }
}
