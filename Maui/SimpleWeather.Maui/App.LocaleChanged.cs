using SimpleWeather.Resources.Strings;
using ResExtras = SimpleWeather.Extras.Resources.Strings.Extras;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui
{
    public partial class App
    {
        private static void UpdateAppLocale()
        {
            var locale = SimpleWeather.Utils.LocaleUtils.GetLocale();

            ResStrings.Culture = locale;
            ResExtras.Culture = locale;
            AQIndex.Culture = locale;
            Beaufort.Culture = locale;
            Config.Culture = locale;
            ConfigiOS.Culture = locale;
            MoonPhases.Culture = locale;
            Units.Culture = locale;
            UVIndex.Culture = locale;
            WeatherConditions.Culture = locale;
            AQIndex.Culture = locale;
        }
    }
}

