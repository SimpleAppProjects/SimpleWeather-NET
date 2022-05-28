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
            string baseuri = WeatherIconsManager.GetPNGBaseUri(isLight);

            string fileIcon = icon switch
            {
                // Day
                WeatherIcons.DAY_SUNNY or
                WeatherIcons.DAY_HOT or
                WeatherIcons.DAY_LIGHT_WIND => "wui-clear.png",

                WeatherIcons.DAY_CLOUDY or
                WeatherIcons.DAY_CLOUDY_GUSTS or
                WeatherIcons.DAY_CLOUDY_WINDY or
                WeatherIcons.DAY_CLOUDY_HIGH => "wui-mostlycloudy.png",

                WeatherIcons.DAY_PARTLY_CLOUDY or
                WeatherIcons.DAY_SUNNY_OVERCAST => "wui-partlycloudy.png",

                // Night
                WeatherIcons.NIGHT_CLEAR or
                WeatherIcons.NIGHT_HOT or
                WeatherIcons.NIGHT_LIGHT_WIND => "wui-nt_clear.png",

                WeatherIcons.NIGHT_ALT_CLOUDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS or
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY => "wui-nt_mostlycloudy.png",

                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY or
                WeatherIcons.NIGHT_OVERCAST => "wui-nt_partlycloudy.png",

                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "wui-nt_mostlycloudy.png",

                // Neutral
                WeatherIcons.DAY_FOG or
                WeatherIcons.DAY_HAZE or
                WeatherIcons.NIGHT_FOG or
                WeatherIcons.NIGHT_HAZE or
                WeatherIcons.FOG or
                WeatherIcons.HAZE => "wui-fog.png",

                WeatherIcons.DAY_LIGHTNING or
                WeatherIcons.NIGHT_ALT_LIGHTNING or
                WeatherIcons.LIGHTNING => "wui-chancetstorms.png",

                WeatherIcons.DAY_HAIL or
                WeatherIcons.DAY_SLEET or
                WeatherIcons.DAY_SLEET_STORM or
                WeatherIcons.NIGHT_ALT_HAIL or
                WeatherIcons.NIGHT_ALT_SLEET or
                WeatherIcons.NIGHT_ALT_SLEET_STORM or
                WeatherIcons.SLEET or
                WeatherIcons.SLEET_STORM => "wui-sleet.png",

                WeatherIcons.DAY_RAIN_MIX or
                WeatherIcons.NIGHT_ALT_RAIN_MIX or
                WeatherIcons.RAIN_MIX or
                WeatherIcons.HAIL => "wui-chancesleet.png",

                WeatherIcons.DAY_RAIN or
                WeatherIcons.DAY_RAIN_WIND or 
                WeatherIcons.DAY_SHOWERS or
                WeatherIcons.NIGHT_ALT_RAIN or 
                WeatherIcons.NIGHT_ALT_RAIN_WIND or
                WeatherIcons.NIGHT_ALT_SHOWERS or
                WeatherIcons.RAIN or 
                WeatherIcons.RAIN_WIND or 
                WeatherIcons.SHOWERS => "wui-rain.png",

                WeatherIcons.DAY_SNOW or 
                WeatherIcons.DAY_SNOW_THUNDERSTORM or 
                WeatherIcons.DAY_SNOW_WIND or 
                WeatherIcons.NIGHT_ALT_SNOW or 
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM or 
                WeatherIcons.NIGHT_ALT_SNOW_WIND or 
                WeatherIcons.SNOW or 
                WeatherIcons.SNOW_THUNDERSTORM or 
                WeatherIcons.SNOW_WIND or 
                WeatherIcons.SNOWFLAKE_COLD => "wui-snow.png",

                WeatherIcons.DAY_SPRINKLE or
                WeatherIcons.NIGHT_ALT_SPRINKLE or
                WeatherIcons.SPRINKLE => "wui-chancerain.png",

                WeatherIcons.DAY_STORM_SHOWERS or
                WeatherIcons.DAY_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS or
                WeatherIcons.NIGHT_ALT_THUNDERSTORM or
                WeatherIcons.STORM_SHOWERS or
                WeatherIcons.THUNDERSTORM => "wui-tstorms.png",

                WeatherIcons.DAY_WINDY or
                WeatherIcons.NIGHT_WINDY => "wi-windy.png",

                WeatherIcons.CLOUD or
                WeatherIcons.CLOUDY or
                WeatherIcons.CLOUDY_GUSTS or
                WeatherIcons.CLOUDY_WINDY or
                WeatherIcons.OVERCAST => "wui-cloudy.png",

                WeatherIcons.HOT => "wi-thermometer-up.png",
                WeatherIcons.SMOG => "wi-smog.png",
                WeatherIcons.SMOKE => "wi-smoke.png",
                WeatherIcons.DUST => "wi-dust.png",
                WeatherIcons.WINDY or
                WeatherIcons.LIGHT_WIND => "wi-windy.png",
                WeatherIcons.STRONG_WIND => "wi-strong-wind.png",
                WeatherIcons.SANDSTORM => "wi-sandstorm.png",
                WeatherIcons.HURRICANE => "wi-hurricane.png",
                WeatherIcons.TORNADO => "wi-tornado.png",

                // Misc icons
                WeatherIcons.FIRE => "wi-fire.png",
                WeatherIcons.FLOOD => "wi-flood.png",
                WeatherIcons.VOLCANO => "wi-volcano.png",
                WeatherIcons.BAROMETER => "wi-barometer.png",
                WeatherIcons.HUMIDITY => "wi-humidity.png",
                WeatherIcons.MOONRISE => "wi-moonrise.png",
                WeatherIcons.MOONSET => "wi-moonset.png",
                WeatherIcons.RAINDROP => "wi-raindrop.png",
                WeatherIcons.RAINDROPS => "wi-raindrops.png",
                WeatherIcons.SUNRISE => "wi-sunrise.png",
                WeatherIcons.SUNSET => "wi-sunset.png",
                WeatherIcons.THERMOMETER => "wi-thermometer.png",
                WeatherIcons.UMBRELLA => "wi-umbrella.png",
                WeatherIcons.WIND_DIRECTION => "wi-wind-direction.png",
                WeatherIcons.DIRECTION_UP => "wi-direction-up.png",
                WeatherIcons.DIRECTION_DOWN => "wi-direction-down.png",

                // Beaufort
                WeatherIcons.WIND_BEAUFORT_0 => "wi-wind-beaufort-0.png",
                WeatherIcons.WIND_BEAUFORT_1 => "wi-wind-beaufort-1.png",
                WeatherIcons.WIND_BEAUFORT_2 => "wi-wind-beaufort-2.png",
                WeatherIcons.WIND_BEAUFORT_3 => "wi-wind-beaufort-3.png",
                WeatherIcons.WIND_BEAUFORT_4 => "wi-wind-beaufort-4.png",
                WeatherIcons.WIND_BEAUFORT_5 => "wi-wind-beaufort-5.png",
                WeatherIcons.WIND_BEAUFORT_6 => "wi-wind-beaufort-6.png",
                WeatherIcons.WIND_BEAUFORT_7 => "wi-wind-beaufort-7.png",
                WeatherIcons.WIND_BEAUFORT_8 => "wi-wind-beaufort-8.png",
                WeatherIcons.WIND_BEAUFORT_9 => "wi-wind-beaufort-9.png",
                WeatherIcons.WIND_BEAUFORT_10 => "wi-wind-beaufort-10.png",
                WeatherIcons.WIND_BEAUFORT_11 => "wi-wind-beaufort-11.png",
                WeatherIcons.WIND_BEAUFORT_12 => "wi-wind-beaufort-12.png",

                // Moon Phase
                WeatherIcons.MOON_NEW => "wi-moon-new.png",
                WeatherIcons.MOON_WAXING_CRESCENT_3 => "wi-moon-waxing-crescent-3.png",
                WeatherIcons.MOON_FIRST_QUARTER => "wi-moon-first-quarter.png",
                WeatherIcons.MOON_WAXING_GIBBOUS_3 => "wi-moon-waxing-gibbous-3.png",
                WeatherIcons.MOON_FULL => "wi-moon-full.png",
                WeatherIcons.MOON_WANING_GIBBOUS_3 => "wi-moon-waning-gibbous-3.png",
                WeatherIcons.MOON_THIRD_QUARTER => "wi-moon-third-quarter.png",
                WeatherIcons.MOON_WANING_CRESCENT_3 => "wi-moon-waning-crescent-3.png",
                WeatherIcons.MOON_ALT_NEW => "wi-moon-alt-new.png",
                WeatherIcons.MOON_ALT_WAXING_CRESCENT_3 => "wi-moon-alt-waxing-crescent-3.png",
                WeatherIcons.MOON_ALT_FIRST_QUARTER => "wi-moon-alt-first-quarter.png",
                WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3 => "wi-moon-alt-waxing-gibbous-3.png",
                WeatherIcons.MOON_ALT_FULL => "wi-moon-alt-full.png",
                WeatherIcons.MOON_ALT_WANING_GIBBOUS_3 => "wi-moon-alt-waning-gibbous-3.png",
                WeatherIcons.MOON_ALT_THIRD_QUARTER => "wi-moon-alt-third-quarter.png",
                WeatherIcons.MOON_ALT_WANING_CRESCENT_3 => "wi-moon-alt-waning-crescent-3.png",

                WeatherIcons.UV_INDEX or
                WeatherIcons.UV_INDEX_1 or
                WeatherIcons.UV_INDEX_2 or
                WeatherIcons.UV_INDEX_3 or
                WeatherIcons.UV_INDEX_4 or
                WeatherIcons.UV_INDEX_5 or
                WeatherIcons.UV_INDEX_6 or
                WeatherIcons.UV_INDEX_7 or
                WeatherIcons.UV_INDEX_8 or 
                WeatherIcons.UV_INDEX_9 or 
                WeatherIcons.UV_INDEX_10 or
                WeatherIcons.UV_INDEX_11 => "wui-clear.png",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.png",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.png",
                WeatherIcons.RAGWEED_POLLEN => "ic_ragweed_pollen.png",

                WeatherIcons.FAHRENHEIT => "wi-fahrenheit.png",
                WeatherIcons.CELSIUS => "wi-celsius.png",

                WeatherIcons.NA => "wui-unknown.png",

                _ => string.Empty,
            };

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

        public override String GetSVGIconUri(string icon, bool isLight = false)
        {
            string baseuri = WeatherIconsManager.GetSVGBaseUri(isLight);

            string fileIcon = icon switch
            {
                // Day
                WeatherIcons.DAY_SUNNY or
                WeatherIcons.DAY_HOT or
                WeatherIcons.DAY_LIGHT_WIND => "wui-clear.svg",

                WeatherIcons.DAY_CLOUDY or
                WeatherIcons.DAY_CLOUDY_GUSTS or
                WeatherIcons.DAY_CLOUDY_WINDY or
                WeatherIcons.DAY_CLOUDY_HIGH => "wui-mostlycloudy.svg",

                WeatherIcons.DAY_PARTLY_CLOUDY or
                WeatherIcons.DAY_SUNNY_OVERCAST => "wui-partlycloudy.svg",

                // Night
                WeatherIcons.NIGHT_CLEAR or
                WeatherIcons.NIGHT_HOT or
                WeatherIcons.NIGHT_LIGHT_WIND => "wui-nt_clear.svg",

                WeatherIcons.NIGHT_ALT_CLOUDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS or
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY => "wui-nt_mostlycloudy.svg",

                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY or
                WeatherIcons.NIGHT_OVERCAST => "wui-nt_partlycloudy.svg",

                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "wui-nt_mostlycloudy.svg",

                // Neutral
                WeatherIcons.DAY_FOG or
                WeatherIcons.DAY_HAZE or
                WeatherIcons.NIGHT_FOG or
                WeatherIcons.NIGHT_HAZE or
                WeatherIcons.FOG or
                WeatherIcons.HAZE => "wui-fog.svg",

                WeatherIcons.DAY_LIGHTNING or
                WeatherIcons.NIGHT_ALT_LIGHTNING or
                WeatherIcons.LIGHTNING => "wui-chancetstorms.svg",

                WeatherIcons.DAY_HAIL or
                WeatherIcons.DAY_SLEET or
                WeatherIcons.DAY_SLEET_STORM or
                WeatherIcons.NIGHT_ALT_HAIL or
                WeatherIcons.NIGHT_ALT_SLEET or
                WeatherIcons.NIGHT_ALT_SLEET_STORM or
                WeatherIcons.SLEET or
                WeatherIcons.SLEET_STORM => "wui-sleet.svg",

                WeatherIcons.DAY_RAIN_MIX or
                WeatherIcons.NIGHT_ALT_RAIN_MIX or
                WeatherIcons.RAIN_MIX or
                WeatherIcons.HAIL => "wui-chancesleet.svg",

                WeatherIcons.DAY_RAIN or
                WeatherIcons.DAY_RAIN_WIND or
                WeatherIcons.DAY_SHOWERS or
                WeatherIcons.NIGHT_ALT_RAIN or
                WeatherIcons.NIGHT_ALT_RAIN_WIND or
                WeatherIcons.NIGHT_ALT_SHOWERS or
                WeatherIcons.RAIN or
                WeatherIcons.RAIN_WIND or
                WeatherIcons.SHOWERS => "wui-rain.svg",

                WeatherIcons.DAY_SNOW or
                WeatherIcons.DAY_SNOW_THUNDERSTORM or
                WeatherIcons.DAY_SNOW_WIND or
                WeatherIcons.NIGHT_ALT_SNOW or
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_SNOW_WIND or
                WeatherIcons.SNOW or
                WeatherIcons.SNOW_THUNDERSTORM or
                WeatherIcons.SNOW_WIND or
                WeatherIcons.SNOWFLAKE_COLD => "wui-snow.svg",

                WeatherIcons.DAY_SPRINKLE or
                WeatherIcons.NIGHT_ALT_SPRINKLE or
                WeatherIcons.SPRINKLE => "wui-chancerain.svg",

                WeatherIcons.DAY_STORM_SHOWERS or
                WeatherIcons.DAY_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS or
                WeatherIcons.NIGHT_ALT_THUNDERSTORM or
                WeatherIcons.STORM_SHOWERS or
                WeatherIcons.THUNDERSTORM => "wui-tstorms.svg",

                WeatherIcons.DAY_WINDY or
                WeatherIcons.NIGHT_WINDY => "wi-windy.svg",

                WeatherIcons.CLOUD or
                WeatherIcons.CLOUDY or
                WeatherIcons.CLOUDY_GUSTS or
                WeatherIcons.CLOUDY_WINDY or
                WeatherIcons.OVERCAST => "wui-cloudy.svg",

                WeatherIcons.HOT => "wi-thermometer-up.svg",
                WeatherIcons.SMOG => "wi-smog.svg",
                WeatherIcons.SMOKE => "wi-smoke.svg",
                WeatherIcons.DUST => "wi-dust.svg",
                WeatherIcons.WINDY or
                WeatherIcons.LIGHT_WIND => "wi-windy.svg",
                WeatherIcons.STRONG_WIND => "wi-strong-wind.svg",
                WeatherIcons.SANDSTORM => "wi-sandstorm.svg",
                WeatherIcons.HURRICANE => "wi-hurricane.svg",
                WeatherIcons.TORNADO => "wi-tornado.svg",

                // Misc icons
                WeatherIcons.FIRE => "wi-fire.svg",
                WeatherIcons.FLOOD => "wi-flood.svg",
                WeatherIcons.VOLCANO => "wi-volcano.svg",
                WeatherIcons.BAROMETER => "wi-barometer.svg",
                WeatherIcons.HUMIDITY => "wi-humidity.svg",
                WeatherIcons.MOONRISE => "wi-moonrise.svg",
                WeatherIcons.MOONSET => "wi-moonset.svg",
                WeatherIcons.RAINDROP => "wi-raindrop.svg",
                WeatherIcons.RAINDROPS => "wi-raindrops.svg",
                WeatherIcons.SUNRISE => "wi-sunrise.svg",
                WeatherIcons.SUNSET => "wi-sunset.svg",
                WeatherIcons.THERMOMETER => "wi-thermometer.svg",
                WeatherIcons.UMBRELLA => "wi-umbrella.svg",
                WeatherIcons.WIND_DIRECTION => "wi-wind-direction.svg",
                WeatherIcons.DIRECTION_UP => "wi-direction-up.svg",
                WeatherIcons.DIRECTION_DOWN => "wi-direction-down.svg",

                // Beaufort
                WeatherIcons.WIND_BEAUFORT_0 => "wi-wind-beaufort-0.svg",
                WeatherIcons.WIND_BEAUFORT_1 => "wi-wind-beaufort-1.svg",
                WeatherIcons.WIND_BEAUFORT_2 => "wi-wind-beaufort-2.svg",
                WeatherIcons.WIND_BEAUFORT_3 => "wi-wind-beaufort-3.svg",
                WeatherIcons.WIND_BEAUFORT_4 => "wi-wind-beaufort-4.svg",
                WeatherIcons.WIND_BEAUFORT_5 => "wi-wind-beaufort-5.svg",
                WeatherIcons.WIND_BEAUFORT_6 => "wi-wind-beaufort-6.svg",
                WeatherIcons.WIND_BEAUFORT_7 => "wi-wind-beaufort-7.svg",
                WeatherIcons.WIND_BEAUFORT_8 => "wi-wind-beaufort-8.svg",
                WeatherIcons.WIND_BEAUFORT_9 => "wi-wind-beaufort-9.svg",
                WeatherIcons.WIND_BEAUFORT_10 => "wi-wind-beaufort-10.svg",
                WeatherIcons.WIND_BEAUFORT_11 => "wi-wind-beaufort-11.svg",
                WeatherIcons.WIND_BEAUFORT_12 => "wi-wind-beaufort-12.svg",

                // Moon Phase
                WeatherIcons.MOON_NEW => "wi-moon-new.svg",
                WeatherIcons.MOON_WAXING_CRESCENT_3 => "wi-moon-waxing-crescent-3.svg",
                WeatherIcons.MOON_FIRST_QUARTER => "wi-moon-first-quarter.svg",
                WeatherIcons.MOON_WAXING_GIBBOUS_3 => "wi-moon-waxing-gibbous-3.svg",
                WeatherIcons.MOON_FULL => "wi-moon-full.svg",
                WeatherIcons.MOON_WANING_GIBBOUS_3 => "wi-moon-waning-gibbous-3.svg",
                WeatherIcons.MOON_THIRD_QUARTER => "wi-moon-third-quarter.svg",
                WeatherIcons.MOON_WANING_CRESCENT_3 => "wi-moon-waning-crescent-3.svg",
                WeatherIcons.MOON_ALT_NEW => "wi-moon-alt-new.svg",
                WeatherIcons.MOON_ALT_WAXING_CRESCENT_3 => "wi-moon-alt-waxing-crescent-3.svg",
                WeatherIcons.MOON_ALT_FIRST_QUARTER => "wi-moon-alt-first-quarter.svg",
                WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3 => "wi-moon-alt-waxing-gibbous-3.svg",
                WeatherIcons.MOON_ALT_FULL => "wi-moon-alt-full.svg",
                WeatherIcons.MOON_ALT_WANING_GIBBOUS_3 => "wi-moon-alt-waning-gibbous-3.svg",
                WeatherIcons.MOON_ALT_THIRD_QUARTER => "wi-moon-alt-third-quarter.svg",
                WeatherIcons.MOON_ALT_WANING_CRESCENT_3 => "wi-moon-alt-waning-crescent-3.svg",

                WeatherIcons.UV_INDEX or
                WeatherIcons.UV_INDEX_1 or
                WeatherIcons.UV_INDEX_2 or
                WeatherIcons.UV_INDEX_3 or
                WeatherIcons.UV_INDEX_4 or
                WeatherIcons.UV_INDEX_5 or
                WeatherIcons.UV_INDEX_6 or
                WeatherIcons.UV_INDEX_7 or
                WeatherIcons.UV_INDEX_8 or
                WeatherIcons.UV_INDEX_9 or
                WeatherIcons.UV_INDEX_10 or
                WeatherIcons.UV_INDEX_11 => "wui-clear.svg",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.svg",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.svg",
                WeatherIcons.RAGWEED_POLLEN => "ic_ragweed_pollen.svg",

                WeatherIcons.FAHRENHEIT => "wi-fahrenheit.svg",
                WeatherIcons.CELSIUS => "wi-celsius.svg",

                WeatherIcons.NA => "wui-unknown.svg",

                _ => string.Empty,
            };

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "wui-unknown.svg";
            }

            return baseuri + fileIcon;
        }
    }
}
