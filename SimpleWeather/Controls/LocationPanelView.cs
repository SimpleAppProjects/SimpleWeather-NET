using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace SimpleWeather
{
    public class LocationPanelView
    {
        public string LocationName { get; set; }
        public string CurrTemp { get; set; }
        public string WeatherIcon { get; set; }
        public KeyValuePair<int, Coordinate> Pair { get; set; }

        // Background
        public ImageBrush Background { get; set; }

        public LocationPanelView(Weather weather)
        {
            // Update background
            Background = WeatherUtils.GetBackground(weather);

            LocationName = weather.location.description;
            CurrTemp = weather.condition.temp + "º";
            WeatherIcon = WeatherUtils.GetWeatherIcon(weather.condition.code);
        }
    }
}
