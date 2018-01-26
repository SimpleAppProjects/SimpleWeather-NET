using System;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SimpleWeather.Droid.Utils;
using Android.Graphics;

namespace SimpleWeather.Droid.Controls
{
    public class LocationPanelViewModel
    {
        private WeatherManager wm;

        public string LocationName { get; set; }
        public string CurrTemp { get; set; }
        public string WeatherIcon { get; set; }
        public string Background { get; set; }
        public LocationData LocationData { get; set; }
        public string WeatherSource { get; set; }

        public bool EditMode { get; set; } = false;

        public LocationPanelViewModel()
        {
            wm = WeatherManager.GetInstance();
        }

        public LocationPanelViewModel(Weather weather)
        {
            wm = WeatherManager.GetInstance();
            SetWeather(weather);
        }

        public void SetWeather(Weather weather)
        {
            // Update background
            Background = wm.GetBackgroundURI(weather);

            LocationName = weather.location.name;
            CurrTemp = (Settings.IsFahrenheit ?
                Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º";
            WeatherIcon = weather.condition.icon;
            WeatherSource = weather.source;

            if (LocationData == null)
                LocationData = new LocationData()
                {
                    query = weather.query,
                    name = weather.location.name,
                    latitude = double.Parse(weather.location.latitude),
                    longitude = double.Parse(weather.location.longitude),
                    tz_long = weather.location.tz_long
                };
        }
    }
}