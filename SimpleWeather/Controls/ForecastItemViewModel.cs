using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;

namespace SimpleWeather.Controls
{
    public class ForecastItemViewModel
    {
        private WeatherManager wm;

        public string WeatherIcon { get; set; }
        public string Date { get; set; }
        public string Condition { get; set; }
        public string HiTemp { get; set; }
        public string LoTemp { get; set; }

        public ForecastItemViewModel()
        {
            wm = WeatherManager.GetInstance();
        }

        public ForecastItemViewModel(Forecast forecast)
        {
            wm = WeatherManager.GetInstance();

#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);
#else
            var culture = CultureInfo.CurrentCulture;
#endif

            WeatherIcon = forecast.icon;
            Date = forecast.date.ToString("dddd dd", culture);
            Condition = forecast.condition;
            try
            {
                HiTemp = (Settings.IsFahrenheit ?
                    Math.Round(double.Parse(forecast.high_f)).ToString() : Math.Round(double.Parse(forecast.high_c)).ToString()) + "º ";
            }
            catch (FormatException ex)
            {
                HiTemp = "--º ";
                Logger.WriteLine(LoggerLevel.Error, "Invalid number format", ex);
            }
            try
            {
                LoTemp = (Settings.IsFahrenheit ?
                    Math.Round(double.Parse(forecast.low_f)).ToString() : Math.Round(double.Parse(forecast.low_c)).ToString()) + "º ";
            }
            catch (FormatException ex)
            {
                LoTemp = "--º ";
                Logger.WriteLine(LoggerLevel.Error, "Invalid number format", ex);
            }
        }
    }
}
