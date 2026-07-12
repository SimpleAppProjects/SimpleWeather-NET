using System;

namespace SimpleWeather.Icons
{
    internal sealed class GoogleWeatherIconsProvider : WeatherIconProvider
    {
        public override string Key => "goog-weather-icons";
        public override string DisplayName => "Google Weather Icons";

        public override string AuthorName => "Google";

        public override Uri AttributionLink => new("https://developers.google.com/maps/documentation/weather/weather-condition-icons");

        public override bool IsFontIcon => false;

        public override Uri GetWeatherIconURI(string icon)
        {
            return new Uri(GetWeatherIconURI(icon, true));
        }

        public override String GetWeatherIconURI(string icon, bool isAbsoluteUri, bool isLight = false)
        {
            string baseuri = GetPNGBaseUri(isLight);

            string fileIcon = icon switch
            {
                // Day
                /* CLEAR */
                WeatherIcons.DAY_SUNNY or
                WeatherIcons.DAY_HOT or
                WeatherIcons.DAY_LIGHT_WIND => "goog_clear_day.png",

                /* PARTLY_CLOUDY */
                WeatherIcons.DAY_PARTLY_CLOUDY => "goog_partly_cloudy_day.png",

                /* MOSTLY_CLOUDY */
                WeatherIcons.DAY_CLOUDY or
                WeatherIcons.DAY_CLOUDY_HIGH => "goog_mostly_cloudy_day.png",

                // Night
                /* CLEAR */
                WeatherIcons.NIGHT_CLEAR or
                WeatherIcons.NIGHT_HOT or
                WeatherIcons.NIGHT_LIGHT_WIND => "goog_clear_night.png",

                /* PARTLY_CLOUDY */
                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY => "goog_partly_cloudy_night.png",

                /* MOSTLY_CLOUDY */
                WeatherIcons.NIGHT_ALT_CLOUDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "goog_mostly_cloudy_night.png",

                /* CLOUDY */
                WeatherIcons.DAY_FOG or
                WeatherIcons.DAY_HAZE or
                WeatherIcons.DAY_SUNNY_OVERCAST or
                WeatherIcons.NIGHT_FOG or
                WeatherIcons.NIGHT_HAZE or
                WeatherIcons.NIGHT_OVERCAST or
                WeatherIcons.FOG or
                WeatherIcons.HAZE or
                WeatherIcons.OVERCAST or
                WeatherIcons.CLOUDY or
                WeatherIcons.CLOUD => "goog_cloudy.png",

                /* WINDY */
                WeatherIcons.DAY_CLOUDY_GUSTS or
                WeatherIcons.DAY_CLOUDY_WINDY or
                WeatherIcons.DAY_WINDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS or
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY or
                WeatherIcons.NIGHT_WINDY or
                WeatherIcons.CLOUDY_GUSTS or
                WeatherIcons.CLOUDY_WINDY or
                WeatherIcons.LIGHT_WIND or
                WeatherIcons.WINDY or
                WeatherIcons.STRONG_WIND => "goog_windy.png",

                /* WIND_AND_RAIN or RAIN_SHOWERS or LIGHT_TO_MODERATE_RAIN or RAIN */
                WeatherIcons.DAY_SHOWERS or
                WeatherIcons.DAY_RAIN or
                WeatherIcons.NIGHT_ALT_SHOWERS or
                WeatherIcons.NIGHT_ALT_RAIN or
                WeatherIcons.SHOWERS or
                WeatherIcons.RAIN => "goog_wind_rain.png",

                /* HEAVY_RAIN_SHOWERS or MODERATE_TO_HEAVY_RAIN or HEAVY_RAIN or RAIN_PERIODICALLY_HEAVY */
                WeatherIcons.DAY_RAIN_WIND or
                WeatherIcons.NIGHT_ALT_RAIN_WIND or
                WeatherIcons.RAIN_WIND => "goog_heavy_rain.png",

                /* LIGHT_RAIN_SHOWERS or CHANCE_OF_SHOWERS or LIGHT_RAIN */
                WeatherIcons.DAY_SPRINKLE or
                WeatherIcons.NIGHT_ALT_SPRINKLE or
                WeatherIcons.SPRINKLE => "goog_light_rain.png",

                /* SCATTERED_SHOWERS */
                // "goog_scattered_showers.png",

                /* LIGHT_SNOW_SHOWERS or LIGHT_SNOW */
                //"goog_light_snow.png",

                /* CHANCE_OF_SNOW_SHOWERS or LIGHT_TO_MODERATE_SNOW or SNOW or SNOWSTORM */
                WeatherIcons.DAY_SNOW or
                WeatherIcons.DAY_SNOW_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_SNOW or
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM or
                WeatherIcons.SNOW or
                WeatherIcons.SNOW_THUNDERSTORM => "goog_snow.png",

                /* SCATTERED_SNOW_SHOWERS or SNOW_SHOWERS */
                //"goog_snow_showers

                /* HEAVY_SNOW_SHOWERS or MODERATE_TO_HEAVY_SNOW or HEAVY_SNOW or SNOW_PERIODICALLY_HEAVY or HEAVY_SNOW_STORM */
                WeatherIcons.DAY_SNOW_WIND or
                WeatherIcons.NIGHT_ALT_SNOW_WIND or
                WeatherIcons.SNOW_WIND => "goog_heavy_snow.png",

                /* BLOWING_SNOW */
                //"goog_blowing_snow.png",

                /* RAIN_AND_SNOW or HAIL_SHOWERS */
                WeatherIcons.DAY_RAIN_MIX or
                WeatherIcons.DAY_SLEET or
                WeatherIcons.DAY_SLEET_STORM or
                WeatherIcons.NIGHT_ALT_RAIN_MIX or
                WeatherIcons.NIGHT_ALT_SLEET or
                WeatherIcons.NIGHT_ALT_SLEET_STORM or
                WeatherIcons.RAIN_MIX or
                WeatherIcons.SLEET or
                WeatherIcons.SLEET_STORM => "goog_rain_mix.png",

                /* HAIL */
                WeatherIcons.DAY_HAIL or
                WeatherIcons.NIGHT_ALT_HAIL or
                WeatherIcons.HAIL => "goog_hail.png",

                /* THUNDERSTORM or THUNDERSHOWER or LIGHT_THUNDERSTORM_RAIN or HEAVY_THUNDERSTORM */
                WeatherIcons.DAY_STORM_SHOWERS or
                WeatherIcons.DAY_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS or
                WeatherIcons.NIGHT_ALT_THUNDERSTORM or
                WeatherIcons.STORM_SHOWERS or
                WeatherIcons.THUNDERSTORM => "goog_tstorm.png",

                /* SCATTERED_THUNDERSTORMS */
                WeatherIcons.DAY_LIGHTNING or
                WeatherIcons.NIGHT_ALT_LIGHTNING or
                WeatherIcons.LIGHTNING => "goog_scattered_tstorms.png",

                // Misc
                WeatherIcons.HOT => "material_thermometer_gain.png",

                WeatherIcons.SMOG => "wi_smog.png",
                WeatherIcons.SMOKE => "wi_smoke.png",
                WeatherIcons.DUST => "wi_dust.png",
                WeatherIcons.SNOWFLAKE_COLD => "material_cold.png",
                WeatherIcons.SANDSTORM => "wi_sandstorm.png",
                WeatherIcons.HURRICANE => "material_storm.png",
                WeatherIcons.TORNADO => "material_tornado.png",
                WeatherIcons.FIRE => "material_fire.png",
                WeatherIcons.FLOOD => "material_flood.png",
                WeatherIcons.VOLCANO => "material_volcano.png",
                WeatherIcons.BAROMETER => "jdwi_pressure.png",
                WeatherIcons.HUMIDITY => "material_humidity_percentage.png",
                WeatherIcons.MOONRISE => "jdwi_moonrise.png",
                WeatherIcons.MOONSET => "jdwi_moonset.png",
                WeatherIcons.RAINDROP or
                WeatherIcons.RAINDROPS => "material_water_drop.png",

                WeatherIcons.SUNRISE => "jdwi_sunrise.png",
                WeatherIcons.SUNSET => "jdwi_sunset.png",
                WeatherIcons.THERMOMETER => "jdwi_temperature.png",
                WeatherIcons.UMBRELLA => "jdwi_precipitation_alt.png",
                WeatherIcons.WIND_DIRECTION => "jdwi_wind_direction.png",
                WeatherIcons.DIRECTION_UP => "material_arrow_upward.png",
                WeatherIcons.DIRECTION_DOWN => "material_arrow_downward.png",

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

                WeatherIcons.MOON_NEW or
                WeatherIcons.MOON_ALT_NEW => "jdwi_new_moon.png",

                WeatherIcons.MOON_WAXING_CRESCENT_3 or
                WeatherIcons.MOON_ALT_WAXING_CRESCENT_3 => "jdwi_waxing_crescent.png",

                WeatherIcons.MOON_FIRST_QUARTER or
                WeatherIcons.MOON_ALT_FIRST_QUARTER => "jdwi_first_quarter.png",

                WeatherIcons.MOON_WAXING_GIBBOUS_3 or
                WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3 => "jdwi_waxing_gibbous.png",

                WeatherIcons.MOON_FULL or
                WeatherIcons.MOON_ALT_FULL => "jdwi_full_moon.png",

                WeatherIcons.MOON_WANING_GIBBOUS_3 or
                WeatherIcons.MOON_ALT_WANING_GIBBOUS_3 => "jdwi_waning_gibbous.png",

                WeatherIcons.MOON_THIRD_QUARTER or
                WeatherIcons.MOON_ALT_THIRD_QUARTER => "jdwi_third_quarter.png",

                WeatherIcons.MOON_WANING_CRESCENT_3 or
                WeatherIcons.MOON_ALT_WANING_CRESCENT_3 => "jdwi_waning_crescent.png",

                WeatherIcons.FAHRENHEIT => "wi_fahrenheit.png",
                WeatherIcons.CELSIUS => "wi_celsius.png",

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
                WeatherIcons.UV_INDEX_11 => "jdwi_uv_index.png",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.png",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.png",
                WeatherIcons.RAGWEED_POLLEN => "ic_ragweed_pollen.png",

                WeatherIcons.NA => "jdwi_unknown.png",

                _ => string.Empty,
            };

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "jdwi_unknown.png";
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
            string baseuri = GetSVGBaseUri(isLight);

            string fileIcon = icon switch
            {
                // Day
                /* CLEAR */
                WeatherIcons.DAY_SUNNY or
                WeatherIcons.DAY_HOT or
                WeatherIcons.DAY_LIGHT_WIND => "goog_clear_day.svg",

                /* PARTLY_CLOUDY */
                WeatherIcons.DAY_PARTLY_CLOUDY => "goog_partly_cloudy_day.svg",

                /* MOSTLY_CLOUDY */
                WeatherIcons.DAY_CLOUDY or
                WeatherIcons.DAY_CLOUDY_HIGH => "goog_mostly_cloudy_day.svg",

                // Night
                /* CLEAR */
                WeatherIcons.NIGHT_CLEAR or
                WeatherIcons.NIGHT_HOT or
                WeatherIcons.NIGHT_LIGHT_WIND => "goog_clear_night.svg",

                /* PARTLY_CLOUDY */
                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY => "goog_partly_cloudy_night.svg",

                /* MOSTLY_CLOUDY */
                WeatherIcons.NIGHT_ALT_CLOUDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "goog_mostly_cloudy_night.svg",

                /* CLOUDY */
                WeatherIcons.DAY_FOG or
                WeatherIcons.DAY_HAZE or
                WeatherIcons.DAY_SUNNY_OVERCAST or
                WeatherIcons.NIGHT_FOG or
                WeatherIcons.NIGHT_HAZE or
                WeatherIcons.NIGHT_OVERCAST or
                WeatherIcons.FOG or
                WeatherIcons.HAZE or
                WeatherIcons.OVERCAST or
                WeatherIcons.CLOUDY or
                WeatherIcons.CLOUD => "goog_cloudy.svg",

                /* WINDY */
                WeatherIcons.DAY_CLOUDY_GUSTS or
                WeatherIcons.DAY_CLOUDY_WINDY or
                WeatherIcons.DAY_WINDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS or
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY or
                WeatherIcons.NIGHT_WINDY or
                WeatherIcons.CLOUDY_GUSTS or
                WeatherIcons.CLOUDY_WINDY or
                WeatherIcons.LIGHT_WIND or
                WeatherIcons.WINDY or
                WeatherIcons.STRONG_WIND => "goog_windy.svg",

                /* WIND_AND_RAIN or RAIN_SHOWERS or LIGHT_TO_MODERATE_RAIN or RAIN */
                WeatherIcons.DAY_SHOWERS or
                WeatherIcons.DAY_RAIN or
                WeatherIcons.NIGHT_ALT_SHOWERS or
                WeatherIcons.NIGHT_ALT_RAIN or
                WeatherIcons.SHOWERS or
                WeatherIcons.RAIN => "goog_wind_rain.svg",

                /* HEAVY_RAIN_SHOWERS or MODERATE_TO_HEAVY_RAIN or HEAVY_RAIN or RAIN_PERIODICALLY_HEAVY */
                WeatherIcons.DAY_RAIN_WIND or
                WeatherIcons.NIGHT_ALT_RAIN_WIND or
                WeatherIcons.RAIN_WIND => "goog_heavy_rain.svg",

                /* LIGHT_RAIN_SHOWERS or CHANCE_OF_SHOWERS or LIGHT_RAIN */
                WeatherIcons.DAY_SPRINKLE or
                WeatherIcons.NIGHT_ALT_SPRINKLE or
                WeatherIcons.SPRINKLE => "goog_light_rain.svg",

                /* SCATTERED_SHOWERS */
                // "goog_scattered_showers.svg",

                /* LIGHT_SNOW_SHOWERS or LIGHT_SNOW */
                //"goog_light_snow.svg",

                /* CHANCE_OF_SNOW_SHOWERS or LIGHT_TO_MODERATE_SNOW or SNOW or SNOWSTORM */
                WeatherIcons.DAY_SNOW or
                WeatherIcons.DAY_SNOW_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_SNOW or
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM or
                WeatherIcons.SNOW or
                WeatherIcons.SNOW_THUNDERSTORM => "goog_snow.svg",

                /* SCATTERED_SNOW_SHOWERS or SNOW_SHOWERS */
                //"goog_snow_showers

                /* HEAVY_SNOW_SHOWERS or MODERATE_TO_HEAVY_SNOW or HEAVY_SNOW or SNOW_PERIODICALLY_HEAVY or HEAVY_SNOW_STORM */
                WeatherIcons.DAY_SNOW_WIND or
                WeatherIcons.NIGHT_ALT_SNOW_WIND or
                WeatherIcons.SNOW_WIND => "goog_heavy_snow.svg",

                /* BLOWING_SNOW */
                //"goog_blowing_snow.svg",

                /* RAIN_AND_SNOW or HAIL_SHOWERS */
                WeatherIcons.DAY_RAIN_MIX or
                WeatherIcons.DAY_SLEET or
                WeatherIcons.DAY_SLEET_STORM or
                WeatherIcons.NIGHT_ALT_RAIN_MIX or
                WeatherIcons.NIGHT_ALT_SLEET or
                WeatherIcons.NIGHT_ALT_SLEET_STORM or
                WeatherIcons.RAIN_MIX or
                WeatherIcons.SLEET or
                WeatherIcons.SLEET_STORM => "goog_rain_mix.svg",

                /* HAIL */
                WeatherIcons.DAY_HAIL or
                WeatherIcons.NIGHT_ALT_HAIL or
                WeatherIcons.HAIL => "goog_hail.svg",

                /* THUNDERSTORM or THUNDERSHOWER or LIGHT_THUNDERSTORM_RAIN or HEAVY_THUNDERSTORM */
                WeatherIcons.DAY_STORM_SHOWERS or
                WeatherIcons.DAY_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS or
                WeatherIcons.NIGHT_ALT_THUNDERSTORM or
                WeatherIcons.STORM_SHOWERS or
                WeatherIcons.THUNDERSTORM => "goog_tstorm.svg",

                /* SCATTERED_THUNDERSTORMS */
                WeatherIcons.DAY_LIGHTNING or
                WeatherIcons.NIGHT_ALT_LIGHTNING or
                WeatherIcons.LIGHTNING => "goog_scattered_tstorms.svg",

                // Misc
                WeatherIcons.HOT => "material_thermometer_gain.svg",

                WeatherIcons.SMOG => "wi_smog.svg",
                WeatherIcons.SMOKE => "wi_smoke.svg",
                WeatherIcons.DUST => "wi_dust.svg",
                WeatherIcons.SNOWFLAKE_COLD => "material_cold.svg",
                WeatherIcons.SANDSTORM => "wi_sandstorm.svg",
                WeatherIcons.HURRICANE => "material_storm.svg",
                WeatherIcons.TORNADO => "material_tornado.svg",
                WeatherIcons.FIRE => "material_fire.svg",
                WeatherIcons.FLOOD => "material_flood.svg",
                WeatherIcons.VOLCANO => "material_volcano.svg",
                WeatherIcons.BAROMETER => "jdwi_pressure.svg",
                WeatherIcons.HUMIDITY => "material_humidity_percentage.svg",
                WeatherIcons.MOONRISE => "jdwi_moonrise.svg",
                WeatherIcons.MOONSET => "jdwi_moonset.svg",
                WeatherIcons.RAINDROP or
                WeatherIcons.RAINDROPS => "material_water_drop.svg",

                WeatherIcons.SUNRISE => "jdwi_sunrise.svg",
                WeatherIcons.SUNSET => "jdwi_sunset.svg",
                WeatherIcons.THERMOMETER => "jdwi_temperature.svg",
                WeatherIcons.UMBRELLA => "jdwi_precipitation_alt.svg",
                WeatherIcons.WIND_DIRECTION => "jdwi_wind_direction.svg",
                WeatherIcons.DIRECTION_UP => "material_arrow_upward.svg",
                WeatherIcons.DIRECTION_DOWN => "material_arrow_downward.svg",

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

                WeatherIcons.MOON_NEW or
                WeatherIcons.MOON_ALT_NEW => "jdwi_new_moon.svg",

                WeatherIcons.MOON_WAXING_CRESCENT_3 or
                WeatherIcons.MOON_ALT_WAXING_CRESCENT_3 => "jdwi_waxing_crescent.svg",

                WeatherIcons.MOON_FIRST_QUARTER or
                WeatherIcons.MOON_ALT_FIRST_QUARTER => "jdwi_first_quarter.svg",

                WeatherIcons.MOON_WAXING_GIBBOUS_3 or
                WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3 => "jdwi_waxing_gibbous.svg",

                WeatherIcons.MOON_FULL or
                WeatherIcons.MOON_ALT_FULL => "jdwi_full_moon.svg",

                WeatherIcons.MOON_WANING_GIBBOUS_3 or
                WeatherIcons.MOON_ALT_WANING_GIBBOUS_3 => "jdwi_waning_gibbous.svg",

                WeatherIcons.MOON_THIRD_QUARTER or
                WeatherIcons.MOON_ALT_THIRD_QUARTER => "jdwi_third_quarter.svg",

                WeatherIcons.MOON_WANING_CRESCENT_3 or
                WeatherIcons.MOON_ALT_WANING_CRESCENT_3 => "jdwi_waning_crescent.svg",

                WeatherIcons.FAHRENHEIT => "wi_fahrenheit.svg",
                WeatherIcons.CELSIUS => "wi_celsius.svg",

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
                WeatherIcons.UV_INDEX_11 => "jdwi_uv_index.svg",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.svg",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.svg",
                WeatherIcons.RAGWEED_POLLEN => "ic_ragweed_pollen.svg",

                WeatherIcons.NA => "jdwi_unknown.svg",

                _ => string.Empty,
            };

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "jdwi_unknown.svg";
            }

            return baseuri + fileIcon;
        }
    }
}
