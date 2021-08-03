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
        public static string GetBaseUri()
        {
            return "ms-appx:///SimpleWeather.Shared/Assets/WeatherIcons/";
        }

        public static string GetBaseUri(bool isLight)
        {
            return "ms-appx:///SimpleWeather.Shared/Assets/WeatherIcons/" + (isLight ? "light/" : "dark/");
        }

        public Uri GetWeatherIconURI(String icon)
        {
            return sIconsProvider.GetWeatherIconURI(icon);
        }

        public String GetWeatherIconURI(String icon, bool isAbsoluteUri, bool isLight = false)
        {
            return sIconsProvider.GetWeatherIconURI(icon, isAbsoluteUri, isLight);
        }
    }
}
