using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.ObjectModel;
#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#elif __ANDROID__
using Android.Graphics;
using Android.Views;
#endif

namespace SimpleWeather.Controls
{
    public class WeatherNowView
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
        public ObservableCollection<ForecastItemView> Forecasts { get; set; }

        // Background
#if WINDOWS_UWP
        public ImageBrush Background { get; set; }
        public SolidColorBrush PanelBackground { get; set; }
#elif __ANDROID__
        public System.IO.Stream Background { get; set; }
        public Color PanelBackground { get; set; }
#endif

        public WeatherNowView()
        {
#if WINDOWS_UWP
            Background = new ImageBrush();
            Background.Stretch = Stretch.UniformToFill;
            Background.AlignmentX = AlignmentX.Center;
#endif
        }

        public WeatherNowView(Weather weather)
        {
#if WINDOWS_UWP
            Background = new ImageBrush();
            Background.Stretch = Stretch.UniformToFill;
            Background.AlignmentX = AlignmentX.Center;
#endif

            updateView(weather);
        }

        public void updateView(Weather weather)
        {
            // Update backgrounds
#if WINDOWS_UWP
            WeatherUtils.SetBackground(Background, weather);
            PanelBackground = new SolidColorBrush(WeatherUtils.isNight(weather) ?
                Windows.UI.Color.FromArgb(15, 128, 128, 128) : Windows.UI.Color.FromArgb(15, 8, 8, 8));
#elif __ANDROID__
            Background = WeatherUtils.GetBackgroundStream(weather);
            PanelBackground = WeatherUtils.isNight(weather) ? new Color(15, 128, 128, 128) : new Color(15, 8, 8, 8);
#endif

            // Location
            Location = weather.location.name;

            // Date Updated
            UpdateDate = WeatherUtils.GetLastBuildDate(weather);

            // Update Current Condition
            CurTemp = Settings.Unit == "F" ?
                Math.Round(weather.condition.temp_f) + "\uf045" : Math.Round(weather.condition.temp_c) + "\uf03c";
            CurCondition = weather.condition.weather;
            WeatherIcon = WeatherUtils.GetWeatherIcon(weather.condition.icon);

            // WeatherDetails
            // Astronomy
            Sunrise = weather.astronomy.sunrise.ToString("h:mm tt");
            Sunset = weather.astronomy.sunset.ToString("h:mm tt");

            // Wind
            WindChill = Settings.Unit == "F" ?
                Math.Round(weather.condition.feelslike_f) + "º" : Math.Round(weather.condition.feelslike_c) + "º";
            WindSpeed = Settings.Unit == "F" ?
                weather.condition.wind_mph.ToString() + " mph" : weather.condition.wind_kph.ToString() + " kph";
            updateWindDirection(weather.condition.wind_degrees);

            // Atmosphere
            Humidity = weather.atmosphere.humidity;
            Pressure = Settings.Unit == "F" ?
                weather.atmosphere.pressure_in + " in" : weather.atmosphere.pressure_mb + " mb";
            updatePressureState(weather.atmosphere.pressure_trend);
            _Visibility = Settings.Unit == "F" ?
                weather.atmosphere.visibility_mi + " mi" : weather.atmosphere.visibility_km + " km";

            // Add UI elements
            Forecasts = new ObservableCollection<ForecastItemView>();
            foreach (Forecast forecast in weather.forecast)
            {
                ForecastItemView forecastView = new ForecastItemView(forecast);
                Forecasts.Add(forecastView);
            }
        }

        private void updatePressureState(string state)
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

        private void updateWindDirection(int angle)
        {
#if WINDOWS_UWP
            RotateTransform rotation = new RotateTransform();
            rotation.Angle = angle;
            WindDirection = rotation;
#elif __ANDROID__
            WindDirection = angle;
#endif
        }
    }
}
