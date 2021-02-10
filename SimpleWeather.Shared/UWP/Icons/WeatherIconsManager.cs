using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Icons
{
    public sealed partial class WeatherIconsManager : IWeatherIconsProvider
    {
        public Uri GetWeatherIconURI(String icon)
        {
            return sIconsProvider.GetWeatherIconURI(icon);
        }

        public String GetWeatherIconURI(String icon, bool isAbsoluteUri)
        {
            return sIconsProvider.GetWeatherIconURI(icon, isAbsoluteUri);
        }
    }
}
