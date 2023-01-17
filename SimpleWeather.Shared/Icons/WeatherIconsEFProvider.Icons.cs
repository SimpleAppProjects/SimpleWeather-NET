using System;

namespace SimpleWeather.Icons
{
    public partial class WeatherIconsEFProvider : WeatherIconProvider
    {
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
                WeatherIcons.DAY_SUNNY => "wi_day_sunny.png",
                WeatherIcons.DAY_CLOUDY => "wi_day_cloudy.png",
                WeatherIcons.DAY_CLOUDY_GUSTS => "wi_day_cloudy_gusts.png",
                WeatherIcons.DAY_CLOUDY_WINDY => "wi_day_cloudy_windy.png",
                WeatherIcons.DAY_FOG => "wi_day_fog.png",
                WeatherIcons.DAY_HAIL => "wi_day_hail.png",
                WeatherIcons.DAY_HAZE => "wi_day_haze.png",
                WeatherIcons.DAY_LIGHTNING => "wi_day_lightning.png",
                WeatherIcons.DAY_RAIN => "wi_day_rain.png",
                WeatherIcons.DAY_RAIN_MIX => "wi_day_rain_mix.png",
                WeatherIcons.DAY_RAIN_WIND => "wi_day_rain_wind.png",
                WeatherIcons.DAY_SHOWERS => "wi_day_showers.png",
                WeatherIcons.DAY_SLEET => "wi_day_sleet.png",
                WeatherIcons.DAY_SLEET_STORM => "wi_day_sleet_storm.png",
                WeatherIcons.DAY_SNOW => "wi_day_snow.png",
                WeatherIcons.DAY_SNOW_THUNDERSTORM => "wi_day_snow_thunderstorm.png",
                WeatherIcons.DAY_SNOW_WIND => "wi_day_snow_wind.png",
                WeatherIcons.DAY_SPRINKLE => "wi_day_sprinkle.png",
                WeatherIcons.DAY_STORM_SHOWERS => "wi_day_storm_showers.png",
                WeatherIcons.DAY_PARTLY_CLOUDY or
                WeatherIcons.DAY_SUNNY_OVERCAST => "wi_day_sunny_overcast.png",
                WeatherIcons.DAY_THUNDERSTORM => "wi_day_thunderstorm.png",
                WeatherIcons.DAY_WINDY => "wi_day_windy.png",
                WeatherIcons.DAY_HOT => "wi_hot.png",
                WeatherIcons.DAY_CLOUDY_HIGH => "wi_day_cloudy_high.png",
                WeatherIcons.DAY_LIGHT_WIND => "wi_day_light_wind.png",

                // Night
                WeatherIcons.NIGHT_CLEAR => "wi_night_clear.png",
                WeatherIcons.NIGHT_ALT_CLOUDY => "wi_night_alt_cloudy.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS => "wi_night_alt_cloudy_gusts.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY => "wi_night_alt_cloudy_windy.png",
                WeatherIcons.NIGHT_ALT_HAIL => "wi_night_alt_hail.png",
                WeatherIcons.NIGHT_ALT_LIGHTNING => "wi_night_alt_lightning.png",
                WeatherIcons.NIGHT_ALT_RAIN => "wi_night_alt_rain.png",
                WeatherIcons.NIGHT_ALT_RAIN_MIX => "wi_night_alt_rain_mix.png",
                WeatherIcons.NIGHT_ALT_RAIN_WIND => "wi_night_alt_rain_wind.png",
                WeatherIcons.NIGHT_ALT_SHOWERS => "wi_night_alt_showers.png",
                WeatherIcons.NIGHT_ALT_SLEET => "wi_night_alt_sleet.png",
                WeatherIcons.NIGHT_ALT_SLEET_STORM => "wi_night_alt_sleet_storm.png",
                WeatherIcons.NIGHT_ALT_SNOW => "wi_night_alt_snow.png",
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM => "wi_night_alt_snow_thunderstorm.png",
                WeatherIcons.NIGHT_ALT_SNOW_WIND => "wi_night_alt_snow_wind.png",
                WeatherIcons.NIGHT_ALT_SPRINKLE => "wi_night_alt_sprinkle.png",
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS => "wi_night_alt_storm_showers.png",
                WeatherIcons.NIGHT_ALT_THUNDERSTORM => "wi_night_alt_thunderstorm.png",
                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY or
                WeatherIcons.NIGHT_OVERCAST => "wi_night_alt_partly_cloudy.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "wi_night_alt_cloudy_high.png",
                WeatherIcons.NIGHT_FOG or
                WeatherIcons.NIGHT_HAZE => "wi_night_fog.png",
                WeatherIcons.NIGHT_WINDY => "wi_windy.png",
                WeatherIcons.NIGHT_HOT => "wi_thermometer_up.png",
                WeatherIcons.NIGHT_LIGHT_WIND => "wi_windy.png",

                // Neutral
                WeatherIcons.CLOUD => "wi_cloud.png",
                WeatherIcons.CLOUDY => "wi_cloudy.png",
                WeatherIcons.CLOUDY_GUSTS => "wi_cloudy_gusts.png",
                WeatherIcons.CLOUDY_WINDY => "wi_cloudy_windy.png",
                WeatherIcons.FOG => "wi_fog.png",
                WeatherIcons.HAIL => "wi_hail.png",
                WeatherIcons.HAZE => "wi_windy.png",
                WeatherIcons.HOT => "wi_thermometer_up.png",
                WeatherIcons.LIGHT_WIND => "wi_windy.png",
                WeatherIcons.OVERCAST => "wi_cloudy.png",
                WeatherIcons.RAIN => "wi_rain.png",
                WeatherIcons.RAIN_MIX => "wi_rain_mix.png",
                WeatherIcons.RAIN_WIND => "wi_rain_wind.png",
                WeatherIcons.SHOWERS => "wi_showers.png",
                WeatherIcons.SLEET => "wi_sleet.png",
                WeatherIcons.SLEET_STORM => "wi_sleet_storm.png",
                WeatherIcons.SNOW => "wi_snow.png",
                WeatherIcons.SNOW_THUNDERSTORM => "wi_snow_thunderstorm.png",
                WeatherIcons.SPRINKLE => "wi_sprinkle.png",
                WeatherIcons.STORM_SHOWERS => "wi_storm_showers.png",
                WeatherIcons.THUNDERSTORM => "wi_thunderstorm.png",
                WeatherIcons.SNOW_WIND => "wi_snow_wind.png",
                WeatherIcons.SMOG => "wi_smog.png",
                WeatherIcons.SMOKE => "wi_smoke.png",
                WeatherIcons.LIGHTNING => "wi_lightning.png",
                WeatherIcons.DUST => "wi_dust.png",
                WeatherIcons.SNOWFLAKE_COLD => "wi_snowflake_cold.png",
                WeatherIcons.WINDY => "wi_windy.png",
                WeatherIcons.STRONG_WIND => "wi_strong_wind.png",
                WeatherIcons.SANDSTORM => "wi_sandstorm.png",
                WeatherIcons.HURRICANE => "wi_hurricane.png",
                WeatherIcons.TORNADO => "wi_tornado.png",
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
                WeatherIcons.UV_INDEX_11 => "wi_day_sunny.png",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.png",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.png",
                WeatherIcons.RAGWEED_POLLEN => "ic_ragweed_pollen.png",

                WeatherIcons.NA => "wi_na.png",

                _ => string.Empty,
            };

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "wi_na.png";
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
                WeatherIcons.DAY_SUNNY => "wi_day_sunny.svg",
                WeatherIcons.DAY_CLOUDY => "wi_day_cloudy.svg",
                WeatherIcons.DAY_CLOUDY_GUSTS => "wi_day_cloudy_gusts.svg",
                WeatherIcons.DAY_CLOUDY_WINDY => "wi_day_cloudy_windy.svg",
                WeatherIcons.DAY_FOG => "wi_day_fog.svg",
                WeatherIcons.DAY_HAIL => "wi_day_hail.svg",
                WeatherIcons.DAY_HAZE => "wi_day_haze.svg",
                WeatherIcons.DAY_LIGHTNING => "wi_day_lightning.svg",
                WeatherIcons.DAY_RAIN => "wi_day_rain.svg",
                WeatherIcons.DAY_RAIN_MIX => "wi_day_rain_mix.svg",
                WeatherIcons.DAY_RAIN_WIND => "wi_day_rain_wind.svg",
                WeatherIcons.DAY_SHOWERS => "wi_day_showers.svg",
                WeatherIcons.DAY_SLEET => "wi_day_sleet.svg",
                WeatherIcons.DAY_SLEET_STORM => "wi_day_sleet_storm.svg",
                WeatherIcons.DAY_SNOW => "wi_day_snow.svg",
                WeatherIcons.DAY_SNOW_THUNDERSTORM => "wi_day_snow_thunderstorm.svg",
                WeatherIcons.DAY_SNOW_WIND => "wi_day_snow_wind.svg",
                WeatherIcons.DAY_SPRINKLE => "wi_day_sprinkle.svg",
                WeatherIcons.DAY_STORM_SHOWERS => "wi_day_storm_showers.svg",
                WeatherIcons.DAY_PARTLY_CLOUDY or
                WeatherIcons.DAY_SUNNY_OVERCAST => "wi_day_sunny_overcast.svg",
                WeatherIcons.DAY_THUNDERSTORM => "wi_day_thunderstorm.svg",
                WeatherIcons.DAY_WINDY => "wi_day_windy.svg",
                WeatherIcons.DAY_HOT => "wi_hot.svg",
                WeatherIcons.DAY_CLOUDY_HIGH => "wi_day_cloudy_high.svg",
                WeatherIcons.DAY_LIGHT_WIND => "wi_day_light_wind.svg",

                // Night
                WeatherIcons.NIGHT_CLEAR => "wi_night_clear.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY => "wi_night_alt_cloudy.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS => "wi_night_alt_cloudy_gusts.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY => "wi_night_alt_cloudy_windy.svg",
                WeatherIcons.NIGHT_ALT_HAIL => "wi_night_alt_hail.svg",
                WeatherIcons.NIGHT_ALT_LIGHTNING => "wi_night_alt_lightning.svg",
                WeatherIcons.NIGHT_ALT_RAIN => "wi_night_alt_rain.svg",
                WeatherIcons.NIGHT_ALT_RAIN_MIX => "wi_night_alt_rain_mix.svg",
                WeatherIcons.NIGHT_ALT_RAIN_WIND => "wi_night_alt_rain_wind.svg",
                WeatherIcons.NIGHT_ALT_SHOWERS => "wi_night_alt_showers.svg",
                WeatherIcons.NIGHT_ALT_SLEET => "wi_night_alt_sleet.svg",
                WeatherIcons.NIGHT_ALT_SLEET_STORM => "wi_night_alt_sleet_storm.svg",
                WeatherIcons.NIGHT_ALT_SNOW => "wi_night_alt_snow.svg",
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM => "wi_night_alt_snow_thunderstorm.svg",
                WeatherIcons.NIGHT_ALT_SNOW_WIND => "wi_night_alt_snow_wind.svg",
                WeatherIcons.NIGHT_ALT_SPRINKLE => "wi_night_alt_sprinkle.svg",
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS => "wi_night_alt_storm_showers.svg",
                WeatherIcons.NIGHT_ALT_THUNDERSTORM => "wi_night_alt_thunderstorm.svg",
                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY or
                WeatherIcons.NIGHT_OVERCAST => "wi_night_alt_partly_cloudy.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "wi_night_alt_cloudy_high.svg",
                WeatherIcons.NIGHT_FOG or
                WeatherIcons.NIGHT_HAZE => "wi_night_fog.svg",
                WeatherIcons.NIGHT_WINDY => "wi_windy.svg",
                WeatherIcons.NIGHT_HOT => "wi_thermometer_up.svg",
                WeatherIcons.NIGHT_LIGHT_WIND => "wi_windy.svg",

                // Neutral
                WeatherIcons.CLOUD => "wi_cloud.svg",
                WeatherIcons.CLOUDY => "wi_cloudy.svg",
                WeatherIcons.CLOUDY_GUSTS => "wi_cloudy_gusts.svg",
                WeatherIcons.CLOUDY_WINDY => "wi_cloudy_windy.svg",
                WeatherIcons.FOG => "wi_fog.svg",
                WeatherIcons.HAIL => "wi_hail.svg",
                WeatherIcons.HAZE => "wi_windy.svg",
                WeatherIcons.HOT => "wi_thermometer_up.svg",
                WeatherIcons.LIGHT_WIND => "wi_windy.svg",
                WeatherIcons.OVERCAST => "wi_cloudy.svg",
                WeatherIcons.RAIN => "wi_rain.svg",
                WeatherIcons.RAIN_MIX => "wi_rain_mix.svg",
                WeatherIcons.RAIN_WIND => "wi_rain_wind.svg",
                WeatherIcons.SHOWERS => "wi_showers.svg",
                WeatherIcons.SLEET => "wi_sleet.svg",
                WeatherIcons.SLEET_STORM => "wi_sleet_storm.svg",
                WeatherIcons.SNOW => "wi_snow.svg",
                WeatherIcons.SNOW_THUNDERSTORM => "wi_snow_thunderstorm.svg",
                WeatherIcons.SPRINKLE => "wi_sprinkle.svg",
                WeatherIcons.STORM_SHOWERS => "wi_storm_showers.svg",
                WeatherIcons.THUNDERSTORM => "wi_thunderstorm.svg",
                WeatherIcons.SNOW_WIND => "wi_snow_wind.svg",
                WeatherIcons.SMOG => "wi_smog.svg",
                WeatherIcons.SMOKE => "wi_smoke.svg",
                WeatherIcons.LIGHTNING => "wi_lightning.svg",
                WeatherIcons.DUST => "wi_dust.svg",
                WeatherIcons.SNOWFLAKE_COLD => "wi_snowflake_cold.svg",
                WeatherIcons.WINDY => "wi_windy.svg",
                WeatherIcons.STRONG_WIND => "wi_strong_wind.svg",
                WeatherIcons.SANDSTORM => "wi_sandstorm.svg",
                WeatherIcons.HURRICANE => "wi_hurricane.svg",
                WeatherIcons.TORNADO => "wi_tornado.svg",
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
                WeatherIcons.UV_INDEX_11 => "wi_day_sunny.svg",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.svg",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.svg",
                WeatherIcons.RAGWEED_POLLEN => "ic_ragweed_pollen.svg",

                WeatherIcons.NA => "wi_na.svg",

                _ => string.Empty,
            };

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "wi_na.svg";
            }

            return baseuri + fileIcon;
        }
    }
}
