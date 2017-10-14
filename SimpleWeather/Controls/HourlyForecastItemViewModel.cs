using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
#if WINDOWS_UWP
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#elif __ANDROID__
using Android.Graphics;
using Android.Views;
using SimpleWeather.Droid;
#endif

namespace SimpleWeather.Controls
{
    public class HourlyForecastItemViewModel
    {
        public string WeatherIcon { get; set; }
        public string Date { get; set; }
        public string Condition { get; set; }
        public string HiTemp { get; set; }
        public string PoP { get; set; }
#if WINDOWS_UWP
        public Transform WindDirection { get; set; }
#elif __ANDROID__
        public int WindDirection { get; set; }
#endif
        public string WindSpeed { get; set; }

        public HourlyForecastItemViewModel()
        {
        }

        public HourlyForecastItemViewModel(HourlyForecast hr_forecast)
        {
            WeatherIcon = WeatherUtils.GetWeatherIcon(hr_forecast.icon);
            Date = hr_forecast.date.ToString("ddd h tt");
            Condition = hr_forecast.condition;
            HiTemp = (Settings.IsFahrenheit ? hr_forecast.high_f : hr_forecast.high_c) + "º ";
            PoP = hr_forecast.pop + "%";
            UpdateWindDirection(hr_forecast.wind_degrees);
            WindSpeed = (Settings.IsFahrenheit ?
               hr_forecast.wind_mph.ToString() + " mph" : hr_forecast.wind_kph.ToString() + " kph");
        }

        private void UpdateWindDirection(int angle)
        {
#if WINDOWS_UWP
            RotateTransform rotation = new RotateTransform()
            {
                Angle = angle
            };
            WindDirection = rotation;
#elif __ANDROID__
            WindDirection = angle;
#endif
        }
    }
}
