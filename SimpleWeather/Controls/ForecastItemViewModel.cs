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
#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
            var culture = new System.Globalization.CultureInfo(userlang);
#else
            var culture = System.Globalization.CultureInfo.CurrentCulture;
#endif

            WeatherIcon = WeatherUtils.GetWeatherIcon(forecast.icon);
            Date = forecast.date.ToString("dddd dd", culture);
            Condition = forecast.condition;
            HiTemp = (Settings.IsFahrenheit ? forecast.high_f : forecast.high_c) + "º ";
            LoTemp = (Settings.IsFahrenheit ? forecast.low_f : forecast.low_c) + "º ";
        }
    }
}
