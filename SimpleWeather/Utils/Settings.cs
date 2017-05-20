namespace SimpleWeather.Utils
{
    public static partial class Settings
    {
        public static bool WeatherLoaded { get { return isWeatherLoaded(); } set { setWeatherLoaded(value); } }
        public static string Unit
        {
            get { return getTempUnit(); }
#if WINDOWS_UWP
            set { setTempUnit(value); }
#endif
        }
        public static string API { get { return getAPI(); } set { setAPI(value); } }
        public static string API_KEY { get { return getAPIKEY(); } set { setAPIKEY(value); } }

        private static string Fahrenheit = "F";
        private static string Celsius = "C";
    }
}
