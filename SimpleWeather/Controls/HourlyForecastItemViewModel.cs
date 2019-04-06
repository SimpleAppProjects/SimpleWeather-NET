using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.Controls
{
    public class HourlyForecastItemViewModel
    {
        private WeatherManager wm;

        public string WeatherIcon { get; set; }
        public string Date { get; set; }
        public string Condition { get; set; }
        public string HiTemp { get; set; }
        public string PoP { get; set; }
        public RotateTransform WindDirection { get; set; }
        public string WindSpeed { get; set; }

        public HourlyForecastItemViewModel()
        {
            wm = WeatherManager.GetInstance();
        }

        public HourlyForecastItemViewModel(HourlyForecast hr_forecast)
        {
            wm = WeatherManager.GetInstance();

            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            WeatherIcon = hr_forecast.icon;

            if (culture.DateTimeFormat.ShortTimePattern.Contains("H"))
                Date = hr_forecast.date.ToString("ddd HH:00", culture);
            else
                Date = hr_forecast.date.ToString("ddd h tt", culture);

            Condition = hr_forecast.condition;
            try
            {
                HiTemp = (Settings.IsFahrenheit ?
                    Math.Round(double.Parse(hr_forecast.high_f)).ToString() : Math.Round(double.Parse(hr_forecast.high_c)).ToString()) + "º ";
            }
            catch (FormatException ex)
            {
                HiTemp = "--º ";
                Logger.WriteLine(LoggerLevel.Error, "Invalid number format", ex);
            }
            PoP = hr_forecast.pop + "%";
            UpdateWindDirection(hr_forecast.wind_degrees);
            WindSpeed = (Settings.IsFahrenheit ?
               Math.Round(hr_forecast.wind_mph) + " mph" : Math.Round(hr_forecast.wind_kph) + " kph");
        }

        private void UpdateWindDirection(int angle)
        {
            RotateTransform rotation = new RotateTransform()
            {
                Angle = angle - 180
            };
            WindDirection = rotation;
        }
    }
}
