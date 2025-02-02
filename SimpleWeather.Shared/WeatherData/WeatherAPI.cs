using SimpleWeather.Controls;
using SimpleWeather.Preferences;
using System.Collections.Generic;
using System.Linq;

namespace SimpleWeather.WeatherData
{
    public static class WeatherAPI
    {
        // APIs
        public const string Yahoo = "Yahoo";
        public const string WeatherUnderground = "WUnderground";
        public const string OpenWeatherMap = "OpenWeather";
        public const string OpenWeatherMap_OneCall = "openweather_onecall";
        public const string MetNo = "Metno";
        public const string Here = "Here";
        public const string NWS = "NWS";
        public const string WeatherUnlocked = "wunlocked";
        public const string MeteoFrance = "meteofrance";
        public const string TomorrowIo = "tomorrowio";
        public const string AccuWeather = "accuweather";
        public const string WeatherBitIo = "weatherbitio";
        public const string Apple = "apple";
        public const string DWD = "dwd";
        public const string ECCC = "eccc";

        // Location APIs
        public const string BingMaps = "Bing";
        public const string Google = "google";
        public const string WeatherApi = "weatherapi";

        // Radar
        public const string RainViewer = "rainviewer";

        // Misc
        public const string Google_Pollen = "google_pollen";

        /*
         * Note to self: Common steps to adding a new weather provider
         * 1) Implement WeatherProviderImpl class
         * 2) Add constructor for Weather data objects
         * 3) Update LocationQueryViewModel (if needed)
         * 4) Add API to provider list below
         * 5) Add API to WeatherManager
         * 6) Add to remote_config_defaults
         */
        public static IReadOnlyList<ProviderEntry> APIs
        {
            get
            {
                var SettingsManager = DI.Utils.SettingsManager;
                return SettingsManager.DevSettingsEnabled ? DefaultAPIs.Concat(TestingAPIs).ToList() : DefaultAPIs;
            }
        }

        private static readonly IReadOnlyList<ProviderEntry> DefaultAPIs = new List<ProviderEntry>(11)
        {
            new ProviderEntry("HERE Weather", Here,
                "https://www.here.com/en", "https://developer.here.com/?create=Freemium-Basic&keepState=true&step=account"),
            new ProviderEntry("Apple Weather", Apple,
                "https://developer.apple.com/weatherkit/", "https://developer.apple.com/weatherkit/"),
            new ProviderEntry("OpenWeatherMap", OpenWeatherMap,
                "http://www.openweathermap.org", "https://home.openweathermap.org/users/sign_up"),
            new ProviderEntry("WeatherAPI.com", WeatherApi,
                "https://weatherapi.com", "https://weatherapi.com/api"),
            new ProviderEntry("National Weather Service (United States)", NWS,
                "https://www.weather.gov", "https://www.weather.gov"),
            new ProviderEntry("BrightSky (DWD) [Germany]", DWD,
                "https://brightsky.dev/", "https://brightsky.dev/"),
            new ProviderEntry("Environment and Climate Change Canada (ECCC)", ECCC,
                "https://www.weather.gc.ca/", "https://www.weather.gc.ca/canada_e.html"),
            new ProviderEntry("Meteo France", MeteoFrance,
                "https://meteofrance.com/", "https://meteofrance.com/"),
            new ProviderEntry("MET Norway", MetNo,
                "https://www.met.no/en", "https://www.met.no/en"),
            new ProviderEntry("Tomorrow.io", TomorrowIo,
                "https://www.tomorrow.io/weather-api/", "https://www.tomorrow.io/weather-api/"),
            new ProviderEntry("Weatherbit.io", WeatherBitIo,
                "https://www.weatherbit.io/", "https://www.weatherbit.io/pricing"),
            new ProviderEntry("WeatherUnlocked", WeatherUnlocked,
                "https://developer.weatherunlocked.com/", "https://developer.weatherunlocked.com/"),
        };

        public static readonly IReadOnlyList<ProviderEntry> LocationAPIs = new List<ProviderEntry>(2)
        {
            new ProviderEntry("Bing Maps", BingMaps,
                    "https://bing.com/maps", "https://bing.com/maps"),
            new ProviderEntry("WeatherAPI.com", WeatherApi,
                "https://weatherapi.com", "https://weatherapi.com/api"),
            new ProviderEntry("Bing Maps", AccuWeather,
                /* Uses BingMapsLocationProvider | accuweather is used for locationid only */
                "https://bing.com/maps", "https://bing.com/maps"),
            new ProviderEntry("Apple Maps", Apple,
                "https://www.apple.com/maps/", "https://www.apple.com/maps/"),
        };

        private static readonly IReadOnlyList<ProviderEntry> TestingAPIs = new List<ProviderEntry>(2)
        {
            new ProviderEntry("AccuWeather", AccuWeather,
                "https://www.accuweather.com/", "https://developer.accuweather.com/")
        };
    }
}