using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Parsers.Core;
#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif
using SimpleWeather.Controls;
using SimpleWeather.NET.Radar.OpenWeather;
using SimpleWeather.NET.Radar.RainViewer;
using SimpleWeather.NET.Radar.TomorrowIo;
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
        internal const string KEY_RADARPROVIDER = "key_radarprovider";

        public static event RadarProviderChangedEventHandler RadarProviderChanged;

        public enum RadarProviders
        {
            [StringValue(WeatherAPI.RainViewer)]
            RainViewer,
            [StringValue(WeatherAPI.OpenWeatherMap)]
            OpenWeatherMap,
            [StringValue(WeatherAPI.TomorrowIo)]
            TomorrowIo
        }

        private static readonly IReadOnlyList<ProviderEntry> RadarAPIProviders =
        [
            new ProviderEntry("RainViewer", WeatherAPI.RainViewer,
                    "https://www.rainviewer.com/", "https://www.rainviewer.com/api.html"),
            new ProviderEntry("OpenWeatherMap", WeatherAPI.OpenWeatherMap,
                    "http://www.openweathermap.org", "https://home.openweathermap.org/users/sign_up"),
            new ProviderEntry("Tomorrow.io", WeatherAPI.TomorrowIo,
                    "https://www.tomorrow.io/weather-api/", "https://www.tomorrow.io/weather-api/")
        ];

        public static IEnumerable<ProviderEntry> GetRadarProviders()
        {
            var settingsManager = Ioc.Default.GetService<SettingsManager>();
            IList<string> apiRadarProviders = [WeatherAPI.OpenWeatherMap, WeatherAPI.TomorrowIo];

            IEnumerable<ProviderEntry> providers = RadarAPIProviders;

            apiRadarProviders.ForEach(api =>
            {
                var p = WeatherModule.Instance.WeatherManager.GetWeatherProvider(api);

                if (!Equals(settingsManager.API, p.WeatherAPI) && (string.IsNullOrWhiteSpace(settingsManager.APIKeys[p.WeatherAPI]) && string.IsNullOrWhiteSpace(p.GetAPIKey())))
                {
                    providers = providers.WhereNot(it => Equals(it.Value, p.WeatherAPI));
                }
            });

            return providers;
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
            return RadarAPIProvider switch
            {
                RadarProviders.OpenWeatherMap => new OWMRadarViewProvider(radarContainer),
                RadarProviders.TomorrowIo => new TomorrowIoRadarViewProvider(radarContainer),
                _ => new RainViewerViewProvider(radarContainer),
            };
        }

        public static string GetRadarProvider()
        {
            string provider = localSettings.GetValue(KEY_RADARPROVIDER, defaultValue: WeatherAPI.RainViewer);

            if (provider == WeatherAPI.OpenWeatherMap)
            {
                var owm = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.OpenWeatherMap);
                var SettingsManager = Ioc.Default.GetService<SettingsManager>();

                // Fallback to default since API KEY is unavailable
                if ((SettingsManager.API != owm.WeatherAPI && owm.GetAPIKey() == null) || SettingsManager.APIKeys[WeatherAPI.OpenWeatherMap] == null)
                {
                    return WeatherAPI.RainViewer;
                }
            }
            else if (provider == WeatherAPI.TomorrowIo)
            {
                var tmr = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.TomorrowIo);
                var SettingsManager = Ioc.Default.GetService<SettingsManager>();

                // Fallback to default since API KEY is unavailable
                if ((SettingsManager.API != tmr.WeatherAPI && tmr.GetAPIKey() == null) || SettingsManager.APIKeys[WeatherAPI.TomorrowIo] == null)
                {
                    return WeatherAPI.RainViewer;
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
