using Microsoft.Toolkit.Parsers.Core;
using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Radar.NullSchool;
using SimpleWeather.UWP.Radar.RainViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.UWP.Radar
{
    public static class RadarProvider
    {
        private static readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private const string KEY_RADARPROVIDER = "key_radarprovider";

        private const string EARTHWINDMAP = "nullschool";
        private const string RAINVIEWER = "rainviewer";
        private const string OPENWEATHERMAP = "openweather";

        public enum RadarProviders
        {
            [StringValue(EARTHWINDMAP)]
            EarthWindMap,
            [StringValue(RAINVIEWER)]
            RainViewer,
            /*
            [StringValue(OPENWEATHERMAP)]
            OpenWeatherMap
            */
        }

        public static readonly IReadOnlyList<ProviderEntry> RadarAPIProviders = new List<ProviderEntry>
        {
            new ProviderEntry("EarthWindMap Project", EARTHWINDMAP,
                    "https://earth.nullschool.net/", "https://earth.nullschool.net/"),
            new ProviderEntry("RainViewer", RAINVIEWER,
                    "https://www.rainviewer.com/", "https://www.rainviewer.com/api.html"),
            /*
            new ProviderEntry("OpenWeatherMap", OPENWEATHERMAP,
                    "http://www.openweathermap.org", "https://home.openweathermap.org/users/sign_up")
            */
        };

        public static RadarProviders RadarAPIProvider { 
            get 
            {
                var value = GetRadarProvider();
                return Enum.GetValues(typeof(RadarProviders)).Cast<RadarProviders>().FirstOrDefault(@enum => Equals(value, @enum.GetStringValue()));
            }
            set { SetRadarProvider(value); }
        }

        public static RadarViewProvider GetRadarViewProvider(Windows.UI.Xaml.Controls.Border radarContainer)
        {
            switch (RadarAPIProvider)
            {
                case RadarProviders.RainViewer:
                    return new RainViewerViewProvider(radarContainer);
                    /*
                case RadarProviders.OpenWeatherMap:
                    return new OWMRadarViewProvider(radarContainer);
                    */
                default:
                case RadarProviders.EarthWindMap:
                    return new EarthWindMapViewProvider(radarContainer);
            }
        }

        public static string GetRadarProvider()
        {
            if (localSettings.Values.TryGetValue(KEY_RADARPROVIDER, out object value))
            {
                return value.ToString();
            }

            return EARTHWINDMAP;
        }

        private static void SetRadarProvider(RadarProviders value)
        {
            localSettings.Values[KEY_RADARPROVIDER] = value.GetStringValue();
        }
    }
}
