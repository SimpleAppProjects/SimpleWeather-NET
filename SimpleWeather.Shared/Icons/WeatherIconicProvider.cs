using System;

namespace SimpleWeather.Icons
{
    internal sealed class WeatherIconicProvider : WeatherIconProvider
    {
        public override string Key => "w-iconic-jackd248";
        public override string DisplayName => "Weather Iconic";

        public override string AuthorName => "jackd248";

        public override Uri AttributionLink => new Uri("https://github.com/jackd248/weather-iconic");

        public override bool IsFontIcon => true;

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
                WeatherIcons.DAY_SUNNY => "wic_sun.png",
                WeatherIcons.DAY_CLOUDY => "wic_sun_cloud.png",
                WeatherIcons.DAY_CLOUDY_GUSTS => "wic_sun_cloud_wind.png",
                WeatherIcons.DAY_CLOUDY_WINDY => "wic_sun_cloud_wind.png",
                WeatherIcons.DAY_FOG => "wic_sun_fog.png",
                WeatherIcons.DAY_HAIL => "wic_hail.png",
                WeatherIcons.DAY_HAZE => "wic_sun_fog.png",
                WeatherIcons.DAY_LIGHTNING => "wic_sun_cloud_lightning.png",
                WeatherIcons.DAY_PARTLY_CLOUDY => "wic_sun_cloud.png",
                WeatherIcons.DAY_RAIN => "wic_sun_cloud_rain.png",
                WeatherIcons.DAY_RAIN_MIX => "wic_sun_cloud_rain.png",
                WeatherIcons.DAY_RAIN_WIND => "wic_sun_cloud_rain.png",
                WeatherIcons.DAY_SHOWERS => "wic_sun_cloud_rain.png",
                WeatherIcons.DAY_SLEET => "wic_sun_cloud_rain.png",
                WeatherIcons.DAY_SLEET_STORM => "wic_sun_cloud_rain.png",
                WeatherIcons.DAY_SNOW => "wic_sun_cloud_snow.png",
                WeatherIcons.DAY_SNOW_THUNDERSTORM => "wic_sun_cloud_snow.png",
                WeatherIcons.DAY_SNOW_WIND => "wic_sun_cloud_snow.png",
                WeatherIcons.DAY_SPRINKLE => "wic_sun_cloud_rain.png",
                WeatherIcons.DAY_STORM_SHOWERS => "wic_sun_cloud_lightning.png",
                WeatherIcons.DAY_SUNNY_OVERCAST => "wic_sun_cloud.png",
                WeatherIcons.DAY_THUNDERSTORM => "wic_sun_cloud_lightning.png",
                WeatherIcons.DAY_WINDY => "wic_wind.png",
                WeatherIcons.DAY_HOT => "wic_sun.png",
                WeatherIcons.DAY_CLOUDY_HIGH => "wic_sun_cloud.png",
                WeatherIcons.DAY_LIGHT_WIND => "wic_wind.png",

                // Night
                WeatherIcons.NIGHT_CLEAR => "wic_moon.png",
                WeatherIcons.NIGHT_ALT_CLOUDY => "wic_moon_cloud.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS => "wic_moon_cloud_wind.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY => "wic_moon_cloud_wind.png",
                WeatherIcons.NIGHT_ALT_HAIL => "wic_hail.png",
                WeatherIcons.NIGHT_ALT_LIGHTNING => "wic_moon_cloud_lightning.png",
                WeatherIcons.NIGHT_ALT_RAIN => "wic_moon_cloud_rain.png",
                WeatherIcons.NIGHT_ALT_RAIN_MIX => "wic_moon_cloud_rain.png",
                WeatherIcons.NIGHT_ALT_RAIN_WIND => "wic_moon_cloud_rain.png",
                WeatherIcons.NIGHT_ALT_SHOWERS => "wic_moon_cloud_rain.png",
                WeatherIcons.NIGHT_ALT_SLEET => "wic_moon_cloud_rain.png",
                WeatherIcons.NIGHT_ALT_SLEET_STORM => "wic_moon_cloud_rain.png",
                WeatherIcons.NIGHT_ALT_SNOW => "wic_moon_cloud_snow.png",
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM => "wic_moon_cloud_snow.png",
                WeatherIcons.NIGHT_ALT_SNOW_WIND => "wic_moon_cloud_snow.png",
                WeatherIcons.NIGHT_ALT_SPRINKLE => "wic_moon_cloud_rain.png",
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS => "wic_moon_cloud_lightning.png",
                WeatherIcons.NIGHT_ALT_THUNDERSTORM => "wic_moon_cloud_lightning.png",
                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY => "wic_moon_cloud.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "wic_moon_cloud.png",
                WeatherIcons.NIGHT_FOG => "wic_moon_fog.png",
                WeatherIcons.NIGHT_OVERCAST => "wic_moon_cloud.png",
                WeatherIcons.NIGHT_HAZE => "wic_moon_fog.png",
                WeatherIcons.NIGHT_WINDY => "wic_wind.png",
                WeatherIcons.NIGHT_HOT => "wic_moon.png",
                WeatherIcons.NIGHT_LIGHT_WIND => "wic_wind.png",

                // Neutral
                WeatherIcons.CLOUD => "wic_cloud.png",
                WeatherIcons.CLOUDY => "wic_clouds.png",
                WeatherIcons.CLOUDY_GUSTS => "wic_cloud_wind.png",
                WeatherIcons.CLOUDY_WINDY => "wic_cloud_wind.png",
                WeatherIcons.FOG => "wic_fog.png",
                WeatherIcons.HAIL => "wic_hail.png",
                WeatherIcons.HAZE => "wic_fog.png",
                WeatherIcons.HOT => "wic_thermometer_hot.png",
                WeatherIcons.LIGHT_WIND => "wic_wind.png",
                WeatherIcons.RAIN => "wic_cloud_rain.png",
                WeatherIcons.RAIN_MIX => "wic_cloud_rain.png",
                WeatherIcons.RAIN_WIND => "wic_cloud_rain.png",
                WeatherIcons.OVERCAST => "wic_clouds.png",
                WeatherIcons.SHOWERS => "wic_cloud_rain.png",
                WeatherIcons.SLEET => "wic_cloud_rain.png",
                WeatherIcons.SLEET_STORM => "wic_cloud_rain.png",
                WeatherIcons.SNOW => "wic_cloud_snow.png",
                WeatherIcons.SNOW_THUNDERSTORM => "wic_cloud_snow.png",
                WeatherIcons.SPRINKLE => "wic_cloud_rain_single.png",
                WeatherIcons.STORM_SHOWERS => "wic_lightning.png",
                WeatherIcons.THUNDERSTORM => "wic_lightning.png",
                WeatherIcons.SNOW_WIND => "wic_cloud_snow.png",
                WeatherIcons.SMOG => "wi_smog.png",
                WeatherIcons.SMOKE => "wi_smoke.png",
                WeatherIcons.LIGHTNING => "wic_lightning.png",
                WeatherIcons.DUST => "wi_dust.png",
                WeatherIcons.SNOWFLAKE_COLD => "wic_snowflake.png",
                WeatherIcons.WINDY => "wic_wind.png",
                WeatherIcons.STRONG_WIND => "wic_wind_high.png",
                WeatherIcons.SANDSTORM => "wi_sandstorm.png",
                WeatherIcons.HURRICANE => "wi_hurricane.png",
                WeatherIcons.TORNADO => "wic_tornado.png",
                WeatherIcons.FIRE => "wi_fire.png",
                WeatherIcons.FLOOD => "wi_flood.png",
                WeatherIcons.VOLCANO => "wi_volcano.png",
                WeatherIcons.BAROMETER => "wic_barometer.png",
                WeatherIcons.HUMIDITY => "wic_raindrop.png",
                WeatherIcons.MOONRISE => "wi_moonrise.png",
                WeatherIcons.MOONSET => "wi_moonset.png",
                WeatherIcons.RAINDROP => "wi_raindrop.png",
                WeatherIcons.RAINDROPS => "wi_raindrops.png",
                WeatherIcons.SUNRISE => "wic_sunrise.png",
                WeatherIcons.SUNSET => "wic_sunset.png",
                WeatherIcons.THERMOMETER => "wic_thermometer_medium.png",
                WeatherIcons.UMBRELLA => "wic_umbrella.png",
                WeatherIcons.WIND_DIRECTION => "wic_compass.png",
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
                WeatherIcons.MOON_NEW => "wic_moon_fullmoon.png",
                WeatherIcons.MOON_WAXING_CRESCENT_3 => "wic_moon_waxing_crescent.png",
                WeatherIcons.MOON_FIRST_QUARTER => "wic_moon_first_quarter.png",
                WeatherIcons.MOON_WAXING_GIBBOUS_3 => "wic_moon_waxing_gibbous.png",
                WeatherIcons.MOON_FULL => "wic_moon_newmoon.png",
                WeatherIcons.MOON_WANING_GIBBOUS_3 => "wic_moon_waning_gibbous.png",
                WeatherIcons.MOON_THIRD_QUARTER => "wic_moon_last_quarter.png",
                WeatherIcons.MOON_WANING_CRESCENT_3 => "wic_moon_waning_crescent.png",

                WeatherIcons.MOON_ALT_NEW => "wic_moon_fullmoon.png",
                WeatherIcons.MOON_ALT_WAXING_CRESCENT_3 => "wic_moon_waxing_crescent.png",
                WeatherIcons.MOON_ALT_FIRST_QUARTER => "wic_moon_first_quarter.png",
                WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3 => "wic_moon_waxing_gibbous.png",
                WeatherIcons.MOON_ALT_FULL => "wic_moon_newmoon.png",
                WeatherIcons.MOON_ALT_WANING_GIBBOUS_3 => "wic_moon_waning_gibbous.png",
                WeatherIcons.MOON_ALT_THIRD_QUARTER => "wic_moon_last_quarter.png",
                WeatherIcons.MOON_ALT_WANING_CRESCENT_3 => "wic_moon_waning_crescent.png",

                WeatherIcons.FAHRENHEIT => "wic_fahrenheit.png",
                WeatherIcons.CELSIUS => "wic_celsius.png",

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
                WeatherIcons.UV_INDEX_11 => "wic_sun.png",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.png",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.png",
                WeatherIcons.RAGWEED_POLLEN => "ic_ragweed_pollen.png",

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
                WeatherIcons.DAY_SUNNY => "wic_sun.svg",
                WeatherIcons.DAY_CLOUDY => "wic_sun_cloud.svg",
                WeatherIcons.DAY_CLOUDY_GUSTS => "wic_sun_cloud_wind.svg",
                WeatherIcons.DAY_CLOUDY_WINDY => "wic_sun_cloud_wind.svg",
                WeatherIcons.DAY_FOG => "wic_sun_fog.svg",
                WeatherIcons.DAY_HAIL => "wic_hail.svg",
                WeatherIcons.DAY_HAZE => "wic_sun_fog.svg",
                WeatherIcons.DAY_LIGHTNING => "wic_sun_cloud_lightning.svg",
                WeatherIcons.DAY_PARTLY_CLOUDY => "wic_sun_cloud.svg",
                WeatherIcons.DAY_RAIN => "wic_sun_cloud_rain.svg",
                WeatherIcons.DAY_RAIN_MIX => "wic_sun_cloud_rain.svg",
                WeatherIcons.DAY_RAIN_WIND => "wic_sun_cloud_rain.svg",
                WeatherIcons.DAY_SHOWERS => "wic_sun_cloud_rain.svg",
                WeatherIcons.DAY_SLEET => "wic_sun_cloud_rain.svg",
                WeatherIcons.DAY_SLEET_STORM => "wic_sun_cloud_rain.svg",
                WeatherIcons.DAY_SNOW => "wic_sun_cloud_snow.svg",
                WeatherIcons.DAY_SNOW_THUNDERSTORM => "wic_sun_cloud_snow.svg",
                WeatherIcons.DAY_SNOW_WIND => "wic_sun_cloud_snow.svg",
                WeatherIcons.DAY_SPRINKLE => "wic_sun_cloud_rain.svg",
                WeatherIcons.DAY_STORM_SHOWERS => "wic_sun_cloud_lightning.svg",
                WeatherIcons.DAY_SUNNY_OVERCAST => "wic_sun_cloud.svg",
                WeatherIcons.DAY_THUNDERSTORM => "wic_sun_cloud_lightning.svg",
                WeatherIcons.DAY_WINDY => "wic_wind.svg",
                WeatherIcons.DAY_HOT => "wic_sun.svg",
                WeatherIcons.DAY_CLOUDY_HIGH => "wic_sun_cloud.svg",
                WeatherIcons.DAY_LIGHT_WIND => "wic_wind.svg",

                // Night
                WeatherIcons.NIGHT_CLEAR => "wic_moon.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY => "wic_moon_cloud.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS => "wic_moon_cloud_wind.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY => "wic_moon_cloud_wind.svg",
                WeatherIcons.NIGHT_ALT_HAIL => "wic_hail.svg",
                WeatherIcons.NIGHT_ALT_LIGHTNING => "wic_moon_cloud_lightning.svg",
                WeatherIcons.NIGHT_ALT_RAIN => "wic_moon_cloud_rain.svg",
                WeatherIcons.NIGHT_ALT_RAIN_MIX => "wic_moon_cloud_rain.svg",
                WeatherIcons.NIGHT_ALT_RAIN_WIND => "wic_moon_cloud_rain.svg",
                WeatherIcons.NIGHT_ALT_SHOWERS => "wic_moon_cloud_rain.svg",
                WeatherIcons.NIGHT_ALT_SLEET => "wic_moon_cloud_rain.svg",
                WeatherIcons.NIGHT_ALT_SLEET_STORM => "wic_moon_cloud_rain.svg",
                WeatherIcons.NIGHT_ALT_SNOW => "wic_moon_cloud_snow.svg",
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM => "wic_moon_cloud_snow.svg",
                WeatherIcons.NIGHT_ALT_SNOW_WIND => "wic_moon_cloud_snow.svg",
                WeatherIcons.NIGHT_ALT_SPRINKLE => "wic_moon_cloud_rain.svg",
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS => "wic_moon_cloud_lightning.svg",
                WeatherIcons.NIGHT_ALT_THUNDERSTORM => "wic_moon_cloud_lightning.svg",
                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY => "wic_moon_cloud.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "wic_moon_cloud.svg",
                WeatherIcons.NIGHT_FOG => "wic_moon_fog.svg",
                WeatherIcons.NIGHT_OVERCAST => "wic_moon_cloud.svg",
                WeatherIcons.NIGHT_HAZE => "wic_moon_fog.svg",
                WeatherIcons.NIGHT_WINDY => "wic_wind.svg",
                WeatherIcons.NIGHT_HOT => "wic_moon.svg",
                WeatherIcons.NIGHT_LIGHT_WIND => "wic_wind.svg",

                // Neutral
                WeatherIcons.CLOUD => "wic_cloud.svg",
                WeatherIcons.CLOUDY => "wic_clouds.svg",
                WeatherIcons.CLOUDY_GUSTS => "wic_cloud_wind.svg",
                WeatherIcons.CLOUDY_WINDY => "wic_cloud_wind.svg",
                WeatherIcons.FOG => "wic_fog.svg",
                WeatherIcons.HAIL => "wic_hail.svg",
                WeatherIcons.HAZE => "wic_fog.svg",
                WeatherIcons.HOT => "wic_thermometer_hot.svg",
                WeatherIcons.LIGHT_WIND => "wic_wind.svg",
                WeatherIcons.RAIN => "wic_cloud_rain.svg",
                WeatherIcons.RAIN_MIX => "wic_cloud_rain.svg",
                WeatherIcons.RAIN_WIND => "wic_cloud_rain.svg",
                WeatherIcons.OVERCAST => "wic_clouds.svg",
                WeatherIcons.SHOWERS => "wic_cloud_rain.svg",
                WeatherIcons.SLEET => "wic_cloud_rain.svg",
                WeatherIcons.SLEET_STORM => "wic_cloud_rain.svg",
                WeatherIcons.SNOW => "wic_cloud_snow.svg",
                WeatherIcons.SNOW_THUNDERSTORM => "wic_cloud_snow.svg",
                WeatherIcons.SPRINKLE => "wic_cloud_rain_single.svg",
                WeatherIcons.STORM_SHOWERS => "wic_lightning.svg",
                WeatherIcons.THUNDERSTORM => "wic_lightning.svg",
                WeatherIcons.SNOW_WIND => "wic_cloud_snow.svg",
                WeatherIcons.SMOG => "wi_smog.svg",
                WeatherIcons.SMOKE => "wi_smoke.svg",
                WeatherIcons.LIGHTNING => "wic_lightning.svg",
                WeatherIcons.DUST => "wi_dust.svg",
                WeatherIcons.SNOWFLAKE_COLD => "wic_snowflake.svg",
                WeatherIcons.WINDY => "wic_wind.svg",
                WeatherIcons.STRONG_WIND => "wic_wind_high.svg",
                WeatherIcons.SANDSTORM => "wi_sandstorm.svg",
                WeatherIcons.HURRICANE => "wi_hurricane.svg",
                WeatherIcons.TORNADO => "wic_tornado.svg",
                WeatherIcons.FIRE => "wi_fire.svg",
                WeatherIcons.FLOOD => "wi_flood.svg",
                WeatherIcons.VOLCANO => "wi_volcano.svg",
                WeatherIcons.BAROMETER => "wic_barometer.svg",
                WeatherIcons.HUMIDITY => "wic_raindrop.svg",
                WeatherIcons.MOONRISE => "wi_moonrise.svg",
                WeatherIcons.MOONSET => "wi_moonset.svg",
                WeatherIcons.RAINDROP => "wi_raindrop.svg",
                WeatherIcons.RAINDROPS => "wi_raindrops.svg",
                WeatherIcons.SUNRISE => "wic_sunrise.svg",
                WeatherIcons.SUNSET => "wic_sunset.svg",
                WeatherIcons.THERMOMETER => "wic_thermometer_medium.svg",
                WeatherIcons.UMBRELLA => "wic_umbrella.svg",
                WeatherIcons.WIND_DIRECTION => "wic_compass.svg",
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
                WeatherIcons.MOON_NEW => "wic_moon_fullmoon.svg",
                WeatherIcons.MOON_WAXING_CRESCENT_3 => "wic_moon_waxing_crescent.svg",
                WeatherIcons.MOON_FIRST_QUARTER => "wic_moon_first_quarter.svg",
                WeatherIcons.MOON_WAXING_GIBBOUS_3 => "wic_moon_waxing_gibbous.svg",
                WeatherIcons.MOON_FULL => "wic_moon_newmoon.svg",
                WeatherIcons.MOON_WANING_GIBBOUS_3 => "wic_moon_waning_gibbous.svg",
                WeatherIcons.MOON_THIRD_QUARTER => "wic_moon_last_quarter.svg",
                WeatherIcons.MOON_WANING_CRESCENT_3 => "wic_moon_waning_crescent.svg",

                WeatherIcons.MOON_ALT_NEW => "wic_moon_fullmoon.svg",
                WeatherIcons.MOON_ALT_WAXING_CRESCENT_3 => "wic_moon_waxing_crescent.svg",
                WeatherIcons.MOON_ALT_FIRST_QUARTER => "wic_moon_first_quarter.svg",
                WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3 => "wic_moon_waxing_gibbous.svg",
                WeatherIcons.MOON_ALT_FULL => "wic_moon_newmoon.svg",
                WeatherIcons.MOON_ALT_WANING_GIBBOUS_3 => "wic_moon_waning_gibbous.svg",
                WeatherIcons.MOON_ALT_THIRD_QUARTER => "wic_moon_last_quarter.svg",
                WeatherIcons.MOON_ALT_WANING_CRESCENT_3 => "wic_moon_waning_crescent.svg",

                WeatherIcons.FAHRENHEIT => "wic_fahrenheit.svg",
                WeatherIcons.CELSIUS => "wic_celsius.svg",

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
                WeatherIcons.UV_INDEX_11 => "wic_sun.svg",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.svg",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.svg",
                WeatherIcons.RAGWEED_POLLEN => "ic_ragweed_pollen.svg",

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
