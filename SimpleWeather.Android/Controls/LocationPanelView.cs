using System;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SimpleWeather.Droid.Utils;

namespace SimpleWeather.Droid.Controls
{
    public class LocationPanelView
    {
        public string LocationName { get; set; }
        public string CurrTemp { get; set; }
        public string WeatherIcon { get; set; }
        public Pair<int, string> Pair { get; set; }

        // Background
        public System.IO.Stream Background { get; set; }

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
            Background = WeatherUtils.GetBackgroundStream(weather);

            LocationName = weather.location.name;
            CurrTemp = (Settings.Unit == "F" ?
                Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º";
            WeatherIcon = WeatherUtils.GetWeatherIcon(weather.condition.icon);
        }
    }
}