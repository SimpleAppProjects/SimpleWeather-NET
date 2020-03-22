using SimpleWeather.Controls;
using SimpleWeather.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Main
{
    internal class WeatherPageArgs
    {
        public LocationData Location { get; set; }
        public WeatherNowViewModel WeatherNowView { get; set; }
    }
}
