using SimpleWeather.Controls;
using SimpleWeather.Utils;
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
        }

        #region Yahoo Weather
        public WeatherNowView(WeatherYahoo.Weather weather)
        {
            updateView(weather);
        }

        private void updateView(WeatherYahoo.Weather weather)
        {
            // Update background
            Background = WeatherUtils.GetBackground(weather);
            PanelBackground = new SolidColorBrush(WeatherUtils.isNight(weather) ?
                Windows.UI.Color.FromArgb(15, 128, 128, 128) : Windows.UI.Color.FromArgb(15, 8, 8, 8));

            // Location
            Location = weather.location.description;

            // Date Updated
            UpdateDate = WeatherUtils.GetLastBuildDate(weather);

            // Update Current Condition
            CurTemp = weather.condition.temp +
                (weather.units.temperature == "F" ? "\uf045" : "\uf03c");
            CurCondition = weather.condition.text;
            WeatherIcon = WeatherUtils.GetWeatherIcon(int.Parse(weather.condition.code));

            // WeatherDetails
            // Astronomy
            Sunrise = weather.astronomy.sunrise;
            Sunset = weather.astronomy.sunset;
            // Wind
            WindChill = (weather.units.temperature == "F" ? weather.wind.chill : ConversionMethods.FtoC(weather.wind.chill)) + "º";
            WindSpeed = weather.wind.speed + " " + weather.units.speed;
            updateWindDirection(int.Parse(weather.wind.direction));

            // Atmosphere
            Humidity = weather.atmosphere.humidity;
            Pressure = (weather.units.temperature == "F" ?
                weather.atmosphere.pressure : Math.Round(double.Parse(weather.atmosphere.pressure)).ToString())
                + " " + weather.units.pressure;
            updatePressureState(int.Parse(weather.atmosphere.rising));
            _Visibility = weather.atmosphere.visibility + " " + weather.units.distance;

            // Add UI elements
            Forecasts = new ObservableCollection<ForecastItemView>();
            foreach (WeatherYahoo.Forecast forecast in weather.forecasts)
            {
                ForecastItemView forecastView = new ForecastItemView(forecast);
                Forecasts.Add(forecastView);
            }
        }
        #endregion

        public WeatherNowView(WeatherUnderground.Weather weather)
        {
            updateView(weather);
        }

        private void updateView(WeatherUnderground.Weather weather)
        {
            // Update backgrounds
            Background = WeatherUtils.GetBackground(weather);
            PanelBackground = new SolidColorBrush(WeatherUtils.isNight(weather) ?
                Windows.UI.Color.FromArgb(15, 128, 128, 128) : Windows.UI.Color.FromArgb(15, 8, 8, 8));

            // Location
            Location = weather.location.full_name;

            // Date Updated
            UpdateDate = WeatherUtils.GetLastBuildDate(weather);

            // Update Current Condition
            CurTemp = Settings.Unit == "F" ?
                Math.Round(weather.condition.temp_f) + "\uf045" : Math.Round(weather.condition.temp_c) + "\uf03c";
            CurCondition = weather.condition.weather;
            WeatherIcon = WeatherUtils.GetWeatherIcon(weather.condition.icon_url);

            // WeatherDetails
            // Astronomy
            Sunrise = DateTime.Parse(weather.sun_phase.sunrise.hour + ":" + weather.sun_phase.sunrise.minute).ToString("hh:mm tt");
            Sunset = DateTime.Parse(weather.sun_phase.sunset.hour + ":" + weather.sun_phase.sunset.minute).ToString("hh:mm tt");
            // Wind
            WindChill = Settings.Unit == "F" ?
                Math.Round(weather.condition.feelslike_f) + "º" : Math.Round(weather.condition.feelslike_c) + "º";
            WindSpeed = Settings.Unit == "F" ?
                weather.condition.wind_mph.ToString() + " mph" : weather.condition.wind_kph.ToString() + " kph";
            updateWindDirection(weather.condition.wind_degrees);

            // Atmosphere
            Humidity = weather.condition.relative_humidity;
            Pressure = Settings.Unit == "F" ?
                weather.condition.pressure_in + " in" : weather.condition.pressure_mb + " mb";

            if (weather.condition.pressure_trend == "+")
                updatePressureState(1);
            else if (weather.condition.pressure_trend == "-")
                updatePressureState(2);
            else
                updatePressureState(0);

            _Visibility = Settings.Unit == "F" ? 
                weather.condition.visibility_mi + " mi" : weather.condition.visibility_km + " km";

            // Add UI elements
            Forecasts = new ObservableCollection<ForecastItemView>();
            foreach (WeatherUnderground.Forecastday1 forecast in weather.forecast.forecastday)
            {
                ForecastItemView forecastView = new ForecastItemView(forecast);
                Forecasts.Add(forecastView);
            }
        }

        private void updatePressureState(int rising)
        {
            switch (rising)
            {
                // Steady
                case 0:
                default:
                    RisingVisiblity = Visibility.Collapsed;
                    RisingIcon = string.Empty;
                    break;
                // Rising
                case 1:
                    RisingVisiblity = Visibility.Visible;
                    RisingIcon = "\uf058\uf058";
                    break;
                // Falling
                case 2:
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
