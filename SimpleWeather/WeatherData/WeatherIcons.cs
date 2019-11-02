using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.WeatherData
{
    public static class WeatherIcons
    {
        // DAY
        public const String DAY_SUNNY = "\uf00d";
        public const String DAY_CLOUDY = "\uf002";
        public const String DAY_CLOUDY_GUSTS = "\uf000";
        public const String DAY_CLOUDY_WINDY = "\uf001";
        public const String DAY_FOG = "\uf003";
        public const String DAY_HAIL = "\uf004";
        public const String DAY_HAZE = "\uf0b6";
        public const String DAY_LIGHTNING = "\uf005";
        public const String DAY_RAIN = "\uf008";
        public const String DAY_RAIN_MIX = "\uf006";
        public const String DAY_RAIN_WIND = "\uf007";
        public const String DAY_SHOWERS = "\uf009";
        public const String DAY_SLEET = "\uf0b2";
        public const String DAY_SLEET_STORM = "\uf068";
        public const String DAY_SNOW = "\uf00a";
        public const String DAY_SNOW_THUNDERSTORM = "\uf06b";
        public const String DAY_SNOW_WIND = "\uf065";
        public const String DAY_SPRINKLE = "\uf00b";
        public const String DAY_STORM_SHOWERS = "\uf00e";
        public const String DAY_SUNNY_OVERCAST = "\uf00c";
        public const String DAY_THUNDERSTORM = "\uf010";
        public const String DAY_WINDY = "\uf085";
        public const String DAY_HOT = "\uf072";
        public const String DAY_CLOUDY_HIGH = "\uf07d";
        public const String DAY_LIGHT_WIND = "\uf0c4";

        // NIGHT_ALT
        public const String NIGHT_CLEAR = "\uf02e";
        public const String NIGHT_ALT_CLOUDY = "\uf086";
        public const String NIGHT_ALT_CLOUDY_GUSTS = "\uf022";
        public const String NIGHT_ALT_CLOUDY_WINDY = "\uf023";
        public const String NIGHT_ALT_HAIL = "\uf024";
        public const String NIGHT_ALT_LIGHTNING = "\uf025";
        public const String NIGHT_ALT_RAIN = "\uf028";
        public const String NIGHT_ALT_RAIN_MIX = "\uf026";
        public const String NIGHT_ALT_RAIN_WIND = "\uf027";
        public const String NIGHT_ALT_SHOWERS = "\uf029";
        public const String NIGHT_ALT_SLEET = "\uf0b4";
        public const String NIGHT_ALT_SLEET_STORM = "\uf06a";
        public const String NIGHT_ALT_SNOW = "\uf02a";
        public const String NIGHT_ALT_SNOW_THUNDERSTORM = "\uf06d";
        public const String NIGHT_ALT_SNOW_WIND = "\uf067";
        public const String NIGHT_ALT_SPRINKLE = "\uf02b";
        public const String NIGHT_ALT_STORM_SHOWERS = "\uf02c";
        public const String NIGHT_ALT_THUNDERSTORM = "\uf02d";
        public const String NIGHT_ALT_PARTLY_CLOUDY = "\uf081";
        public const String NIGHT_ALT_CLOUDY_HIGH = "\uf07e";

        // NIGHT
        public const String NIGHT_FOG = "\uf04a";

        // NEUTRAL
        public const String CLOUD = "\uf041";
        public const String CLOUDY = "\uf013";
        public const String CLOUDY_GUSTS = "\uf011";
        public const String CLOUDY_WINDY = "\uf012";
        public const String FOG = "\uf014";
        public const String HAIL = "\uf015";
        public const String RAIN = "\uf019";
        public const String RAIN_MIX = "\uf017";
        public const String RAIN_WIND = "\uf018";
        public const String SHOWERS = "\uf01a";
        public const String SLEET = "\uf0b5";
        public const String SNOW = "\uf01b";
        public const String SPRINKLE = "\uf01c";
        public const String STORM_SHOWERS = "\uf01d";
        public const String THUNDERSTORM = "\uf01e";
        public const String SNOW_WIND = "\uf064";
        public const String SMOG = "\uf074";
        public const String SMOKE = "\uf062";
        public const String LIGHTNING = "\uf016";
        public const String DUST = "\uf063";
        public const String SNOWFLAKE_COLD = "\uf076";
        public const String WINDY = "\uf021";
        public const String STRONG_WIND = "\uf050";
        public const String SANDSTORM = "\uf082";
        public const String HURRICANE = "\uf073";
        public const String TORNADO = "\uf056";

        public const String FIRE = "\uf0c7";
        public const String FLOOD = "\uf07c";
        public const String VOLCANO = "\uf0c8";

        public const String BAROMETER = "\uf079";
        public const String HUMIDITY = "\uf07a";
        public const String MOONRISE = "\uf0c9";
        public const String MOONSET = "\uf0ca";
        public const String RAINDROP = "\uf078";
        public const String RAINDROPS = "\uf04e";
        public const String SUNRISE = "\uf051";
        public const String SUNSET = "\uf052";
        public const String THERMOMETER = "\uf055";
        public const String UMBRELLA = "\uf084";
        public const String WIND_DIRECTION = "\uf0b1";

        // Beaufort
        public const String WIND_BEAUFORT_0 = "\uf0b7";
        public const String WIND_BEAUFORT_1 = "\uf0b8";
        public const String WIND_BEAUFORT_2 = "\uf0b9";
        public const String WIND_BEAUFORT_3 = "\uf0ba";
        public const String WIND_BEAUFORT_4 = "\uf0bb";
        public const String WIND_BEAUFORT_5 = "\uf0bc";
        public const String WIND_BEAUFORT_6 = "\uf0bd";
        public const String WIND_BEAUFORT_7 = "\uf0be";
        public const String WIND_BEAUFORT_8 = "\uf0bf";
        public const String WIND_BEAUFORT_9 = "\uf0c0";
        public const String WIND_BEAUFORT_10 = "\uf0c1";
        public const String WIND_BEAUFORT_11 = "\uf0c2";
        public const String WIND_BEAUFORT_12 = "\uf0c3";

        // Moon Phase
        public const String MOON_ALT_NEW = "\uf0eb";
        public const String MOON_ALT_WAXING_CRESCENT_3 = "\uf0d2";
        public const String MOON_ALT_FIRST_QUARTER = "\uf0d6";
        public const String MOON_ALT_WAXING_GIBBOUS_3 = "\uf0d9";
        public const String MOON_ALT_FULL = "\uf0dd";
        public const String MOON_ALT_WANING_GIBBOUS_3 = "\uf0e0";
        public const String MOON_ALT_THIRD_QUARTER = "\uf0e4";
        public const String MOON_ALT_WANING_CRESCENT_3 = "\uf0e7";

        public const String FAHRENHEIT = "\uf045";
        public const String CELSIUS = "\uf03c";

        // N/A
        public const String NA = "\uf07b";
    }
}
