using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Parsers.Core;
using SimpleWeather.Controls;
using SimpleWeather.Preferences;
#if !HAS_UNO_SKIA
using SimpleWeather.Uno.Radar.NullSchool;
#endif
using SimpleWeather.Uno.Radar.OpenWeather;
using SimpleWeather.Uno.Radar.RainViewer;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using static SimpleWeather.Uno.Radar.RadarProviderChangedEventArgs;

namespace SimpleWeather.Uno.Radar
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
#if !HAS_UNO_SKIA
            [StringValue(EARTHWINDMAP)]
            EarthWindMap,
#endif
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
            var owm = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.OpenWeatherMap);
            var SettingsManager = Ioc.Default.GetService<SettingsManager>();

            if (SettingsManager.API != owm.WeatherAPI && owm.GetAPIKey() == null)
            {
                return RadarAPIProviders.Where(p => p.Value != WeatherAPI.OpenWeatherMap);
            }
            else
            {
                return RadarAPIProviders;
            }
        }

        public static RadarProviders RadarAPIProvider
        {
            get
            {
                var value = GetRadarProvider();
                return Enum.GetValues(typeof(RadarProviders)).Cast<RadarProviders>().FirstOrDefault(@enum => Equals(value, @enum.GetStringValue()));
            }
            set { SetRadarProvider(value); }
        }

        public static RadarViewProvider GetRadarViewProvider(Microsoft.UI.Xaml.Controls.Border radarContainer)
        {
            switch (RadarAPIProvider)
            {
#if HAS_UNO_SKIA
                default:
#endif
                case RadarProviders.RainViewer:
                    return new RainViewerViewProvider(radarContainer);
                case RadarProviders.OpenWeatherMap:
                    return new OWMRadarViewProvider(radarContainer);
#if !HAS_UNO_SKIA
                default:
                case RadarProviders.EarthWindMap:
                    return new EarthWindMapViewProvider(radarContainer);
#endif
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
                var owm = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.OpenWeatherMap);
                var SettingsManager = Ioc.Default.GetService<SettingsManager>();

                // Fallback to default since API KEY is unavailable
                if ((SettingsManager.API != owm.WeatherAPI && owm.GetAPIKey() == null) || SettingsManager.APIKeys[WeatherAPI.OpenWeatherMap] == null)
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
