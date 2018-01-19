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
        private WeatherManager wm;

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
            wm = WeatherManager.GetInstance();
        }

        public HourlyForecastItemViewModel(HourlyForecast hr_forecast)
        {
            wm = WeatherManager.GetInstance();

#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
            var culture = new System.Globalization.CultureInfo(userlang);
#else
            var culture = System.Globalization.CultureInfo.CurrentCulture;
#endif
            WeatherIcon = hr_forecast.icon;

#if WINDOWS_UWP
            if (culture.DateTimeFormat.ShortTimePattern.Contains("H"))
                Date = hr_forecast.date.ToString("ddd HH:00", culture);
            else
                Date = hr_forecast.date.ToString("ddd h tt", culture);
#elif __ANDROID__
            if (Android.Text.Format.DateFormat.Is24HourFormat(App.Context))
                Date = hr_forecast.date.ToString("ddd HH:00");
            else
                Date = hr_forecast.date.ToString("ddd h tt");
#endif
            Condition = hr_forecast.condition;
            HiTemp = (Settings.IsFahrenheit ? hr_forecast.high_f : hr_forecast.high_c) + "º ";
            PoP = hr_forecast.pop + "%";
            UpdateWindDirection(hr_forecast.wind_degrees);
            WindSpeed = (Settings.IsFahrenheit ?
               hr_forecast.wind_mph.ToString(culture) + " mph" : hr_forecast.wind_kph.ToString(culture) + " kph");
        }

        private void UpdateWindDirection(int angle)
        {
#if WINDOWS_UWP
            RotateTransform rotation = new RotateTransform()
            {
                Angle = angle - 180
            };
            WindDirection = rotation;
#elif __ANDROID__
            WindDirection = angle - 180;
#endif
        }
    }
}
