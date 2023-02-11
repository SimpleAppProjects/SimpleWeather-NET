using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Parsers.Core;
#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif
using SimpleWeather.Controls;
using SimpleWeather.NET.Radar.NullSchool;
using SimpleWeather.NET.Radar.OpenWeather;
using SimpleWeather.NET.Radar.RainViewer;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.WeatherData;
using static SimpleWeather.NET.Radar.RadarProviderChangedEventArgs;

namespace SimpleWeather.NET.Radar
{
    public static class RadarProvider
    {
        private static readonly SettingsContainer localSettings = new SettingsContainer();
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

        public static RadarViewProvider GetRadarViewProvider(Border radarContainer)
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

            if (localSettings.GetValue<string>(KEY_RADARPROVIDER) is string value && value != null)
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
            localSettings.SetValue(KEY_RADARPROVIDER, value.GetStringValue());
            RadarProviderChanged?.Invoke(new RadarProviderChangedEventArgs() { NewValue = value });
        }
    }
}
