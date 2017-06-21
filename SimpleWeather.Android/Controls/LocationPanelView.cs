using System;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SimpleWeather.Droid.Utils;
using Android.Graphics;

namespace SimpleWeather.Droid.Controls
{
    public class LocationPanelView
    {
        public string LocationName { get; set; }
        public string CurrTemp { get; set; }
        public string WeatherIcon { get; set; }
        public Pair<int, string> Pair { get; set; }
        public string Background { get; set; }

        public bool IsHome { get; set; } = false;
        public bool EditMode { get; set; } = false;

        public LocationPanelView()
        {
        }

        public LocationPanelView(Weather weather)
        {
            setWeather(weather);
        }

        public void setWeather(Weather weather)
        {
            // Update background
            Background = WeatherUtils.GetBackgroundURI(weather);

            LocationName = weather.location.name;
            CurrTemp = (Settings.Unit == "F" ?
                Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º";
            WeatherIcon = WeatherUtils.GetWeatherIcon(weather.condition.icon);
        }
    }
}