using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Icons
{
    public abstract partial class WeatherIconProvider : IWeatherIconsProvider
    {
        public abstract Uri GetWeatherIconURI(string icon);
        public abstract String GetWeatherIconURI(string icon, bool isAbsoluteUri, bool isLight = false);
    }
}
