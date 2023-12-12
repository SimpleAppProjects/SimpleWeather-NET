using System;

namespace SimpleWeather.Icons
{
    internal sealed class JDWIIconsProvider : WeatherIconProvider
    {
        public override string Key => "mat-icons-jdwi";
        public override string DisplayName => "Material Icons";

        public override string AuthorName => $"Jelle Dekkers{Environment.NewLine}Material Icons";

        public override Uri AttributionLink => new Uri("https://github.com/MoRan1412/JDWI");

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
                WeatherIcons.DAY_LIGHT_WIND => "jdwi_clear_day.png",

                WeatherIcons.DAY_CLOUDY or
                WeatherIcons.DAY_CLOUDY_GUSTS or
                WeatherIcons.DAY_CLOUDY_WINDY or
                WeatherIcons.DAY_CLOUDY_HIGH => "jdwi_cloudy_day.png",

                WeatherIcons.DAY_FOG or
                WeatherIcons.DAY_HAZE => "jdwi_fog_day.png",

                WeatherIcons.DAY_HAIL or
                WeatherIcons.DAY_RAIN_MIX or
                WeatherIcons.DAY_SLEET or
                WeatherIcons.DAY_SLEET_STORM => "jdwi_sleet_day.png",

                WeatherIcons.DAY_LIGHTNING or
                WeatherIcons.DAY_STORM_SHOWERS or
                WeatherIcons.DAY_THUNDERSTORM => "jdwi_storm_day.png",

                WeatherIcons.DAY_RAIN or
                WeatherIcons.DAY_RAIN_WIND or
                WeatherIcons.DAY_SHOWERS or
                WeatherIcons.DAY_SPRINKLE => "jdwi_rain_day.png",

                WeatherIcons.DAY_SNOW or
                WeatherIcons.DAY_SNOW_THUNDERSTORM or
                WeatherIcons.DAY_SNOW_WIND => "jdwi_snow_day.png",

                WeatherIcons.DAY_PARTLY_CLOUDY or
                WeatherIcons.DAY_SUNNY_OVERCAST => "jdwi_partly_cloudy_day.png",

                WeatherIcons.DAY_WINDY => "jdwi_windy.png",

                // Night
                WeatherIcons.NIGHT_CLEAR or
                WeatherIcons.NIGHT_HOT or
                WeatherIcons.NIGHT_LIGHT_WIND => "jdwi_clear_night.png",

                WeatherIcons.NIGHT_ALT_CLOUDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS or
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "jdwi_cloudy_night.png",

                WeatherIcons.NIGHT_ALT_HAIL or
                WeatherIcons.NIGHT_ALT_RAIN_MIX or
                WeatherIcons.NIGHT_ALT_SLEET or
                WeatherIcons.NIGHT_ALT_SLEET_STORM => "jdwi_sleet_night.png",

                WeatherIcons.NIGHT_ALT_LIGHTNING or
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS or
                WeatherIcons.NIGHT_ALT_THUNDERSTORM => "jdwi_storm_night.png",

                WeatherIcons.NIGHT_ALT_RAIN or
                WeatherIcons.NIGHT_ALT_RAIN_WIND or
                WeatherIcons.NIGHT_ALT_SHOWERS or
                WeatherIcons.NIGHT_ALT_SPRINKLE => "jdwi_rain_night.png",

                WeatherIcons.NIGHT_ALT_SNOW or
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_SNOW_WIND => "jdwi_snow_night.png",

                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY or
                WeatherIcons.NIGHT_OVERCAST => "jdwi_partly_cloudy_night.png",

                WeatherIcons.NIGHT_FOG or
                WeatherIcons.NIGHT_HAZE => "jdwi_fog_night.png",

                WeatherIcons.NIGHT_WINDY => "jdwi_windy.png",

                // Neutral
                WeatherIcons.CLOUD or
                WeatherIcons.CLOUDY or
                WeatherIcons.OVERCAST => "jdwi_cloudy.png",

                WeatherIcons.FOG or
                WeatherIcons.HAZE => "jdwi_fog.png",

                WeatherIcons.HAIL or
                WeatherIcons.RAIN_MIX or
                WeatherIcons.SLEET or
                WeatherIcons.SLEET_STORM => "jdwi_sleet.png",

                WeatherIcons.HOT => "material_thermometer_gain.png",

                WeatherIcons.CLOUDY_GUSTS or
                WeatherIcons.CLOUDY_WINDY or
                WeatherIcons.LIGHT_WIND or
                WeatherIcons.WINDY or
                WeatherIcons.STRONG_WIND => "jdwi_windy.png",

                WeatherIcons.RAIN or
                WeatherIcons.RAIN_WIND or
                WeatherIcons.SHOWERS or
                WeatherIcons.SPRINKLE => "jdwi_rain.png",

                WeatherIcons.SNOW or
                WeatherIcons.SNOW_THUNDERSTORM or
                WeatherIcons.SNOW_WIND => "jdwi_snow.png",

                WeatherIcons.STORM_SHOWERS or
                WeatherIcons.THUNDERSTORM or
                WeatherIcons.LIGHTNING => "jdwi_storm.png",

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
            string baseuri = WeatherIconsManager.GetSVGBaseUri(isLight);

            string fileIcon = icon switch
            {
                // Day
                WeatherIcons.DAY_SUNNY or
                WeatherIcons.DAY_HOT or
                WeatherIcons.DAY_LIGHT_WIND => "jdwi_clear_day.svg",

                WeatherIcons.DAY_CLOUDY or
                WeatherIcons.DAY_CLOUDY_GUSTS or
                WeatherIcons.DAY_CLOUDY_WINDY or
                WeatherIcons.DAY_CLOUDY_HIGH => "jdwi_cloudy_day.svg",

                WeatherIcons.DAY_FOG or
                WeatherIcons.DAY_HAZE => "jdwi_fog_day.svg",

                WeatherIcons.DAY_HAIL or
                WeatherIcons.DAY_RAIN_MIX or
                WeatherIcons.DAY_SLEET or
                WeatherIcons.DAY_SLEET_STORM => "jdwi_sleet_day.svg",

                WeatherIcons.DAY_LIGHTNING or
                WeatherIcons.DAY_STORM_SHOWERS or
                WeatherIcons.DAY_THUNDERSTORM => "jdwi_storm_day.svg",

                WeatherIcons.DAY_RAIN or
                WeatherIcons.DAY_RAIN_WIND or
                WeatherIcons.DAY_SHOWERS or
                WeatherIcons.DAY_SPRINKLE => "jdwi_rain_day.svg",

                WeatherIcons.DAY_SNOW or
                WeatherIcons.DAY_SNOW_THUNDERSTORM or
                WeatherIcons.DAY_SNOW_WIND => "jdwi_snow_day.svg",

                WeatherIcons.DAY_PARTLY_CLOUDY or
                WeatherIcons.DAY_SUNNY_OVERCAST => "jdwi_partly_cloudy_day.svg",

                WeatherIcons.DAY_WINDY => "jdwi_windy.svg",

                // Night
                WeatherIcons.NIGHT_CLEAR or
                WeatherIcons.NIGHT_HOT or
                WeatherIcons.NIGHT_LIGHT_WIND => "jdwi_clear_night.svg",

                WeatherIcons.NIGHT_ALT_CLOUDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS or
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "jdwi_cloudy_night.svg",

                WeatherIcons.NIGHT_ALT_HAIL or
                WeatherIcons.NIGHT_ALT_RAIN_MIX or
                WeatherIcons.NIGHT_ALT_SLEET or
                WeatherIcons.NIGHT_ALT_SLEET_STORM => "jdwi_sleet_night.svg",

                WeatherIcons.NIGHT_ALT_LIGHTNING or
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS or
                WeatherIcons.NIGHT_ALT_THUNDERSTORM => "jdwi_storm_night.svg",

                WeatherIcons.NIGHT_ALT_RAIN or
                WeatherIcons.NIGHT_ALT_RAIN_WIND or
                WeatherIcons.NIGHT_ALT_SHOWERS or
                WeatherIcons.NIGHT_ALT_SPRINKLE => "jdwi_rain_night.svg",

                WeatherIcons.NIGHT_ALT_SNOW or
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_SNOW_WIND => "jdwi_snow_night.svg",

                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY or
                WeatherIcons.NIGHT_OVERCAST => "jdwi_partly_cloudy_night.svg",

                WeatherIcons.NIGHT_FOG or
                WeatherIcons.NIGHT_HAZE => "jdwi_fog_night.svg",

                WeatherIcons.NIGHT_WINDY => "jdwi_windy.svg",

                // Neutral
                WeatherIcons.CLOUD or
                WeatherIcons.CLOUDY or
                WeatherIcons.OVERCAST => "jdwi_cloudy.svg",

                WeatherIcons.FOG or
                WeatherIcons.HAZE => "jdwi_fog.svg",

                WeatherIcons.HAIL or
                WeatherIcons.RAIN_MIX or
                WeatherIcons.SLEET or
                WeatherIcons.SLEET_STORM => "jdwi_sleet.svg",

                WeatherIcons.HOT => "material_thermometer_gain.svg",

                WeatherIcons.CLOUDY_GUSTS or
                WeatherIcons.CLOUDY_WINDY or
                WeatherIcons.LIGHT_WIND or
                WeatherIcons.WINDY or
                WeatherIcons.STRONG_WIND => "jdwi_windy.svg",

                WeatherIcons.RAIN or
                WeatherIcons.RAIN_WIND or
                WeatherIcons.SHOWERS or
                WeatherIcons.SPRINKLE => "jdwi_rain.svg",

                WeatherIcons.SNOW or
                WeatherIcons.SNOW_THUNDERSTORM or
                WeatherIcons.SNOW_WIND => "jdwi_snow.svg",

                WeatherIcons.STORM_SHOWERS or
                WeatherIcons.THUNDERSTORM or
                WeatherIcons.LIGHTNING => "jdwi_storm.svg",

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
