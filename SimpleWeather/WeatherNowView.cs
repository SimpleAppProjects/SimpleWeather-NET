using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace SimpleWeather
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
        public Visibility RisingVisiblity { get; set; }
        public string RisingIcon { get; set; }
        public string _Visibility { get; set; }
        public string WindChill { get; set; }
        public Transform WindDirection { get; set; }
        public string WindSpeed { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }

        // Forecast
        public ObservableCollection<ForecastItemView> Forecasts { get; set; }

        // Background
        public ImageBrush Background { get; set; }
        public SolidColorBrush PanelBackground { get; set; }

        public WeatherNowView()
        {
            Background = new ImageBrush();
            Background.Stretch = Stretch.UniformToFill;
            Background.AlignmentX = AlignmentX.Center;
        }

        public WeatherNowView(Weather weather)
        {
            Background = new ImageBrush();
            Background.Stretch = Stretch.UniformToFill;
            Background.AlignmentX = AlignmentX.Center;

            updateView(weather);
        }

        public void updateView(Weather weather)
        {
            // Update backgrounds
            WeatherUtils.SetBackground(Background, weather);
            PanelBackground = new SolidColorBrush(WeatherUtils.isNight(weather) ?
                Windows.UI.Color.FromArgb(15, 128, 128, 128) : Windows.UI.Color.FromArgb(15, 8, 8, 8));

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
            foreach (WeatherData.Forecast forecast in weather.forecast)
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
                    RisingVisiblity = Visibility.Collapsed;
                    RisingIcon = string.Empty;
                    break;
                // Rising
                case "1":
                case "+":
                    RisingVisiblity = Visibility.Visible;
                    RisingIcon = "\uf058\uf058";
                    break;
                // Falling
                case "2":
                case "-":
                    RisingVisiblity = Visibility.Visible;
                    RisingIcon = "\uf044\uf044";
                    break;
            }
        }

        private void updateWindDirection(int angle)
        {
            RotateTransform rotation = new RotateTransform();
            rotation.Angle = angle;
            WindDirection = rotation;
        }
    }
}
