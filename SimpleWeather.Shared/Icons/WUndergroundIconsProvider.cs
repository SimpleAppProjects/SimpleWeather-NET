using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Icons
{
    internal sealed class WUndergroundIconsProvider : WeatherIconProvider
    {
        public override string Key => "wui-ashley-jager";
        public override string DisplayName => "WeatherUnderground Icons";

        public override string AuthorName => "Ashley Jager";

        public override Uri AttributionLink => new Uri("https://github.com/manifestinteractive/weather-underground-icons");

        public override bool IsFontIcon => false;

        public override Uri GetWeatherIconURI(string icon)
        {
            return new Uri(GetWeatherIconURI(icon, true));
        }

        public override String GetWeatherIconURI(string icon, bool isAbsoluteUri, bool isLight = false)
        {
            string baseuri = WeatherIconsManager.GetBaseUri(isLight);
            string fileIcon = string.Empty;

            switch (icon)
            {
                // Day
                case WeatherIcons.DAY_SUNNY:
                case WeatherIcons.DAY_HOT:
                case WeatherIcons.DAY_LIGHT_WIND:
                    fileIcon = "wui-clear.png";
                    break;

                case WeatherIcons.DAY_CLOUDY:
                case WeatherIcons.DAY_CLOUDY_GUSTS:
                case WeatherIcons.DAY_CLOUDY_WINDY:
                case WeatherIcons.DAY_CLOUDY_HIGH:
                    fileIcon = "wui-mostlycloudy.png";
                    break;

                case WeatherIcons.DAY_SUNNY_OVERCAST:
                    fileIcon = "wui-partlycloudy.png";
                    break;

                // Night
                case WeatherIcons.NIGHT_CLEAR:
                    fileIcon = "wui-nt_clear.png";
                    break;

                case WeatherIcons.NIGHT_ALT_CLOUDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS:
                case WeatherIcons.NIGHT_ALT_CLOUDY_WINDY:
                    fileIcon = "wui-nt_mostlycloudy.png";
                    break;

                case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY_HIGH:
                    fileIcon = "wui-nt_partlycloudy.png";
                    break;

                // Neutral
                case WeatherIcons.DAY_FOG:
                case WeatherIcons.DAY_HAZE:
                case WeatherIcons.NIGHT_FOG:
                case WeatherIcons.FOG:
                    fileIcon = "wui-fog.png";
                    break;

                case WeatherIcons.DAY_LIGHTNING:
                case WeatherIcons.NIGHT_ALT_LIGHTNING:
                case WeatherIcons.LIGHTNING:
                    fileIcon = "wui-chancetstorms.png";
                    break;

                case WeatherIcons.DAY_HAIL:
                case WeatherIcons.DAY_SLEET:
                case WeatherIcons.DAY_SLEET_STORM:
                case WeatherIcons.NIGHT_ALT_HAIL:
                case WeatherIcons.NIGHT_ALT_SLEET:
                case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                case WeatherIcons.SLEET:
                    fileIcon = "wui-sleet.png";
                    break;

                case WeatherIcons.DAY_RAIN_MIX:
                case WeatherIcons.NIGHT_ALT_RAIN_MIX:
                case WeatherIcons.RAIN_MIX:
                case WeatherIcons.HAIL:
                    fileIcon = "wui-chancesleet.png";
                    break;

                case WeatherIcons.DAY_RAIN:
                case WeatherIcons.DAY_RAIN_WIND:
                case WeatherIcons.DAY_SHOWERS:
                case WeatherIcons.NIGHT_ALT_RAIN:
                case WeatherIcons.NIGHT_ALT_RAIN_WIND:
                case WeatherIcons.NIGHT_ALT_SHOWERS:
                case WeatherIcons.RAIN:
                case WeatherIcons.RAIN_WIND:
                case WeatherIcons.SHOWERS:
                    fileIcon = "wui-rain.png";
                    break;

                case WeatherIcons.DAY_SNOW:
                case WeatherIcons.DAY_SNOW_THUNDERSTORM:
                case WeatherIcons.DAY_SNOW_WIND:
                case WeatherIcons.NIGHT_ALT_SNOW:
                case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                case WeatherIcons.SNOW:
                case WeatherIcons.SNOW_WIND:
                case WeatherIcons.SNOWFLAKE_COLD:
                    fileIcon = "wui-snow.png";
                    break;

                case WeatherIcons.DAY_SPRINKLE:
                case WeatherIcons.NIGHT_ALT_SPRINKLE:
                case WeatherIcons.SPRINKLE:
                    fileIcon = "wui-chancerain.png";
                    break;

                case WeatherIcons.DAY_STORM_SHOWERS:
                case WeatherIcons.DAY_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                case WeatherIcons.STORM_SHOWERS:
                case WeatherIcons.THUNDERSTORM:
                    fileIcon = "wui-tstorms.png";
                    break;

                case WeatherIcons.DAY_WINDY:
                    fileIcon = "wi-day-windy.png";
                    break;

                case WeatherIcons.CLOUD:
                case WeatherIcons.CLOUDY:
                case WeatherIcons.CLOUDY_GUSTS:
                case WeatherIcons.CLOUDY_WINDY:
                    fileIcon = "wui-cloudy.png";
                    break;

                case WeatherIcons.SMOG:
                    fileIcon = "wi-smog.png";
                    break;

                case WeatherIcons.SMOKE:
                    fileIcon = "wi-smoke.png";
                    break;

                case WeatherIcons.DUST:
                    fileIcon = "wi-dust.png";
                    break;

                case WeatherIcons.WINDY:
                    fileIcon = "wi-windy.png";
                    break;

                case WeatherIcons.STRONG_WIND:
                    fileIcon = "wi-strong-wind.png";
                    break;

                case WeatherIcons.SANDSTORM:
                    fileIcon = "wi-sandstorm.png";
                    break;

                case WeatherIcons.HURRICANE:
                    fileIcon = "wi-hurricane.png";
                    break;

                case WeatherIcons.TORNADO:
                    fileIcon = "wi-tornado.png";
                    break;

                case WeatherIcons.FIRE:
                    fileIcon = "wi-fire.png";
                    break;

                case WeatherIcons.FLOOD:
                    fileIcon = "wi-flood.png";
                    break;

                case WeatherIcons.VOLCANO:
                    fileIcon = "wi-volcano.png";
                    break;

                case WeatherIcons.BAROMETER:
                    fileIcon = "wi-barometer.png";
                    break;

                case WeatherIcons.HUMIDITY:
                    fileIcon = "wi-humidity.png";
                    break;

                case WeatherIcons.MOONRISE:
                    fileIcon = "wi-moonrise.png";
                    break;

                case WeatherIcons.MOONSET:
                    fileIcon = "wi-moonset.png";
                    break;

                case WeatherIcons.RAINDROP:
                    fileIcon = "wi-raindrop.png";
                    break;

                case WeatherIcons.RAINDROPS:
                    fileIcon = "wi-raindrops.png";
                    break;

                case WeatherIcons.SUNRISE:
                    fileIcon = "wi-sunrise.png";
                    break;

                case WeatherIcons.SUNSET:
                    fileIcon = "wi-sunset.png";
                    break;

                case WeatherIcons.THERMOMETER:
                    fileIcon = "wi-thermometer.png";
                    break;

                case WeatherIcons.UMBRELLA:
                    fileIcon = "wi-umbrella.png";
                    break;

                case WeatherIcons.WIND_DIRECTION:
                    fileIcon = "wi-wind-direction.png";
                    break;

                case WeatherIcons.DIRECTION_UP:
                    fileIcon = "wi-direction-up.png";
                    break;

                case WeatherIcons.DIRECTION_DOWN:
                    fileIcon = "wi-direction-down.png";
                    break;

                // Beaufort
                case WeatherIcons.WIND_BEAUFORT_0:
                    fileIcon = "wi-wind-beaufort-0.png";
                    break;

                case WeatherIcons.WIND_BEAUFORT_1:
                    fileIcon = "wi-wind-beaufort-1.png";
                    break;

                case WeatherIcons.WIND_BEAUFORT_2:
                    fileIcon = "wi-wind-beaufort-2.png";
                    break;

                case WeatherIcons.WIND_BEAUFORT_3:
                    fileIcon = "wi-wind-beaufort-3.png";
                    break;

                case WeatherIcons.WIND_BEAUFORT_4:
                    fileIcon = "wi-wind-beaufort-4.png";
                    break;

                case WeatherIcons.WIND_BEAUFORT_5:
                    fileIcon = "wi-wind-beaufort-5.png";
                    break;

                case WeatherIcons.WIND_BEAUFORT_6:
                    fileIcon = "wi-wind-beaufort-6.png";
                    break;

                case WeatherIcons.WIND_BEAUFORT_7:
                    fileIcon = "wi-wind-beaufort-7.png";
                    break;

                case WeatherIcons.WIND_BEAUFORT_8:
                    fileIcon = "wi-wind-beaufort-8.png";
                    break;

                case WeatherIcons.WIND_BEAUFORT_9:
                    fileIcon = "wi-wind-beaufort-9.png";
                    break;

                case WeatherIcons.WIND_BEAUFORT_10:
                    fileIcon = "wi-wind-beaufort-10.png";
                    break;

                case WeatherIcons.WIND_BEAUFORT_11:
                    fileIcon = "wi-wind-beaufort-11.png";
                    break;

                case WeatherIcons.WIND_BEAUFORT_12:
                    fileIcon = "wi-wind-beaufort-12.png";
                    break;

                // Moon Phase
                case WeatherIcons.MOON_NEW:
                    fileIcon = "wi-moon-new.png";
                    break;

                case WeatherIcons.MOON_WAXING_CRESCENT_3:
                    fileIcon = "wi-moon-waxing-crescent-3.png";
                    break;

                case WeatherIcons.MOON_FIRST_QUARTER:
                    fileIcon = "wi-moon-first-quarter.png";
                    break;

                case WeatherIcons.MOON_WAXING_GIBBOUS_3:
                    fileIcon = "wi-moon-waxing-gibbous-3.png";
                    break;

                case WeatherIcons.MOON_FULL:
                    fileIcon = "wi-moon-full.png";
                    break;

                case WeatherIcons.MOON_WANING_GIBBOUS_3:
                    fileIcon = "wi-moon-waning-gibbous-3.png";
                    break;

                case WeatherIcons.MOON_THIRD_QUARTER:
                    fileIcon = "wi-moon-third-quarter.png";
                    break;

                case WeatherIcons.MOON_WANING_CRESCENT_3:
                    fileIcon = "wi-moon-waning-crescent-3.png";
                    break;

                case WeatherIcons.MOON_ALT_NEW:
                    fileIcon = "wi-moon-alt-new.png";
                    break;

                case WeatherIcons.MOON_ALT_WAXING_CRESCENT_3:
                    fileIcon = "wi-moon-alt-waxing-crescent-3.png";
                    break;

                case WeatherIcons.MOON_ALT_FIRST_QUARTER:
                    fileIcon = "wi-moon-alt-first-quarter.png";
                    break;

                case WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3:
                    fileIcon = "wi-moon-alt-waxing-gibbous-3.png";
                    break;

                case WeatherIcons.MOON_ALT_FULL:
                    fileIcon = "wi-moon-alt-full.png";
                    break;

                case WeatherIcons.MOON_ALT_WANING_GIBBOUS_3:
                    fileIcon = "wi-moon-alt-waning-gibbous-3.png";
                    break;

                case WeatherIcons.MOON_ALT_THIRD_QUARTER:
                    fileIcon = "wi-moon-alt-third-quarter.png";
                    break;

                case WeatherIcons.MOON_ALT_WANING_CRESCENT_3:
                    fileIcon = "wi-moon-alt-waning-crescent-3.png";
                    break;

                case WeatherIcons.FAHRENHEIT:
                    fileIcon = "wi-fahrenheit.png";
                    break;

                case WeatherIcons.CELSIUS:
                    fileIcon = "wi-celsius.png";
                    break;

                case WeatherIcons.NA:
                    fileIcon = "wui-unknown.png";
                    break;
            }

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "wui-unknown.png";
            }

            if (isAbsoluteUri)
            {
                return baseuri + fileIcon;
            }
            else
            {
                return fileIcon;
            }
        }
    }
}
