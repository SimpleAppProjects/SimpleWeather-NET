using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.ObjectModel;
#if WINDOWS_UWP
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#elif __ANDROID__
using Android.Graphics;
using Android.Views;
#endif

namespace SimpleWeather.Controls
{
    public class WeatherNowViewModel
    {
        public string Location { get; set; }
        public string UpdateDate { get; set; }

        // Current Condition
        public string CurTemp { get; set; }
        public string CurCondition { get; set; }
        public string WeatherIcon { get; set; }

        // Weather Details
        public string Humidity { get; set; }
        public string Pressure { get; set; }
#if WINDOWS_UWP
        public Visibility RisingVisiblity { get; set; }
#elif __ANDROID__
        public ViewStates RisingVisiblity { get; set; }
#endif
        public string RisingIcon { get; set; }
        public string _Visibility { get; set; }
        public string WindChill { get; set; }
#if WINDOWS_UWP
        public Transform WindDirection { get; set; }
#elif __ANDROID__
        public int WindDirection { get; set; }
#endif
        public string WindSpeed { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }

        // Forecast
        public ObservableCollection<ForecastItemViewModel> Forecasts { get; set; }

        // Background
#if WINDOWS_UWP
        public ImageBrush Background { get; set; }
        public SolidColorBrush PanelBackground { get; set; }
#elif __ANDROID__
        public string Background { get; set; }
        public Color PanelBackground { get; set; }
#endif

        public WeatherNowViewModel()
        {
#if WINDOWS_UWP
            Background = new ImageBrush()
            {
                Stretch = Stretch.UniformToFill,
                AlignmentX = AlignmentX.Center
            };
#endif
        }

        public WeatherNowViewModel(Weather weather)
        {
#if WINDOWS_UWP
            Background = new ImageBrush()
            {
                Stretch = Stretch.UniformToFill,
                AlignmentX = AlignmentX.Center
            };
#endif

            UpdateView(weather);
        }

        public void UpdateView(Weather weather)
        {
            // Update backgrounds
#if WINDOWS_UWP
            WeatherUtils.SetBackground(Background, weather);
            PanelBackground = new SolidColorBrush(WeatherUtils.IsNight(weather) ?
                Color.FromArgb(45, 128, 128, 128) : Color.FromArgb(15, 8, 8, 8));
#elif __ANDROID__
            Background = WeatherUtils.GetBackgroundURI(weather);
            PanelBackground = WeatherUtils.IsNight(weather) ? new Color(128, 128, 128, 45) : new Color(8, 8, 8, 15);
#endif

            // Location
            Location = weather.location.name;

            // Date Updated
            UpdateDate = WeatherUtils.GetLastBuildDate(weather);

            // Update Current Condition
            CurTemp = Settings.Unit == Settings.Fahrenheit ?
                Math.Round(weather.condition.temp_f) + "\uf045" : Math.Round(weather.condition.temp_c) + "\uf03c";
            CurCondition = weather.condition.weather;
            WeatherIcon = WeatherUtils.GetWeatherIcon(weather.condition.icon);

            // WeatherDetails
            // Astronomy
            Sunrise = weather.astronomy.sunrise.ToString("h:mm tt");
            Sunset = weather.astronomy.sunset.ToString("h:mm tt");

            // Wind
            WindChill = Settings.Unit == Settings.Fahrenheit ?
                Math.Round(weather.condition.feelslike_f) + "º" : Math.Round(weather.condition.feelslike_c) + "º";
            WindSpeed = Settings.Unit == Settings.Fahrenheit ?
                weather.condition.wind_mph.ToString() + " mph" : weather.condition.wind_kph.ToString() + " kph";
            UpdateWindDirection(weather.condition.wind_degrees);

            // Atmosphere
            Humidity = weather.atmosphere.humidity;
            Pressure = Settings.Unit == Settings.Fahrenheit ?
                weather.atmosphere.pressure_in + " in" : weather.atmosphere.pressure_mb + " mb";
            UpdatePressureState(weather.atmosphere.pressure_trend);
            _Visibility = Settings.Unit == Settings.Fahrenheit ?
                weather.atmosphere.visibility_mi + " mi" : weather.atmosphere.visibility_km + " km";

            // Add UI elements
            Forecasts = new ObservableCollection<ForecastItemViewModel>();
            foreach (Forecast forecast in weather.forecast)
            {
                ForecastItemViewModel forecastView = new ForecastItemViewModel(forecast);
                Forecasts.Add(forecastView);
            }
        }

        private void UpdatePressureState(string state)
        {
            switch (state)
            {
                // Steady
                case "0":
                default:
#if WINDOWS_UWP
                    RisingVisiblity = Visibility.Collapsed;
#elif __ANDROID__
                    RisingVisiblity = ViewStates.Gone;
#endif
                    RisingIcon = string.Empty;
                    break;
                // Rising
                case "1":
                case "+":
#if WINDOWS_UWP
                    RisingVisiblity = Visibility.Visible;
#elif __ANDROID__
                    RisingVisiblity = ViewStates.Visible;
#endif
                    RisingIcon = "\uf058\uf058";
                    break;
                // Falling
                case "2":
                case "-":
#if WINDOWS_UWP
                    RisingVisiblity = Visibility.Visible;
#elif __ANDROID__
                    RisingVisiblity = ViewStates.Visible;
#endif
                    RisingIcon = "\uf044\uf044";
                    break;
            }
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
