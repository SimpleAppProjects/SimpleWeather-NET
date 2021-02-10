#if WINDOWS_UWP
using SimpleWeather.Icons;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Icons
{
    public partial class WeatherIconsProvider : WeatherIconProvider
    {
        public override Uri GetWeatherIconURI(string icon)
        {
            return new Uri(GetWeatherIconURI(icon, true));
        }

        public override String GetWeatherIconURI(string icon, bool isAbsoluteUri)
        {
            string baseuri = "ms-appx:///SimpleWeather.Shared/Assets/WeatherIcons/png/";
            string fileIcon = string.Empty;

            switch (icon)
            {
                // Day
                case WeatherIcons.DAY_SUNNY:
                    fileIcon = "wi-day-sunny.png";
                    break;

                case WeatherIcons.DAY_CLOUDY:
                    fileIcon = "wi-day-cloudy.png";
                    break;

                case WeatherIcons.DAY_CLOUDY_GUSTS:
                    fileIcon = "wi-day-cloudy-gusts.png";
                    break;

                case WeatherIcons.DAY_CLOUDY_WINDY:
                    fileIcon = "wi-day-cloudy-windy.png";
                    break;

                case WeatherIcons.DAY_FOG:
                    fileIcon = "wi-day-fog.png";
                    break;

                case WeatherIcons.DAY_HAIL:
                    fileIcon = "wi-day-hail.png";
                    break;

                case WeatherIcons.DAY_HAZE:
                    fileIcon = "wi-day-haze.png";
                    break;

                case WeatherIcons.DAY_LIGHTNING:
                    fileIcon = "wi-day-lightning.png";
                    break;

                case WeatherIcons.DAY_RAIN:
                    fileIcon = "wi-day-rain.png";
                    break;

                case WeatherIcons.DAY_RAIN_MIX:
                    fileIcon = "wi-day-rain-mix.png";
                    break;

                case WeatherIcons.DAY_RAIN_WIND:
                    fileIcon = "wi-day-rain-wind.png";
                    break;

                case WeatherIcons.DAY_SHOWERS:
                    fileIcon = "wi-day-showers.png";
                    break;

                case WeatherIcons.DAY_SLEET:
                    fileIcon = "wi-day-sleet.png";
                    break;

                case WeatherIcons.DAY_SLEET_STORM:
                    fileIcon = "wi-day-sleet-storm.png";
                    break;

                case WeatherIcons.DAY_SNOW:
                    fileIcon = "wi-day-snow.png";
                    break;

                case WeatherIcons.DAY_SNOW_THUNDERSTORM:
                    fileIcon = "wi-day-snow-thunderstorm.png";
                    break;

                case WeatherIcons.DAY_SNOW_WIND:
                    fileIcon = "wi-day-snow-wind.png";
                    break;

                case WeatherIcons.DAY_SPRINKLE:
                    fileIcon = "wi-day-sprinkle.png";
                    break;

                case WeatherIcons.DAY_STORM_SHOWERS:
                    fileIcon = "wi-day-storm-showers.png";
                    break;

                case WeatherIcons.DAY_SUNNY_OVERCAST:
                    fileIcon = "wi-day-sunny-overcast.png";
                    break;

                case WeatherIcons.DAY_THUNDERSTORM:
                    fileIcon = "wi-day-thunderstorm.png";
                    break;

                case WeatherIcons.DAY_WINDY:
                    fileIcon = "wi-day-windy.png";
                    break;

                case WeatherIcons.DAY_HOT:
                    fileIcon = "wi-hot.png";
                    break;

                case WeatherIcons.DAY_CLOUDY_HIGH:
                    fileIcon = "wi-day-cloudy-high.png";
                    break;

                case WeatherIcons.DAY_LIGHT_WIND:
                    fileIcon = "wi-day-light-wind.png";
                    break;

                // Night
                case WeatherIcons.NIGHT_CLEAR:
                    fileIcon = "wi-night-clear.png";
                    break;

                case WeatherIcons.NIGHT_ALT_CLOUDY:
                    fileIcon = "wi-night-alt-cloudy.png";
                    break;

                case WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS:
                    fileIcon = "wi-night-alt-cloudy-gusts.png";
                    break;

                case WeatherIcons.NIGHT_ALT_CLOUDY_WINDY:
                    fileIcon = "wi-night-alt-cloudy-windy.png";
                    break;

                case WeatherIcons.NIGHT_ALT_HAIL:
                    fileIcon = "wi-night-alt-hail.png";
                    break;

                case WeatherIcons.NIGHT_ALT_LIGHTNING:
                    fileIcon = "wi-night-alt-lightning.png";
                    break;

                case WeatherIcons.NIGHT_ALT_RAIN:
                    fileIcon = "wi-night-alt-rain.png";
                    break;

                case WeatherIcons.NIGHT_ALT_RAIN_MIX:
                    fileIcon = "wi-night-alt-rain-mix.png";
                    break;

                case WeatherIcons.NIGHT_ALT_RAIN_WIND:
                    fileIcon = "wi-night-alt-rain-wind.png";
                    break;

                case WeatherIcons.NIGHT_ALT_SHOWERS:
                    fileIcon = "wi-night-alt-showers.png";
                    break;

                case WeatherIcons.NIGHT_ALT_SLEET:
                    fileIcon = "wi-night-alt-sleet.png";
                    break;

                case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                    fileIcon = "wi-night-alt-sleet-storm.png";
                    break;

                case WeatherIcons.NIGHT_ALT_SNOW:
                    fileIcon = "wi-night-alt-snow.png";
                    break;

                case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                    fileIcon = "wi-night-alt-snow-thunderstorm.png";
                    break;

                case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                    fileIcon = "wi-night-alt-snow-wind.png";
                    break;

                case WeatherIcons.NIGHT_ALT_SPRINKLE:
                    fileIcon = "wi-night-alt-sprinkle.png";
                    break;

                case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                    fileIcon = "wi-night-alt-storm-showers.png";
                    break;

                case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                    fileIcon = "wi-night-alt-thunderstorm.png";
                    break;

                case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                    fileIcon = "wi-night-alt-partly-cloudy.png";
                    break;

                case WeatherIcons.NIGHT_ALT_CLOUDY_HIGH:
                    fileIcon = "wi-night-alt-cloudy-high.png";
                    break;

                case WeatherIcons.NIGHT_FOG:
                    fileIcon = "wi-night-fog.png";
                    break;

                // Neutral
                case WeatherIcons.CLOUD:
                    fileIcon = "wi-cloud.png";
                    break;

                case WeatherIcons.CLOUDY:
                    fileIcon = "wi-cloudy.png";
                    break;

                case WeatherIcons.CLOUDY_GUSTS:
                    fileIcon = "wi-cloudy-gusts.png";
                    break;

                case WeatherIcons.CLOUDY_WINDY:
                    fileIcon = "wi-cloudy-windy.png";
                    break;

                case WeatherIcons.FOG:
                    fileIcon = "wi-fog.png";
                    break;

                case WeatherIcons.HAIL:
                    fileIcon = "wi-hail.png";
                    break;

                case WeatherIcons.RAIN:
                    fileIcon = "wi-rain.png";
                    break;

                case WeatherIcons.RAIN_MIX:
                    fileIcon = "wi-rain-mix.png";
                    break;

                case WeatherIcons.RAIN_WIND:
                    fileIcon = "wi-rain-wind.png";
                    break;

                case WeatherIcons.SHOWERS:
                    fileIcon = "wi-showers.png";
                    break;

                case WeatherIcons.SLEET:
                    fileIcon = "wi-sleet.png";
                    break;

                case WeatherIcons.SNOW:
                    fileIcon = "wi-snow.png";
                    break;

                case WeatherIcons.SPRINKLE:
                    fileIcon = "wi-sprinkle.png";
                    break;

                case WeatherIcons.STORM_SHOWERS:
                    fileIcon = "wi-storm-showers.png";
                    break;

                case WeatherIcons.THUNDERSTORM:
                    fileIcon = "wi-thunderstorm.png";
                    break;

                case WeatherIcons.SNOW_WIND:
                    fileIcon = "wi-snow-wind.png";
                    break;

                case WeatherIcons.SMOG:
                    fileIcon = "wi-smog.png";
                    break;

                case WeatherIcons.SMOKE:
                    fileIcon = "wi-smoke.png";
                    break;

                case WeatherIcons.LIGHTNING:
                    fileIcon = "wi-lightning.png";
                    break;

                case WeatherIcons.DUST:
                    fileIcon = "wi-dust.png";
                    break;

                case WeatherIcons.SNOWFLAKE_COLD:
                    fileIcon = "wi-snowflake-cold.png";
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
                    fileIcon = "wi-na.png";
                    break;
            }

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "wi-na.png";
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
#endif