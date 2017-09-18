using System;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SimpleWeather.Droid.Utils;
using Android.Graphics;

namespace SimpleWeather.Droid.Controls
{
    public class LocationPanelViewModel
    {
        public string LocationName { get; set; }
        public string CurrTemp { get; set; }
        public string WeatherIcon { get; set; }
        public string Background { get; set; }
        public LocationData LocationData { get; set; }
        public string WeatherSource { get; set; }

        public bool EditMode { get; set; } = false;

        public LocationPanelViewModel()
        {
        }

        public LocationPanelViewModel(Weather weather)
        {
            SetWeather(weather);
        }

        public void SetWeather(Weather weather)
        {
            // Update background
            Background = WeatherUtils.GetBackgroundURI(weather);

            LocationName = weather.location.name;
            CurrTemp = (Settings.Unit == Settings.Fahrenheit ?
                Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º";
            WeatherIcon = WeatherUtils.GetWeatherIcon(weather.condition.icon);
            WeatherSource = weather.source;

            if (LocationData == null)
                LocationData = new LocationData(weather.query);
        }
    }
}