using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using muxc = Microsoft.UI.Xaml.Controls;

namespace SimpleWeather.Icons
{
    public abstract partial class WeatherIconProvider : IWeatherIconsProvider, ISVGWeatherIconProvider
    {
        public abstract Uri GetWeatherIconURI(string icon);
        public abstract String GetWeatherIconURI(string icon, bool isAbsoluteUri, bool isLight = false);
        public abstract String GetSVGIconUri(string icon, bool isLight = false);
    }
}
