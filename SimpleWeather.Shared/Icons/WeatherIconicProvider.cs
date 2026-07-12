using System;

namespace SimpleWeather.Icons
{
    internal sealed class WeatherIconicProvider(bool isColored = false) : WeatherIconProvider
    {
        public override string Key => "w-iconic-jackd248" + (isColored ? "-multicolor" : "");
        public override string DisplayName => "Weather Iconic" + (isColored ? " (Multi-Color)" : "");

        public override string AuthorName => "konradmichalik";

        public override Uri AttributionLink => new("https://konradmichalik.github.io/weather-iconic/");

        public override bool IsFontIcon => !isColored;

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
                WeatherIcons.DAY_SUNNY => isColored ? "wic_color_sun.png" : "wic_sun.png",
                WeatherIcons.DAY_CLOUDY => isColored ? "wic_color_sun_cloud.png" : "wic_sun_cloud.png",
                WeatherIcons.DAY_CLOUDY_GUSTS => isColored ? "wic_color_sun_cloud_wind.png" : "wic_sun_cloud_wind.png",
                WeatherIcons.DAY_CLOUDY_WINDY => isColored ? "wic_color_sun_cloud_wind.png" : "wic_sun_cloud_wind.png",
                WeatherIcons.DAY_FOG => isColored ? "wic_color_sun_fog.png" : "wic_sun_fog.png",
                WeatherIcons.DAY_HAIL => isColored ? "wic_color_sun_cloud_hail.png" : "wic_sun_cloud_hail.png",
                WeatherIcons.DAY_HAZE => isColored ? "wic_color_sun_cloud_haze.png" : "wic_sun_cloud_haze.png",
                WeatherIcons.DAY_LIGHTNING => isColored ? "wic_color_sun_cloud_lightning.png" : "wic_sun_cloud_lightning.png",
                WeatherIcons.DAY_PARTLY_CLOUDY => isColored ? "wic_color_sun_cloud.png" : "wic_sun_cloud.png",
                WeatherIcons.DAY_RAIN => isColored ? "wic_color_sun_cloud_rain.png" : "wic_sun_cloud_rain.png",
                WeatherIcons.DAY_RAIN_MIX => isColored ? "wic_color_sun_cloud_rain.png" : "wic_sun_cloud_rain.png",
                WeatherIcons.DAY_RAIN_WIND => isColored ? "wic_color_sun_cloud_rain.png" : "wic_sun_cloud_rain.png",
                WeatherIcons.DAY_SHOWERS => isColored ? "wic_color_sun_cloud_rain.png" : "wic_sun_cloud_rain.png",
                WeatherIcons.DAY_SLEET => isColored ? "wic_color_sun_cloud_rain.png" : "wic_sun_cloud_rain.png",
                WeatherIcons.DAY_SLEET_STORM => isColored ? "wic_color_sun_cloud_rain.png" : "wic_sun_cloud_rain.png",
                WeatherIcons.DAY_SNOW => isColored ? "wic_color_sun_cloud_snow_alt.png" : "wic_sun_cloud_snow_alt.png",
                WeatherIcons.DAY_SNOW_THUNDERSTORM => isColored ? "wic_color_sun_cloud_snow.png" : "wic_sun_cloud_snow.png",
                WeatherIcons.DAY_SNOW_WIND => isColored ? "wic_color_sun_cloud_snow.png" : "wic_sun_cloud_snow.png",
                WeatherIcons.DAY_SPRINKLE => isColored ? "wic_color_sun_cloud_rain_alt.png" : "wic_sun_cloud_rain_alt.png",
                WeatherIcons.DAY_STORM_SHOWERS => isColored ? "wic_color_sun_cloud_lightning.png" : "wic_sun_cloud_lightning.png",
                WeatherIcons.DAY_SUNNY_OVERCAST => isColored ? "wic_color_sun_cloud.png" : "wic_sun_cloud.png",
                WeatherIcons.DAY_THUNDERSTORM => isColored ? "wic_color_sun_cloud_lightning.png" : "wic_sun_cloud_lightning.png",
                WeatherIcons.DAY_WINDY => isColored ? "wic_color_wind.png" : "wic_wind.png",
                WeatherIcons.DAY_HOT => isColored ? "wic_color_sun.png" : "wic_sun.png",
                WeatherIcons.DAY_CLOUDY_HIGH => isColored ? "wic_color_sun_cloud.png" : "wic_sun_cloud.png",
                WeatherIcons.DAY_LIGHT_WIND => isColored ? "wic_color_wind.png" : "wic_wind.png",

                // Night
                WeatherIcons.NIGHT_CLEAR => isColored ? "wic_color_moon.png" : "wic_moon.png",
                WeatherIcons.NIGHT_ALT_CLOUDY => isColored ? "wic_color_moon_cloud.png" : "wic_moon_cloud.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS => isColored ? "wic_color_moon_cloud_wind.png" : "wic_moon_cloud_wind.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY => isColored ? "wic_color_moon_cloud_wind.png" : "wic_moon_cloud_wind.png",
                WeatherIcons.NIGHT_ALT_HAIL => isColored ? "wic_color_moon_cloud_hail.png" : "wic_moon_cloud_hail.png",
                WeatherIcons.NIGHT_ALT_LIGHTNING => isColored ? "wic_color_moon_cloud_lightning.png" : "wic_moon_cloud_lightning.png",
                WeatherIcons.NIGHT_ALT_RAIN => isColored ? "wic_color_moon_cloud_rain.png" : "wic_moon_cloud_rain.png",
                WeatherIcons.NIGHT_ALT_RAIN_MIX => isColored ? "wic_color_moon_cloud_rain.png" : "wic_moon_cloud_rain.png",
                WeatherIcons.NIGHT_ALT_RAIN_WIND => isColored ? "wic_color_moon_cloud_rain.png" : "wic_moon_cloud_rain.png",
                WeatherIcons.NIGHT_ALT_SHOWERS => isColored ? "wic_color_moon_cloud_rain.png" : "wic_moon_cloud_rain.png",
                WeatherIcons.NIGHT_ALT_SLEET => isColored ? "wic_color_moon_cloud_sleet.png" : "wic_moon_cloud_sleet.png",
                WeatherIcons.NIGHT_ALT_SLEET_STORM => isColored ? "wic_color_moon_cloud_sleet.png" : "wic_moon_cloud_sleet.png",
                WeatherIcons.NIGHT_ALT_SNOW => isColored ? "wic_color_moon_cloud_snow_alt.png" : "wic_moon_cloud_snow_alt.png",
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM => isColored ? "wic_color_moon_cloud_snow.png" : "wic_moon_cloud_snow.png",
                WeatherIcons.NIGHT_ALT_SNOW_WIND => isColored ? "wic_color_moon_cloud_snow.png" : "wic_moon_cloud_snow.png",
                WeatherIcons.NIGHT_ALT_SPRINKLE => isColored ? "wic_color_moon_cloud_rain_alt.png" : "wic_moon_cloud_rain_alt.png",
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS => isColored ? "wic_color_moon_cloud_lightning.png" : "wic_moon_cloud_lightning.png",
                WeatherIcons.NIGHT_ALT_THUNDERSTORM => isColored ? "wic_color_moon_cloud_lightning.png" : "wic_moon_cloud_lightning.png",
                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY => isColored ? "wic_color_moon_cloud.png" : "wic_moon_cloud.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => isColored ? "wic_color_moon_cloud.png" : "wic_moon_cloud.png",
                WeatherIcons.NIGHT_FOG => isColored ? "wic_color_moon_fog.png" : "wic_moon_fog.png",
                WeatherIcons.NIGHT_OVERCAST => isColored ? "wic_color_moon_cloud.png" : "wic_moon_cloud.png",
                WeatherIcons.NIGHT_HAZE => isColored ? "wic_color_moon_cloud_haze.png" : "wic_moon_cloud_haze.png",
                WeatherIcons.NIGHT_WINDY => isColored ? "wic_color_wind.png" : "wic_wind.png",
                WeatherIcons.NIGHT_HOT => isColored ? "wic_color_moon.png" : "wic_moon.png",
                WeatherIcons.NIGHT_LIGHT_WIND => isColored ? "wic_color_wind.png" : "wic_wind.png",

                // Neutral
                WeatherIcons.CLOUD => isColored ? "wic_color_cloud.png" : "wic_cloud.png",
                WeatherIcons.CLOUDY => isColored ? "wic_color_clouds.png" : "wic_clouds.png",
                WeatherIcons.CLOUDY_GUSTS => isColored ? "wic_color_cloud_wind.png" : "wic_cloud_wind.png",
                WeatherIcons.CLOUDY_WINDY => isColored ? "wic_color_cloud_wind.png" : "wic_cloud_wind.png",
                WeatherIcons.FOG => isColored ? "wic_color_fog.png" : "wic_fog.png",
                WeatherIcons.HAIL => isColored ? "wic_color_cloud_hail.png" : "wic_cloud_hail.png",
                WeatherIcons.HAZE => isColored ? "wic_color_cloud_haze.png" : "wic_cloud_haze.png",
                WeatherIcons.HOT => isColored ? "wic_color_thermometer_heat.png" : "wic_thermometer_heat.png",
                WeatherIcons.LIGHT_WIND => isColored ? "wic_color_wind.png" : "wic_wind.png",
                WeatherIcons.RAIN => isColored ? "wic_color_cloud_rain.png" : "wic_cloud_rain.png",
                WeatherIcons.RAIN_MIX => isColored ? "wic_color_cloud_sleet.png" : "wic_cloud_sleet.png",
                WeatherIcons.RAIN_WIND => isColored ? "wic_color_cloud_rain.png" : "wic_cloud_rain.png",
                WeatherIcons.OVERCAST => isColored ? "wic_color_clouds_alt.png" : "wic_clouds_alt.png",
                WeatherIcons.SHOWERS => isColored ? "wic_color_cloud_rain.png" : "wic_cloud_rain.png",
                WeatherIcons.SLEET => isColored ? "wic_color_cloud_sleet.png" : "wic_cloud_sleet.png",
                WeatherIcons.SLEET_STORM => isColored ? "wic_color_cloud_sleet.png" : "wic_cloud_sleet.png",
                WeatherIcons.SNOW => isColored ? "wic_color_cloud_snow_alt.png" : "wic_cloud_snow_alt.png",
                WeatherIcons.SNOW_THUNDERSTORM => isColored ? "wic_color_cloud_snow.png" : "wic_cloud_snow.png",
                WeatherIcons.SPRINKLE => isColored ? "wic_color_cloud_rain_alt.png" : "wic_cloud_rain_alt.png",
                WeatherIcons.STORM_SHOWERS => isColored ? "wic_color_lightning.png" : "wic_lightning.png",
                WeatherIcons.THUNDERSTORM => isColored ? "wic_color_lightning.png" : "wic_lightning.png",
                WeatherIcons.SNOW_WIND => isColored ? "wic_color_cloud_snow.png" : "wic_cloud_snow.png",
                WeatherIcons.SMOG => isColored ? "wic_color_cloud_smog.png" : "wic_cloud_smog.png",
                WeatherIcons.SMOKE => "wi_smoke.png",
                WeatherIcons.LIGHTNING => isColored ? "wic_color_lightning.png" : "wic_lightning.png",
                WeatherIcons.DUST => "wi_dust.png",
                WeatherIcons.SNOWFLAKE_COLD => isColored ? "wic_color_snowflake.png" : "wic_snowflake.png",
                WeatherIcons.WINDY => isColored ? "wic_color_wind.png" : "wic_wind.png",
                WeatherIcons.STRONG_WIND => isColored ? "wic_color_wind_high.png" : "wic_wind_high.png",
                WeatherIcons.SANDSTORM => isColored ? "wic_color_sandstorm.png" : "wic_sandstorm.png",
                WeatherIcons.HURRICANE => isColored ? "wic_color_hurricane.png" : "wic_hurricane.png",
                WeatherIcons.TORNADO => isColored ? "wic_color_tornado.png" : "wic_tornado.png",
                WeatherIcons.FIRE => isColored ? "wic_color_fire.png" : "wic_fire.png",
                WeatherIcons.FLOOD => isColored ? "wic_color_flood.png" : "wic_flood.png",
                WeatherIcons.VOLCANO => "wi_volcano.png",
                WeatherIcons.BAROMETER => isColored ? "wic_color_barometer.png" : "wic_barometer.png",
                WeatherIcons.HUMIDITY => isColored ? "wic_color_humidity.png" : "wic_humidity.png",
                WeatherIcons.MOONRISE => isColored ? "wic_color_moonrise.png" : "wic_moonrise.png",
                WeatherIcons.MOONSET => isColored ? "wic_color_moonset.png" : "wic_moonset.png",
                WeatherIcons.RAINDROP => isColored ? "wic_color_raindrop.png" : "wic_raindrop.png",
                WeatherIcons.RAINDROPS => "wi_raindrops.png",
                WeatherIcons.SUNRISE => isColored ? "wic_color_sunrise.png" : "wic_sunrise.png",
                WeatherIcons.SUNSET => isColored ? "wic_color_sunset.png" : "wic_sunset.png",
                WeatherIcons.THERMOMETER => isColored ? "wic_color_thermometer_medium.png" : "wic_thermometer_medium.png",
                WeatherIcons.UMBRELLA => isColored ? "wic_color_umbrella.png" : "wic_umbrella.png",
                WeatherIcons.WIND_DIRECTION => isColored ? "wic_color_compass.png" : "wic_compass.png",
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
                WeatherIcons.MOON_NEW => isColored ? "wic_color_moon_fullmoon.png" : "wic_moon_fullmoon.png",
                WeatherIcons.MOON_WAXING_CRESCENT_3 => isColored ? "wic_color_moon_waxing_crescent.png" : "wic_moon_waxing_crescent.png",
                WeatherIcons.MOON_FIRST_QUARTER => isColored ? "wic_color_moon_first_quarter.png" : "wic_moon_first_quarter.png",
                WeatherIcons.MOON_WAXING_GIBBOUS_3 => isColored ? "wic_color_moon_waxing_gibbous.png" : "wic_moon_waxing_gibbous.png",
                WeatherIcons.MOON_FULL => isColored ? "wic_color_moon_newmoon.png" : "wic_moon_newmoon.png",
                WeatherIcons.MOON_WANING_GIBBOUS_3 => isColored ? "wic_color_moon_waning_gibbous.png" : "wic_moon_waning_gibbous.png",
                WeatherIcons.MOON_THIRD_QUARTER => isColored ? "wic_color_moon_last_quarter.png" : "wic_moon_last_quarter.png",
                WeatherIcons.MOON_WANING_CRESCENT_3 => isColored ? "wic_color_moon_waning_crescent.png" : "wic_moon_waning_crescent.png",

                WeatherIcons.MOON_ALT_NEW => isColored ? "wic_color_moon_fullmoon.png" : "wic_moon_fullmoon.png",
                WeatherIcons.MOON_ALT_WAXING_CRESCENT_3 => isColored ? "wic_color_moon_waxing_crescent.png" : "wic_moon_waxing_crescent.png",
                WeatherIcons.MOON_ALT_FIRST_QUARTER => isColored ? "wic_color_moon_first_quarter.png" : "wic_moon_first_quarter.png",
                WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3 => isColored ? "wic_color_moon_waxing_gibbous.png" : "wic_moon_waxing_gibbous.png",
                WeatherIcons.MOON_ALT_FULL => isColored ? "wic_color_moon_newmoon.png" : "wic_moon_newmoon.png",
                WeatherIcons.MOON_ALT_WANING_GIBBOUS_3 => isColored ? "wic_color_moon_waning_gibbous.png" : "wic_moon_waning_gibbous.png",
                WeatherIcons.MOON_ALT_THIRD_QUARTER => isColored ? "wic_color_moon_last_quarter.png" : "wic_moon_last_quarter.png",
                WeatherIcons.MOON_ALT_WANING_CRESCENT_3 => isColored ? "wic_color_moon_waning_crescent.png" : "wic_moon_waning_crescent.png",

                WeatherIcons.FAHRENHEIT => isColored ? "wic_color_fahrenheit.png" : "wic_fahrenheit.png",
                WeatherIcons.CELSIUS => isColored ? "wic_color_celsius.png" : "wic_celsius.png",

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
                WeatherIcons.UV_INDEX_11 => isColored ? "wic_color_sun.png" : "wic_sun.png",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.png",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.png",
                WeatherIcons.RAGWEED_POLLEN => isColored ? "wic_color_pollen.png" : "wic_pollen.png",

                WeatherIcons.NA => isColored ? "wic_color_na.png" : "wic_na.png",

                _ => string.Empty,
            };

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = isColored ? "wic_color_na.png" : "wic_na.png";
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
                WeatherIcons.DAY_SUNNY => isColored ? "wic_color_sun.svg" : "wic_sun.svg",
                WeatherIcons.DAY_CLOUDY => isColored ? "wic_color_sun_cloud.svg" : "wic_sun_cloud.svg",
                WeatherIcons.DAY_CLOUDY_GUSTS => isColored ? "wic_color_sun_cloud_wind.svg" : "wic_sun_cloud_wind.svg",
                WeatherIcons.DAY_CLOUDY_WINDY => isColored ? "wic_color_sun_cloud_wind.svg" : "wic_sun_cloud_wind.svg",
                WeatherIcons.DAY_FOG => isColored ? "wic_color_sun_fog.svg" : "wic_sun_fog.svg",
                WeatherIcons.DAY_HAIL => isColored ? "wic_color_sun_cloud_hail.svg" : "wic_sun_cloud_hail.svg",
                WeatherIcons.DAY_HAZE => isColored ? "wic_color_sun_cloud_haze.svg" : "wic_sun_cloud_haze.svg",
                WeatherIcons.DAY_LIGHTNING => isColored ? "wic_color_sun_cloud_lightning.svg" : "wic_sun_cloud_lightning.svg",
                WeatherIcons.DAY_PARTLY_CLOUDY => isColored ? "wic_color_sun_cloud.svg" : "wic_sun_cloud.svg",
                WeatherIcons.DAY_RAIN => isColored ? "wic_color_sun_cloud_rain.svg" : "wic_sun_cloud_rain.svg",
                WeatherIcons.DAY_RAIN_MIX => isColored ? "wic_color_sun_cloud_rain.svg" : "wic_sun_cloud_rain.svg",
                WeatherIcons.DAY_RAIN_WIND => isColored ? "wic_color_sun_cloud_rain.svg" : "wic_sun_cloud_rain.svg",
                WeatherIcons.DAY_SHOWERS => isColored ? "wic_color_sun_cloud_rain.svg" : "wic_sun_cloud_rain.svg",
                WeatherIcons.DAY_SLEET => isColored ? "wic_color_sun_cloud_rain.svg" : "wic_sun_cloud_rain.svg",
                WeatherIcons.DAY_SLEET_STORM => isColored ? "wic_color_sun_cloud_rain.svg" : "wic_sun_cloud_rain.svg",
                WeatherIcons.DAY_SNOW => isColored ? "wic_color_sun_cloud_snow_alt.svg" : "wic_sun_cloud_snow_alt.svg",
                WeatherIcons.DAY_SNOW_THUNDERSTORM => isColored ? "wic_color_sun_cloud_snow.svg" : "wic_sun_cloud_snow.svg",
                WeatherIcons.DAY_SNOW_WIND => isColored ? "wic_color_sun_cloud_snow.svg" : "wic_sun_cloud_snow.svg",
                WeatherIcons.DAY_SPRINKLE => isColored ? "wic_color_sun_cloud_rain_alt.svg" : "wic_sun_cloud_rain_alt.svg",
                WeatherIcons.DAY_STORM_SHOWERS => isColored ? "wic_color_sun_cloud_lightning.svg" : "wic_sun_cloud_lightning.svg",
                WeatherIcons.DAY_SUNNY_OVERCAST => isColored ? "wic_color_sun_cloud.svg" : "wic_sun_cloud.svg",
                WeatherIcons.DAY_THUNDERSTORM => isColored ? "wic_color_sun_cloud_lightning.svg" : "wic_sun_cloud_lightning.svg",
                WeatherIcons.DAY_WINDY => isColored ? "wic_color_wind.svg" : "wic_wind.svg",
                WeatherIcons.DAY_HOT => isColored ? "wic_color_sun.svg" : "wic_sun.svg",
                WeatherIcons.DAY_CLOUDY_HIGH => isColored ? "wic_color_sun_cloud.svg" : "wic_sun_cloud.svg",
                WeatherIcons.DAY_LIGHT_WIND => isColored ? "wic_color_wind.svg" : "wic_wind.svg",

                // Night
                WeatherIcons.NIGHT_CLEAR => isColored ? "wic_color_moon.svg" : "wic_moon.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY => isColored ? "wic_color_moon_cloud.svg" : "wic_moon_cloud.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS => isColored ? "wic_color_moon_cloud_wind.svg" : "wic_moon_cloud_wind.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY => isColored ? "wic_color_moon_cloud_wind.svg" : "wic_moon_cloud_wind.svg",
                WeatherIcons.NIGHT_ALT_HAIL => isColored ? "wic_color_moon_cloud_hail.svg" : "wic_moon_cloud_hail.svg",
                WeatherIcons.NIGHT_ALT_LIGHTNING => isColored ? "wic_color_moon_cloud_lightning.svg" : "wic_moon_cloud_lightning.svg",
                WeatherIcons.NIGHT_ALT_RAIN => isColored ? "wic_color_moon_cloud_rain.svg" : "wic_moon_cloud_rain.svg",
                WeatherIcons.NIGHT_ALT_RAIN_MIX => isColored ? "wic_color_moon_cloud_rain.svg" : "wic_moon_cloud_rain.svg",
                WeatherIcons.NIGHT_ALT_RAIN_WIND => isColored ? "wic_color_moon_cloud_rain.svg" : "wic_moon_cloud_rain.svg",
                WeatherIcons.NIGHT_ALT_SHOWERS => isColored ? "wic_color_moon_cloud_rain.svg" : "wic_moon_cloud_rain.svg",
                WeatherIcons.NIGHT_ALT_SLEET => isColored ? "wic_color_moon_cloud_sleet.svg" : "wic_moon_cloud_sleet.svg",
                WeatherIcons.NIGHT_ALT_SLEET_STORM => isColored ? "wic_color_moon_cloud_sleet.svg" : "wic_moon_cloud_sleet.svg",
                WeatherIcons.NIGHT_ALT_SNOW => isColored ? "wic_color_moon_cloud_snow_alt.svg" : "wic_moon_cloud_snow_alt.svg",
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM => isColored ? "wic_color_moon_cloud_snow.svg" : "wic_moon_cloud_snow.svg",
                WeatherIcons.NIGHT_ALT_SNOW_WIND => isColored ? "wic_color_moon_cloud_snow.svg" : "wic_moon_cloud_snow.svg",
                WeatherIcons.NIGHT_ALT_SPRINKLE => isColored ? "wic_color_moon_cloud_rain_alt.svg" : "wic_moon_cloud_rain_alt.svg",
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS => isColored ? "wic_color_moon_cloud_lightning.svg" : "wic_moon_cloud_lightning.svg",
                WeatherIcons.NIGHT_ALT_THUNDERSTORM => isColored ? "wic_color_moon_cloud_lightning.svg" : "wic_moon_cloud_lightning.svg",
                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY => isColored ? "wic_color_moon_cloud.svg" : "wic_moon_cloud.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => isColored ? "wic_color_moon_cloud.svg" : "wic_moon_cloud.svg",
                WeatherIcons.NIGHT_FOG => isColored ? "wic_color_moon_fog.svg" : "wic_moon_fog.svg",
                WeatherIcons.NIGHT_OVERCAST => isColored ? "wic_color_moon_cloud.svg" : "wic_moon_cloud.svg",
                WeatherIcons.NIGHT_HAZE => isColored ? "wic_color_moon_cloud_haze.svg" : "wic_moon_cloud_haze.svg",
                WeatherIcons.NIGHT_WINDY => isColored ? "wic_color_wind.svg" : "wic_wind.svg",
                WeatherIcons.NIGHT_HOT => isColored ? "wic_color_moon.svg" : "wic_moon.svg",
                WeatherIcons.NIGHT_LIGHT_WIND => isColored ? "wic_color_wind.svg" : "wic_wind.svg",

                // Neutral
                WeatherIcons.CLOUD => isColored ? "wic_color_cloud.svg" : "wic_cloud.svg",
                WeatherIcons.CLOUDY => isColored ? "wic_color_clouds.svg" : "wic_clouds.svg",
                WeatherIcons.CLOUDY_GUSTS => isColored ? "wic_color_cloud_wind.svg" : "wic_cloud_wind.svg",
                WeatherIcons.CLOUDY_WINDY => isColored ? "wic_color_cloud_wind.svg" : "wic_cloud_wind.svg",
                WeatherIcons.FOG => isColored ? "wic_color_fog.svg" : "wic_fog.svg",
                WeatherIcons.HAIL => isColored ? "wic_color_cloud_hail.svg" : "wic_cloud_hail.svg",
                WeatherIcons.HAZE => isColored ? "wic_color_cloud_haze.svg" : "wic_cloud_haze.svg",
                WeatherIcons.HOT => isColored ? "wic_color_thermometer_heat.svg" : "wic_thermometer_heat.svg",
                WeatherIcons.LIGHT_WIND => isColored ? "wic_color_wind.svg" : "wic_wind.svg",
                WeatherIcons.RAIN => isColored ? "wic_color_cloud_rain.svg" : "wic_cloud_rain.svg",
                WeatherIcons.RAIN_MIX => isColored ? "wic_color_cloud_sleet.svg" : "wic_cloud_sleet.svg",
                WeatherIcons.RAIN_WIND => isColored ? "wic_color_cloud_rain.svg" : "wic_cloud_rain.svg",
                WeatherIcons.OVERCAST => isColored ? "wic_color_clouds_alt.svg" : "wic_clouds_alt.svg",
                WeatherIcons.SHOWERS => isColored ? "wic_color_cloud_rain.svg" : "wic_cloud_rain.svg",
                WeatherIcons.SLEET => isColored ? "wic_color_cloud_sleet.svg" : "wic_cloud_sleet.svg",
                WeatherIcons.SLEET_STORM => isColored ? "wic_color_cloud_sleet.svg" : "wic_cloud_sleet.svg",
                WeatherIcons.SNOW => isColored ? "wic_color_cloud_snow_alt.svg" : "wic_cloud_snow_alt.svg",
                WeatherIcons.SNOW_THUNDERSTORM => isColored ? "wic_color_cloud_snow.svg" : "wic_cloud_snow.svg",
                WeatherIcons.SPRINKLE => isColored ? "wic_color_cloud_rain_alt.svg" : "wic_cloud_rain_alt.svg",
                WeatherIcons.STORM_SHOWERS => isColored ? "wic_color_lightning.svg" : "wic_lightning.svg",
                WeatherIcons.THUNDERSTORM => isColored ? "wic_color_lightning.svg" : "wic_lightning.svg",
                WeatherIcons.SNOW_WIND => isColored ? "wic_color_cloud_snow.svg" : "wic_cloud_snow.svg",
                WeatherIcons.SMOG => isColored ? "wic_color_cloud_smog.svg" : "wic_cloud_smog.svg",
                WeatherIcons.SMOKE => "wi_smoke.svg",
                WeatherIcons.LIGHTNING => isColored ? "wic_color_lightning.svg" : "wic_lightning.svg",
                WeatherIcons.DUST => "wi_dust.svg",
                WeatherIcons.SNOWFLAKE_COLD => isColored ? "wic_color_snowflake.svg" : "wic_snowflake.svg",
                WeatherIcons.WINDY => isColored ? "wic_color_wind.svg" : "wic_wind.svg",
                WeatherIcons.STRONG_WIND => isColored ? "wic_color_wind_high.svg" : "wic_wind_high.svg",
                WeatherIcons.SANDSTORM => isColored ? "wic_color_sandstorm.svg" : "wic_sandstorm.svg",
                WeatherIcons.HURRICANE => isColored ? "wic_color_hurricane.svg" : "wic_hurricane.svg",
                WeatherIcons.TORNADO => isColored ? "wic_color_tornado.svg" : "wic_tornado.svg",
                WeatherIcons.FIRE => isColored ? "wic_color_fire.svg" : "wic_fire.svg",
                WeatherIcons.FLOOD => isColored ? "wic_color_flood.svg" : "wic_flood.svg",
                WeatherIcons.VOLCANO => "wi_volcano.svg",
                WeatherIcons.BAROMETER => isColored ? "wic_color_barometer.svg" : "wic_barometer.svg",
                WeatherIcons.HUMIDITY => isColored ? "wic_color_humidity.svg" : "wic_humidity.svg",
                WeatherIcons.MOONRISE => isColored ? "wic_color_moonrise.svg" : "wic_moonrise.svg",
                WeatherIcons.MOONSET => isColored ? "wic_color_moonset.svg" : "wic_moonset.svg",
                WeatherIcons.RAINDROP => isColored ? "wic_color_raindrop.svg" : "wic_raindrop.svg",
                WeatherIcons.RAINDROPS => "wi_raindrops.svg",
                WeatherIcons.SUNRISE => isColored ? "wic_color_sunrise.svg" : "wic_sunrise.svg",
                WeatherIcons.SUNSET => isColored ? "wic_color_sunset.svg" : "wic_sunset.svg",
                WeatherIcons.THERMOMETER => isColored ? "wic_color_thermometer_medium.svg" : "wic_thermometer_medium.svg",
                WeatherIcons.UMBRELLA => isColored ? "wic_color_umbrella.svg" : "wic_umbrella.svg",
                WeatherIcons.WIND_DIRECTION => isColored ? "wic_color_compass.svg" : "wic_compass.svg",
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
                WeatherIcons.MOON_NEW => isColored ? "wic_color_moon_fullmoon.svg" : "wic_moon_fullmoon.svg",
                WeatherIcons.MOON_WAXING_CRESCENT_3 => isColored ? "wic_color_moon_waxing_crescent.svg" : "wic_moon_waxing_crescent.svg",
                WeatherIcons.MOON_FIRST_QUARTER => isColored ? "wic_color_moon_first_quarter.svg" : "wic_moon_first_quarter.svg",
                WeatherIcons.MOON_WAXING_GIBBOUS_3 => isColored ? "wic_color_moon_waxing_gibbous.svg" : "wic_moon_waxing_gibbous.svg",
                WeatherIcons.MOON_FULL => isColored ? "wic_color_moon_newmoon.svg" : "wic_moon_newmoon.svg",
                WeatherIcons.MOON_WANING_GIBBOUS_3 => isColored ? "wic_color_moon_waning_gibbous.svg" : "wic_moon_waning_gibbous.svg",
                WeatherIcons.MOON_THIRD_QUARTER => isColored ? "wic_color_moon_last_quarter.svg" : "wic_moon_last_quarter.svg",
                WeatherIcons.MOON_WANING_CRESCENT_3 => isColored ? "wic_color_moon_waning_crescent.svg" : "wic_moon_waning_crescent.svg",

                WeatherIcons.MOON_ALT_NEW => isColored ? "wic_color_moon_fullmoon.svg" : "wic_moon_fullmoon.svg",
                WeatherIcons.MOON_ALT_WAXING_CRESCENT_3 => isColored ? "wic_color_moon_waxing_crescent.svg" : "wic_moon_waxing_crescent.svg",
                WeatherIcons.MOON_ALT_FIRST_QUARTER => isColored ? "wic_color_moon_first_quarter.svg" : "wic_moon_first_quarter.svg",
                WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3 => isColored ? "wic_color_moon_waxing_gibbous.svg" : "wic_moon_waxing_gibbous.svg",
                WeatherIcons.MOON_ALT_FULL => isColored ? "wic_color_moon_newmoon.svg" : "wic_moon_newmoon.svg",
                WeatherIcons.MOON_ALT_WANING_GIBBOUS_3 => isColored ? "wic_color_moon_waning_gibbous.svg" : "wic_moon_waning_gibbous.svg",
                WeatherIcons.MOON_ALT_THIRD_QUARTER => isColored ? "wic_color_moon_last_quarter.svg" : "wic_moon_last_quarter.svg",
                WeatherIcons.MOON_ALT_WANING_CRESCENT_3 => isColored ? "wic_color_moon_waning_crescent.svg" : "wic_moon_waning_crescent.svg",

                WeatherIcons.FAHRENHEIT => isColored ? "wic_color_fahrenheit.svg" : "wic_fahrenheit.svg",
                WeatherIcons.CELSIUS => isColored ? "wic_color_celsius.svg" : "wic_celsius.svg",

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
                WeatherIcons.UV_INDEX_11 => isColored ? "wic_color_sun.svg" : "wic_sun.svg",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.svg",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.svg",
                WeatherIcons.RAGWEED_POLLEN => isColored ? "wic_color_pollen.svg" : "wic_pollen.svg",

                WeatherIcons.NA => isColored ? "wic_color_na.svg" : "wic_na.svg",

                _ => string.Empty,
            };

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = isColored ? "wic_color_na.svg" : "wic_na.svg";
            }

            return baseuri + fileIcon;
        }
    }
}
