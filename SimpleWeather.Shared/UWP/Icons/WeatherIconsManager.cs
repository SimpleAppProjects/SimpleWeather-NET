using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

namespace SimpleWeather.Icons
{
    public sealed partial class WeatherIconsManager : IWeatherIconsProvider
    {
        public static string GetPNGBaseUri(bool isLight = false)
        {
            return "ms-appx:///SimpleWeather.Shared/Assets/WeatherIcons/png/" + (isLight ? "light/" : "dark/");
        }

        public static string GetSVGBaseUri(bool isLight = false)
        {
            return "ms-appx:///SimpleWeather.Shared/Assets/WeatherIcons/svg/" + (isLight ? "light/" : "dark/");
        }

        public Uri GetWeatherIconURI(String icon)
        {
            return _IconsProvider.GetWeatherIconURI(icon);
        }

        public String GetWeatherIconURI(String icon, bool isAbsoluteUri, bool isLight = false)
        {
            return _IconsProvider.GetWeatherIconURI(icon, isAbsoluteUri, isLight);
        }
    }
}
