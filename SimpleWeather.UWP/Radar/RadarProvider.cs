using Microsoft.Toolkit.Parsers.Core;
using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Radar.NullSchool;
using SimpleWeather.UWP.Radar.OpenWeather;
using SimpleWeather.UWP.Radar.RainViewer;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using static SimpleWeather.UWP.Radar.RadarProviderChangedEventArgs;

namespace SimpleWeather.UWP.Radar
{
    public static class RadarProvider
    {
        private static readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private const string KEY_RADARPROVIDER = "key_radarprovider";

        private const string EARTHWINDMAP = "nullschool";
        private const string RAINVIEWER = "rainviewer";
        private const string OPENWEATHERMAP = "openweather";

        public static event RadarProviderChangedEventHandler RadarProviderChanged;

        public enum RadarProviders
        {
            [StringValue(EARTHWINDMAP)]
            EarthWindMap,
            [StringValue(RAINVIEWER)]
            RainViewer,
            [StringValue(OPENWEATHERMAP)]
            OpenWeatherMap
        }

        private static readonly IReadOnlyList<ProviderEntry> RadarAPIProviders = new List<ProviderEntry>
        {
            new ProviderEntry("EarthWindMap Project", EARTHWINDMAP,
                    "https://earth.nullschool.net/", "https://earth.nullschool.net/"),
            new ProviderEntry("RainViewer", RAINVIEWER,
                    "https://www.rainviewer.com/", "https://www.rainviewer.com/api.html"),
            new ProviderEntry("OpenWeatherMap", OPENWEATHERMAP,
                    "http://www.openweathermap.org", "https://home.openweathermap.org/users/sign_up")
        };

        public static IEnumerable<ProviderEntry> GetRadarProviders()
        {
            var owm = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap);
            if (Settings.API != owm.WeatherAPI && owm.GetAPIKey() == null)
            {
                return RadarAPIProviders.Where(p => p.Value != WeatherAPI.OpenWeatherMap);
            }
            else
            {
                return RadarAPIProviders;
            }
        }

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
                case RadarProviders.OpenWeatherMap:
                    return new OWMRadarViewProvider(radarContainer);
                default:
                case RadarProviders.EarthWindMap:
                    return new EarthWindMapViewProvider(radarContainer);
            }
        }

        public static string GetRadarProvider()
        {
            string provider;

            if (localSettings.Values.TryGetValue(KEY_RADARPROVIDER, out object value))
            {
                provider = value.ToString();
            }
            else
            {
                provider = EARTHWINDMAP;
            }

            if (provider == WeatherAPI.OpenWeatherMap)
            {
                var owm = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap);
                // Fallback to default since API KEY is unavailable
                if ((Settings.API != owm.WeatherAPI && owm.GetAPIKey() == null) || Settings.API_KEY == null)
                {
                    return EARTHWINDMAP;
                }
            }

            return provider;
        }

        private static void SetRadarProvider(RadarProviders value)
        {
            localSettings.Values[KEY_RADARPROVIDER] = value.GetStringValue();
            RadarProviderChanged?.Invoke(new RadarProviderChangedEventArgs() { NewValue = value });
        }
    }
}
