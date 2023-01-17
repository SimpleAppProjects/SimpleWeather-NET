using System;

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
                WeatherIcons.DAY_LIGHT_WIND => "wui_clear.png",

                WeatherIcons.DAY_CLOUDY or
                WeatherIcons.DAY_CLOUDY_GUSTS or
                WeatherIcons.DAY_CLOUDY_WINDY or
                WeatherIcons.DAY_CLOUDY_HIGH => "wui_mostlycloudy.png",

                WeatherIcons.DAY_PARTLY_CLOUDY or
                WeatherIcons.DAY_SUNNY_OVERCAST => "wui_partlycloudy.png",

                // Night
                WeatherIcons.NIGHT_CLEAR or
                WeatherIcons.NIGHT_HOT or
                WeatherIcons.NIGHT_LIGHT_WIND => "wui_nt_clear.png",

                WeatherIcons.NIGHT_ALT_CLOUDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS or
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY => "wui_nt_mostlycloudy.png",

                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY or
                WeatherIcons.NIGHT_OVERCAST => "wui_nt_partlycloudy.png",

                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "wui_nt_mostlycloudy.png",

                // Neutral
                WeatherIcons.DAY_FOG or
                WeatherIcons.DAY_HAZE or
                WeatherIcons.NIGHT_FOG or
                WeatherIcons.NIGHT_HAZE or
                WeatherIcons.FOG or
                WeatherIcons.HAZE => "wui_fog.png",

                WeatherIcons.DAY_LIGHTNING or
                WeatherIcons.NIGHT_ALT_LIGHTNING or
                WeatherIcons.LIGHTNING => "wui_chancetstorms.png",

                WeatherIcons.DAY_HAIL or
                WeatherIcons.DAY_SLEET or
                WeatherIcons.DAY_SLEET_STORM or
                WeatherIcons.NIGHT_ALT_HAIL or
                WeatherIcons.NIGHT_ALT_SLEET or
                WeatherIcons.NIGHT_ALT_SLEET_STORM or
                WeatherIcons.SLEET or
                WeatherIcons.SLEET_STORM => "wui_sleet.png",

                WeatherIcons.DAY_RAIN_MIX or
                WeatherIcons.NIGHT_ALT_RAIN_MIX or
                WeatherIcons.RAIN_MIX or
                WeatherIcons.HAIL => "wui_chancesleet.png",

                WeatherIcons.DAY_RAIN or
                WeatherIcons.DAY_RAIN_WIND or
                WeatherIcons.DAY_SHOWERS or
                WeatherIcons.NIGHT_ALT_RAIN or
                WeatherIcons.NIGHT_ALT_RAIN_WIND or
                WeatherIcons.NIGHT_ALT_SHOWERS or
                WeatherIcons.RAIN or
                WeatherIcons.RAIN_WIND or
                WeatherIcons.SHOWERS => "wui_rain.png",

                WeatherIcons.DAY_SNOW or
                WeatherIcons.DAY_SNOW_THUNDERSTORM or
                WeatherIcons.DAY_SNOW_WIND or
                WeatherIcons.NIGHT_ALT_SNOW or
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_SNOW_WIND or
                WeatherIcons.SNOW or
                WeatherIcons.SNOW_THUNDERSTORM or
                WeatherIcons.SNOW_WIND or
                WeatherIcons.SNOWFLAKE_COLD => "wui_snow.png",

                WeatherIcons.DAY_SPRINKLE or
                WeatherIcons.NIGHT_ALT_SPRINKLE or
                WeatherIcons.SPRINKLE => "wui_chancerain.png",

                WeatherIcons.DAY_STORM_SHOWERS or
                WeatherIcons.DAY_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS or
                WeatherIcons.NIGHT_ALT_THUNDERSTORM or
                WeatherIcons.STORM_SHOWERS or
                WeatherIcons.THUNDERSTORM => "wui_tstorms.png",

                WeatherIcons.DAY_WINDY or
                WeatherIcons.NIGHT_WINDY => "wi_windy.png",

                WeatherIcons.CLOUD or
                WeatherIcons.CLOUDY or
                WeatherIcons.CLOUDY_GUSTS or
                WeatherIcons.CLOUDY_WINDY or
                WeatherIcons.OVERCAST => "wui_cloudy.png",

                WeatherIcons.HOT => "wi_thermometer_up.png",
                WeatherIcons.SMOG => "wi_smog.png",
                WeatherIcons.SMOKE => "wi_smoke.png",
                WeatherIcons.DUST => "wi_dust.png",
                WeatherIcons.WINDY or
                WeatherIcons.LIGHT_WIND => "wi_windy.png",
                WeatherIcons.STRONG_WIND => "wi_strong_wind.png",
                WeatherIcons.SANDSTORM => "wi_sandstorm.png",
                WeatherIcons.HURRICANE => "wi_hurricane.png",
                WeatherIcons.TORNADO => "wi_tornado.png",

                // Misc icons
                WeatherIcons.FIRE => "wi_fire.png",
                WeatherIcons.FLOOD => "wi_flood.png",
                WeatherIcons.VOLCANO => "wi_volcano.png",
                WeatherIcons.BAROMETER => "wi_barometer.png",
                WeatherIcons.HUMIDITY => "wi_humidity.png",
                WeatherIcons.MOONRISE => "wi_moonrise.png",
                WeatherIcons.MOONSET => "wi_moonset.png",
                WeatherIcons.RAINDROP => "wi_raindrop.png",
                WeatherIcons.RAINDROPS => "wi_raindrops.png",
                WeatherIcons.SUNRISE => "wi_sunrise.png",
                WeatherIcons.SUNSET => "wi_sunset.png",
                WeatherIcons.THERMOMETER => "wi_thermometer.png",
                WeatherIcons.UMBRELLA => "wi_umbrella.png",
                WeatherIcons.WIND_DIRECTION => "wi_wind_direction.png",
                WeatherIcons.DIRECTION_UP => "wi_direction_up.png",
                WeatherIcons.DIRECTION_DOWN => "wi_direction_down.png",

                // Beaufort
                WeatherIcons.WIND_BEAUFORT_0 => "wi_wind_beaufort_0.png",
                WeatherIcons.WIND_BEAUFORT_1 => "wi_wind_beaufort_1.png",
                WeatherIcons.WIND_BEAUFORT_2 => "wi_wind_beaufort_2.png",
                WeatherIcons.WIND_BEAUFORT_3 => "wi_wind_beaufort_3.png",
                WeatherIcons.WIND_BEAUFORT_4 => "wi_wind_beaufort_4.png",
                WeatherIcons.WIND_BEAUFORT_5 => "wi_wind_beaufort_5.png",
                WeatherIcons.WIND_BEAUFORT_6 => "wi_wind_beaufort_6.png",
                WeatherIcons.WIND_BEAUFORT_7 => "wi_wind_beaufort_7.png",
                WeatherIcons.WIND_BEAUFORT_8 => "wi_wind_beaufort_8.png",
                WeatherIcons.WIND_BEAUFORT_9 => "wi_wind_beaufort_9.png",
                WeatherIcons.WIND_BEAUFORT_10 => "wi_wind_beaufort_10.png",
                WeatherIcons.WIND_BEAUFORT_11 => "wi_wind_beaufort_11.png",
                WeatherIcons.WIND_BEAUFORT_12 => "wi_wind_beaufort_12.png",

                // Moon Phase
                WeatherIcons.MOON_NEW => "wi_moon_new.png",
                WeatherIcons.MOON_WAXING_CRESCENT_3 => "wi_moon_waxing_crescent_3.png",
                WeatherIcons.MOON_FIRST_QUARTER => "wi_moon_first_quarter.png",
                WeatherIcons.MOON_WAXING_GIBBOUS_3 => "wi_moon_waxing_gibbous_3.png",
                WeatherIcons.MOON_FULL => "wi_moon_full.png",
                WeatherIcons.MOON_WANING_GIBBOUS_3 => "wi_moon_waning_gibbous_3.png",
                WeatherIcons.MOON_THIRD_QUARTER => "wi_moon_third_quarter.png",
                WeatherIcons.MOON_WANING_CRESCENT_3 => "wi_moon_waning_crescent_3.png",
                WeatherIcons.MOON_ALT_NEW => "wi_moon_alt_new.png",
                WeatherIcons.MOON_ALT_WAXING_CRESCENT_3 => "wi_moon_alt_waxing_crescent_3.png",
                WeatherIcons.MOON_ALT_FIRST_QUARTER => "wi_moon_alt_first_quarter.png",
                WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3 => "wi_moon_alt_waxing_gibbous_3.png",
                WeatherIcons.MOON_ALT_FULL => "wi_moon_alt_full.png",
                WeatherIcons.MOON_ALT_WANING_GIBBOUS_3 => "wi_moon_alt_waning_gibbous_3.png",
                WeatherIcons.MOON_ALT_THIRD_QUARTER => "wi_moon_alt_third_quarter.png",
                WeatherIcons.MOON_ALT_WANING_CRESCENT_3 => "wi_moon_alt_waning_crescent_3.png",

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
                WeatherIcons.UV_INDEX_11 => "wui_clear.png",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.png",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.png",
                WeatherIcons.RAGWEED_POLLEN => "ic_ragweed_pollen.png",

                WeatherIcons.FAHRENHEIT => "wi_fahrenheit.png",
                WeatherIcons.CELSIUS => "wi_celsius.png",

                WeatherIcons.NA => "wui_unknown.png",

                _ => string.Empty,
            };

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "wui_unknown.png";
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
                WeatherIcons.DAY_LIGHT_WIND => "wui_clear.svg",

                WeatherIcons.DAY_CLOUDY or
                WeatherIcons.DAY_CLOUDY_GUSTS or
                WeatherIcons.DAY_CLOUDY_WINDY or
                WeatherIcons.DAY_CLOUDY_HIGH => "wui_mostlycloudy.svg",

                WeatherIcons.DAY_PARTLY_CLOUDY or
                WeatherIcons.DAY_SUNNY_OVERCAST => "wui_partlycloudy.svg",

                // Night
                WeatherIcons.NIGHT_CLEAR or
                WeatherIcons.NIGHT_HOT or
                WeatherIcons.NIGHT_LIGHT_WIND => "wui_nt_clear.svg",

                WeatherIcons.NIGHT_ALT_CLOUDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS or
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY => "wui_nt_mostlycloudy.svg",

                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY or
                WeatherIcons.NIGHT_OVERCAST => "wui_nt_partlycloudy.svg",

                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "wui_nt_mostlycloudy.svg",

                // Neutral
                WeatherIcons.DAY_FOG or
                WeatherIcons.DAY_HAZE or
                WeatherIcons.NIGHT_FOG or
                WeatherIcons.NIGHT_HAZE or
                WeatherIcons.FOG or
                WeatherIcons.HAZE => "wui_fog.svg",

                WeatherIcons.DAY_LIGHTNING or
                WeatherIcons.NIGHT_ALT_LIGHTNING or
                WeatherIcons.LIGHTNING => "wui_chancetstorms.svg",

                WeatherIcons.DAY_HAIL or
                WeatherIcons.DAY_SLEET or
                WeatherIcons.DAY_SLEET_STORM or
                WeatherIcons.NIGHT_ALT_HAIL or
                WeatherIcons.NIGHT_ALT_SLEET or
                WeatherIcons.NIGHT_ALT_SLEET_STORM or
                WeatherIcons.SLEET or
                WeatherIcons.SLEET_STORM => "wui_sleet.svg",

                WeatherIcons.DAY_RAIN_MIX or
                WeatherIcons.NIGHT_ALT_RAIN_MIX or
                WeatherIcons.RAIN_MIX or
                WeatherIcons.HAIL => "wui_chancesleet.svg",

                WeatherIcons.DAY_RAIN or
                WeatherIcons.DAY_RAIN_WIND or
                WeatherIcons.DAY_SHOWERS or
                WeatherIcons.NIGHT_ALT_RAIN or
                WeatherIcons.NIGHT_ALT_RAIN_WIND or
                WeatherIcons.NIGHT_ALT_SHOWERS or
                WeatherIcons.RAIN or
                WeatherIcons.RAIN_WIND or
                WeatherIcons.SHOWERS => "wui_rain.svg",

                WeatherIcons.DAY_SNOW or
                WeatherIcons.DAY_SNOW_THUNDERSTORM or
                WeatherIcons.DAY_SNOW_WIND or
                WeatherIcons.NIGHT_ALT_SNOW or
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_SNOW_WIND or
                WeatherIcons.SNOW or
                WeatherIcons.SNOW_THUNDERSTORM or
                WeatherIcons.SNOW_WIND or
                WeatherIcons.SNOWFLAKE_COLD => "wui_snow.svg",

                WeatherIcons.DAY_SPRINKLE or
                WeatherIcons.NIGHT_ALT_SPRINKLE or
                WeatherIcons.SPRINKLE => "wui_chancerain.svg",

                WeatherIcons.DAY_STORM_SHOWERS or
                WeatherIcons.DAY_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS or
                WeatherIcons.NIGHT_ALT_THUNDERSTORM or
                WeatherIcons.STORM_SHOWERS or
                WeatherIcons.THUNDERSTORM => "wui_tstorms.svg",

                WeatherIcons.DAY_WINDY or
                WeatherIcons.NIGHT_WINDY => "wi_windy.svg",

                WeatherIcons.CLOUD or
                WeatherIcons.CLOUDY or
                WeatherIcons.CLOUDY_GUSTS or
                WeatherIcons.CLOUDY_WINDY or
                WeatherIcons.OVERCAST => "wui_cloudy.svg",

                WeatherIcons.HOT => "wi_thermometer_up.svg",
                WeatherIcons.SMOG => "wi_smog.svg",
                WeatherIcons.SMOKE => "wi_smoke.svg",
                WeatherIcons.DUST => "wi_dust.svg",
                WeatherIcons.WINDY or
                WeatherIcons.LIGHT_WIND => "wi_windy.svg",
                WeatherIcons.STRONG_WIND => "wi_strong_wind.svg",
                WeatherIcons.SANDSTORM => "wi_sandstorm.svg",
                WeatherIcons.HURRICANE => "wi_hurricane.svg",
                WeatherIcons.TORNADO => "wi_tornado.svg",

                // Misc icons
                WeatherIcons.FIRE => "wi_fire.svg",
                WeatherIcons.FLOOD => "wi_flood.svg",
                WeatherIcons.VOLCANO => "wi_volcano.svg",
                WeatherIcons.BAROMETER => "wi_barometer.svg",
                WeatherIcons.HUMIDITY => "wi_humidity.svg",
                WeatherIcons.MOONRISE => "wi_moonrise.svg",
                WeatherIcons.MOONSET => "wi_moonset.svg",
                WeatherIcons.RAINDROP => "wi_raindrop.svg",
                WeatherIcons.RAINDROPS => "wi_raindrops.svg",
                WeatherIcons.SUNRISE => "wi_sunrise.svg",
                WeatherIcons.SUNSET => "wi_sunset.svg",
                WeatherIcons.THERMOMETER => "wi_thermometer.svg",
                WeatherIcons.UMBRELLA => "wi_umbrella.svg",
                WeatherIcons.WIND_DIRECTION => "wi_wind_direction.svg",
                WeatherIcons.DIRECTION_UP => "wi_direction_up.svg",
                WeatherIcons.DIRECTION_DOWN => "wi_direction_down.svg",

                // Beaufort
                WeatherIcons.WIND_BEAUFORT_0 => "wi_wind_beaufort_0.svg",
                WeatherIcons.WIND_BEAUFORT_1 => "wi_wind_beaufort_1.svg",
                WeatherIcons.WIND_BEAUFORT_2 => "wi_wind_beaufort_2.svg",
                WeatherIcons.WIND_BEAUFORT_3 => "wi_wind_beaufort_3.svg",
                WeatherIcons.WIND_BEAUFORT_4 => "wi_wind_beaufort_4.svg",
                WeatherIcons.WIND_BEAUFORT_5 => "wi_wind_beaufort_5.svg",
                WeatherIcons.WIND_BEAUFORT_6 => "wi_wind_beaufort_6.svg",
                WeatherIcons.WIND_BEAUFORT_7 => "wi_wind_beaufort_7.svg",
                WeatherIcons.WIND_BEAUFORT_8 => "wi_wind_beaufort_8.svg",
                WeatherIcons.WIND_BEAUFORT_9 => "wi_wind_beaufort_9.svg",
                WeatherIcons.WIND_BEAUFORT_10 => "wi_wind_beaufort_10.svg",
                WeatherIcons.WIND_BEAUFORT_11 => "wi_wind_beaufort_11.svg",
                WeatherIcons.WIND_BEAUFORT_12 => "wi_wind_beaufort_12.svg",

                // Moon Phase
                WeatherIcons.MOON_NEW => "wi_moon_new.svg",
                WeatherIcons.MOON_WAXING_CRESCENT_3 => "wi_moon_waxing_crescent_3.svg",
                WeatherIcons.MOON_FIRST_QUARTER => "wi_moon_first_quarter.svg",
                WeatherIcons.MOON_WAXING_GIBBOUS_3 => "wi_moon_waxing_gibbous_3.svg",
                WeatherIcons.MOON_FULL => "wi_moon_full.svg",
                WeatherIcons.MOON_WANING_GIBBOUS_3 => "wi_moon_waning_gibbous_3.svg",
                WeatherIcons.MOON_THIRD_QUARTER => "wi_moon_third_quarter.svg",
                WeatherIcons.MOON_WANING_CRESCENT_3 => "wi_moon_waning_crescent_3.svg",
                WeatherIcons.MOON_ALT_NEW => "wi_moon_alt_new.svg",
                WeatherIcons.MOON_ALT_WAXING_CRESCENT_3 => "wi_moon_alt_waxing_crescent_3.svg",
                WeatherIcons.MOON_ALT_FIRST_QUARTER => "wi_moon_alt_first_quarter.svg",
                WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3 => "wi_moon_alt_waxing_gibbous_3.svg",
                WeatherIcons.MOON_ALT_FULL => "wi_moon_alt_full.svg",
                WeatherIcons.MOON_ALT_WANING_GIBBOUS_3 => "wi_moon_alt_waning_gibbous_3.svg",
                WeatherIcons.MOON_ALT_THIRD_QUARTER => "wi_moon_alt_third_quarter.svg",
                WeatherIcons.MOON_ALT_WANING_CRESCENT_3 => "wi_moon_alt_waning_crescent_3.svg",

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
                WeatherIcons.UV_INDEX_11 => "wui_clear.svg",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.svg",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.svg",
                WeatherIcons.RAGWEED_POLLEN => "ic_ragweed_pollen.svg",

                WeatherIcons.FAHRENHEIT => "wi_fahrenheit.svg",
                WeatherIcons.CELSIUS => "wi_celsius.svg",

                WeatherIcons.NA => "wui_unknown.svg",

                _ => string.Empty,
            };

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "wui_unknown.svg";
            }

            return baseuri + fileIcon;
        }
    }
}
