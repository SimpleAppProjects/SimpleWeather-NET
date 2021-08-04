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
            return sIconsProvider.GetWeatherIconURI(icon);
        }

        public String GetWeatherIconURI(String icon, bool isAbsoluteUri, bool isLight = false)
        {
            return sIconsProvider.GetWeatherIconURI(icon, isAbsoluteUri, isLight);
        }

        public bool ShouldUseMonochrome() => ShouldUseMonochrome(Settings.IconProvider);

        public bool ShouldUseMonochrome(string wip)
        {
            switch (wip)
            {
                case "wi-erik-flowers":
                case "wui-ashley-jager":
                case "w-iconic-jackd248":
                case "pixeden-icons_set-weather":
                default:
                    return true;
                case "meteocons-basmilius":
                case "wci_sliu_iconfinder":
                    return false;
            }
        }
    }
}
