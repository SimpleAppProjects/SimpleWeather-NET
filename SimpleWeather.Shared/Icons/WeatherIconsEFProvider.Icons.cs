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
                WeatherIcons.DAY_SUNNY => "wi-day-sunny.png",
                WeatherIcons.DAY_CLOUDY => "wi-day-cloudy.png",
                WeatherIcons.DAY_CLOUDY_GUSTS => "wi-day-cloudy-gusts.png",
                WeatherIcons.DAY_CLOUDY_WINDY => "wi-day-cloudy-windy.png",
                WeatherIcons.DAY_FOG => "wi-day-fog.png",
                WeatherIcons.DAY_HAIL => "wi-day-hail.png",
                WeatherIcons.DAY_HAZE => "wi-day-haze.png",
                WeatherIcons.DAY_LIGHTNING => "wi-day-lightning.png",
                WeatherIcons.DAY_RAIN => "wi-day-rain.png",
                WeatherIcons.DAY_RAIN_MIX => "wi-day-rain-mix.png",
                WeatherIcons.DAY_RAIN_WIND => "wi-day-rain-wind.png",
                WeatherIcons.DAY_SHOWERS => "wi-day-showers.png",
                WeatherIcons.DAY_SLEET => "wi-day-sleet.png",
                WeatherIcons.DAY_SLEET_STORM => "wi-day-sleet-storm.png",
                WeatherIcons.DAY_SNOW => "wi-day-snow.png",
                WeatherIcons.DAY_SNOW_THUNDERSTORM => "wi-day-snow-thunderstorm.png",
                WeatherIcons.DAY_SNOW_WIND => "wi-day-snow-wind.png",
                WeatherIcons.DAY_SPRINKLE => "wi-day-sprinkle.png",
                WeatherIcons.DAY_STORM_SHOWERS => "wi-day-storm-showers.png",
                WeatherIcons.DAY_PARTLY_CLOUDY or
                WeatherIcons.DAY_SUNNY_OVERCAST => "wi-day-sunny-overcast.png",
                WeatherIcons.DAY_THUNDERSTORM => "wi-day-thunderstorm.png",
                WeatherIcons.DAY_WINDY => "wi-day-windy.png",
                WeatherIcons.DAY_HOT => "wi-hot.png",
                WeatherIcons.DAY_CLOUDY_HIGH => "wi-day-cloudy-high.png",
                WeatherIcons.DAY_LIGHT_WIND => "wi-day-light-wind.png",

                // Night
                WeatherIcons.NIGHT_CLEAR => "wi-night-clear.png",
                WeatherIcons.NIGHT_ALT_CLOUDY => "wi-night-alt-cloudy.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS => "wi-night-alt-cloudy-gusts.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY => "wi-night-alt-cloudy-windy.png",
                WeatherIcons.NIGHT_ALT_HAIL => "wi-night-alt-hail.png",
                WeatherIcons.NIGHT_ALT_LIGHTNING => "wi-night-alt-lightning.png",
                WeatherIcons.NIGHT_ALT_RAIN => "wi-night-alt-rain.png",
                WeatherIcons.NIGHT_ALT_RAIN_MIX => "wi-night-alt-rain-mix.png",
                WeatherIcons.NIGHT_ALT_RAIN_WIND => "wi-night-alt-rain-wind.png",
                WeatherIcons.NIGHT_ALT_SHOWERS => "wi-night-alt-showers.png",
                WeatherIcons.NIGHT_ALT_SLEET => "wi-night-alt-sleet.png",
                WeatherIcons.NIGHT_ALT_SLEET_STORM => "wi-night-alt-sleet-storm.png",
                WeatherIcons.NIGHT_ALT_SNOW => "wi-night-alt-snow.png",
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM => "wi-night-alt-snow-thunderstorm.png",
                WeatherIcons.NIGHT_ALT_SNOW_WIND => "wi-night-alt-snow-wind.png",
                WeatherIcons.NIGHT_ALT_SPRINKLE => "wi-night-alt-sprinkle.png",
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS => "wi-night-alt-storm-showers.png",
                WeatherIcons.NIGHT_ALT_THUNDERSTORM => "wi-night-alt-thunderstorm.png",
                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY or
                WeatherIcons.NIGHT_OVERCAST => "wi-night-alt-partly-cloudy.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "wi-night-alt-cloudy-high.png",
                WeatherIcons.NIGHT_FOG or
                WeatherIcons.NIGHT_HAZE => "wi-night-fog.png",
                WeatherIcons.NIGHT_WINDY => "wi-windy.png",
                WeatherIcons.NIGHT_HOT => "wi-thermometer-up.png",
                WeatherIcons.NIGHT_LIGHT_WIND => "wi-windy.png",

                // Neutral
                WeatherIcons.CLOUD => "wi-cloud.png",
                WeatherIcons.CLOUDY => "wi-cloudy.png",
                WeatherIcons.CLOUDY_GUSTS => "wi-cloudy-gusts.png",
                WeatherIcons.CLOUDY_WINDY => "wi-cloudy-windy.png",
                WeatherIcons.FOG => "wi-fog.png",
                WeatherIcons.HAIL => "wi-hail.png",
                WeatherIcons.HAZE => "wi-windy.png",
                WeatherIcons.HOT => "wi-thermometer-up.png",
                WeatherIcons.LIGHT_WIND => "wi-windy.png",
                WeatherIcons.OVERCAST => "wi-cloudy.png",
                WeatherIcons.RAIN => "wi-rain.png",
                WeatherIcons.RAIN_MIX => "wi-rain-mix.png",
                WeatherIcons.RAIN_WIND => "wi-rain-wind.png",
                WeatherIcons.SHOWERS => "wi-showers.png",
                WeatherIcons.SLEET => "wi-sleet.png",
                WeatherIcons.SLEET_STORM => "wi-sleet-storm.png",
                WeatherIcons.SNOW => "wi-snow.png",
                WeatherIcons.SNOW_THUNDERSTORM => "wi-snow-thunderstorm.png",
                WeatherIcons.SPRINKLE => "wi-sprinkle.png",
                WeatherIcons.STORM_SHOWERS => "wi-storm-showers.png",
                WeatherIcons.THUNDERSTORM => "wi-thunderstorm.png",
                WeatherIcons.SNOW_WIND => "wi-snow-wind.png",
                WeatherIcons.SMOG => "wi-smog.png",
                WeatherIcons.SMOKE => "wi-smoke.png",
                WeatherIcons.LIGHTNING => "wi-lightning.png",
                WeatherIcons.DUST => "wi-dust.png",
                WeatherIcons.SNOWFLAKE_COLD => "wi-snowflake-cold.png",
                WeatherIcons.WINDY => "wi-windy.png",
                WeatherIcons.STRONG_WIND => "wi-strong-wind.png",
                WeatherIcons.SANDSTORM => "wi-sandstorm.png",
                WeatherIcons.HURRICANE => "wi-hurricane.png",
                WeatherIcons.TORNADO => "wi-tornado.png",
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

                WeatherIcons.FAHRENHEIT => "wi-fahrenheit.png",
                WeatherIcons.CELSIUS => "wi-celsius.png",

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
                WeatherIcons.UV_INDEX_11 => "wi-day-sunny.png",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.png",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.png",
                WeatherIcons.RAGWEED_POLLEN => "ic_ragweed_pollen.png",

                WeatherIcons.NA => "wi-na.png",

                _ => string.Empty,
            };

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

        public override String GetSVGIconUri(string icon, bool isLight = false)
        {
            string baseuri = WeatherIconsManager.GetSVGBaseUri(isLight);

            string fileIcon = icon switch
            {
                // Day
                WeatherIcons.DAY_SUNNY => "wi-day-sunny.svg",
                WeatherIcons.DAY_CLOUDY => "wi-day-cloudy.svg",
                WeatherIcons.DAY_CLOUDY_GUSTS => "wi-day-cloudy-gusts.svg",
                WeatherIcons.DAY_CLOUDY_WINDY => "wi-day-cloudy-windy.svg",
                WeatherIcons.DAY_FOG => "wi-day-fog.svg",
                WeatherIcons.DAY_HAIL => "wi-day-hail.svg",
                WeatherIcons.DAY_HAZE => "wi-day-haze.svg",
                WeatherIcons.DAY_LIGHTNING => "wi-day-lightning.svg",
                WeatherIcons.DAY_RAIN => "wi-day-rain.svg",
                WeatherIcons.DAY_RAIN_MIX => "wi-day-rain-mix.svg",
                WeatherIcons.DAY_RAIN_WIND => "wi-day-rain-wind.svg",
                WeatherIcons.DAY_SHOWERS => "wi-day-showers.svg",
                WeatherIcons.DAY_SLEET => "wi-day-sleet.svg",
                WeatherIcons.DAY_SLEET_STORM => "wi-day-sleet-storm.svg",
                WeatherIcons.DAY_SNOW => "wi-day-snow.svg",
                WeatherIcons.DAY_SNOW_THUNDERSTORM => "wi-day-snow-thunderstorm.svg",
                WeatherIcons.DAY_SNOW_WIND => "wi-day-snow-wind.svg",
                WeatherIcons.DAY_SPRINKLE => "wi-day-sprinkle.svg",
                WeatherIcons.DAY_STORM_SHOWERS => "wi-day-storm-showers.svg",
                WeatherIcons.DAY_PARTLY_CLOUDY or
                WeatherIcons.DAY_SUNNY_OVERCAST => "wi-day-sunny-overcast.svg",
                WeatherIcons.DAY_THUNDERSTORM => "wi-day-thunderstorm.svg",
                WeatherIcons.DAY_WINDY => "wi-day-windy.svg",
                WeatherIcons.DAY_HOT => "wi-hot.svg",
                WeatherIcons.DAY_CLOUDY_HIGH => "wi-day-cloudy-high.svg",
                WeatherIcons.DAY_LIGHT_WIND => "wi-day-light-wind.svg",

                // Night
                WeatherIcons.NIGHT_CLEAR => "wi-night-clear.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY => "wi-night-alt-cloudy.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS => "wi-night-alt-cloudy-gusts.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY => "wi-night-alt-cloudy-windy.svg",
                WeatherIcons.NIGHT_ALT_HAIL => "wi-night-alt-hail.svg",
                WeatherIcons.NIGHT_ALT_LIGHTNING => "wi-night-alt-lightning.svg",
                WeatherIcons.NIGHT_ALT_RAIN => "wi-night-alt-rain.svg",
                WeatherIcons.NIGHT_ALT_RAIN_MIX => "wi-night-alt-rain-mix.svg",
                WeatherIcons.NIGHT_ALT_RAIN_WIND => "wi-night-alt-rain-wind.svg",
                WeatherIcons.NIGHT_ALT_SHOWERS => "wi-night-alt-showers.svg",
                WeatherIcons.NIGHT_ALT_SLEET => "wi-night-alt-sleet.svg",
                WeatherIcons.NIGHT_ALT_SLEET_STORM => "wi-night-alt-sleet-storm.svg",
                WeatherIcons.NIGHT_ALT_SNOW => "wi-night-alt-snow.svg",
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM => "wi-night-alt-snow-thunderstorm.svg",
                WeatherIcons.NIGHT_ALT_SNOW_WIND => "wi-night-alt-snow-wind.svg",
                WeatherIcons.NIGHT_ALT_SPRINKLE => "wi-night-alt-sprinkle.svg",
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS => "wi-night-alt-storm-showers.svg",
                WeatherIcons.NIGHT_ALT_THUNDERSTORM => "wi-night-alt-thunderstorm.svg",
                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY or
                WeatherIcons.NIGHT_OVERCAST => "wi-night-alt-partly-cloudy.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "wi-night-alt-cloudy-high.svg",
                WeatherIcons.NIGHT_FOG or
                WeatherIcons.NIGHT_HAZE => "wi-night-fog.svg",
                WeatherIcons.NIGHT_WINDY => "wi-windy.svg",
                WeatherIcons.NIGHT_HOT => "wi-thermometer-up.svg",
                WeatherIcons.NIGHT_LIGHT_WIND => "wi-windy.svg",

                // Neutral
                WeatherIcons.CLOUD => "wi-cloud.svg",
                WeatherIcons.CLOUDY => "wi-cloudy.svg",
                WeatherIcons.CLOUDY_GUSTS => "wi-cloudy-gusts.svg",
                WeatherIcons.CLOUDY_WINDY => "wi-cloudy-windy.svg",
                WeatherIcons.FOG => "wi-fog.svg",
                WeatherIcons.HAIL => "wi-hail.svg",
                WeatherIcons.HAZE => "wi-windy.svg",
                WeatherIcons.HOT => "wi-thermometer-up.svg",
                WeatherIcons.LIGHT_WIND => "wi-windy.svg",
                WeatherIcons.OVERCAST => "wi-cloudy.svg",
                WeatherIcons.RAIN => "wi-rain.svg",
                WeatherIcons.RAIN_MIX => "wi-rain-mix.svg",
                WeatherIcons.RAIN_WIND => "wi-rain-wind.svg",
                WeatherIcons.SHOWERS => "wi-showers.svg",
                WeatherIcons.SLEET => "wi-sleet.svg",
                WeatherIcons.SLEET_STORM => "wi-sleet-storm.svg",
                WeatherIcons.SNOW => "wi-snow.svg",
                WeatherIcons.SNOW_THUNDERSTORM => "wi-snow-thunderstorm.svg",
                WeatherIcons.SPRINKLE => "wi-sprinkle.svg",
                WeatherIcons.STORM_SHOWERS => "wi-storm-showers.svg",
                WeatherIcons.THUNDERSTORM => "wi-thunderstorm.svg",
                WeatherIcons.SNOW_WIND => "wi-snow-wind.svg",
                WeatherIcons.SMOG => "wi-smog.svg",
                WeatherIcons.SMOKE => "wi-smoke.svg",
                WeatherIcons.LIGHTNING => "wi-lightning.svg",
                WeatherIcons.DUST => "wi-dust.svg",
                WeatherIcons.SNOWFLAKE_COLD => "wi-snowflake-cold.svg",
                WeatherIcons.WINDY => "wi-windy.svg",
                WeatherIcons.STRONG_WIND => "wi-strong-wind.svg",
                WeatherIcons.SANDSTORM => "wi-sandstorm.svg",
                WeatherIcons.HURRICANE => "wi-hurricane.svg",
                WeatherIcons.TORNADO => "wi-tornado.svg",
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

                WeatherIcons.FAHRENHEIT => "wi-fahrenheit.svg",
                WeatherIcons.CELSIUS => "wi-celsius.svg",

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
                WeatherIcons.UV_INDEX_11 => "wi-day-sunny.svg",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.svg",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.svg",
                WeatherIcons.RAGWEED_POLLEN => "ic_ragweed_pollen.svg",

                WeatherIcons.NA => "wi-na.svg",

                _ => string.Empty,
            };

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "wi-na.svg";
            }

            return baseuri + fileIcon;
        }
    }
}
