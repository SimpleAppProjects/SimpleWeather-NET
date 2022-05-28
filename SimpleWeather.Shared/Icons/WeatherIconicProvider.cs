using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                WeatherIcons.DAY_SUNNY => "wic-sun.png",
                WeatherIcons.DAY_CLOUDY => "wic-sun-cloud.png",
                WeatherIcons.DAY_CLOUDY_GUSTS => "wic-sun-cloud-wind.png",
                WeatherIcons.DAY_CLOUDY_WINDY => "wic-sun-cloud-wind.png",
                WeatherIcons.DAY_FOG => "wic-sun-fog.png",
                WeatherIcons.DAY_HAIL => "wic-hail.png",
                WeatherIcons.DAY_HAZE => "wic-sun-fog.png",
                WeatherIcons.DAY_LIGHTNING => "wic-sun-cloud-lightning.png",
                WeatherIcons.DAY_PARTLY_CLOUDY => "wic-sun-cloud.png",
                WeatherIcons.DAY_RAIN => "wic-sun-cloud-rain.png",
                WeatherIcons.DAY_RAIN_MIX => "wic-sun-cloud-rain.png",
                WeatherIcons.DAY_RAIN_WIND => "wic-sun-cloud-rain.png",
                WeatherIcons.DAY_SHOWERS => "wic-sun-cloud-rain.png",
                WeatherIcons.DAY_SLEET => "wic-sun-cloud-rain.png",
                WeatherIcons.DAY_SLEET_STORM => "wic-sun-cloud-rain.png",
                WeatherIcons.DAY_SNOW => "wic-sun-cloud-snow.png",
                WeatherIcons.DAY_SNOW_THUNDERSTORM => "wic-sun-cloud-snow.png",
                WeatherIcons.DAY_SNOW_WIND => "wic-sun-cloud-snow.png",
                WeatherIcons.DAY_SPRINKLE => "wic-sun-cloud-rain.png",
                WeatherIcons.DAY_STORM_SHOWERS => "wic-sun-cloud-lightning.png",
                WeatherIcons.DAY_SUNNY_OVERCAST => "wic-sun-cloud.png",
                WeatherIcons.DAY_THUNDERSTORM => "wic-sun-cloud-lightning.png",
                WeatherIcons.DAY_WINDY => "wic-wind.png",
                WeatherIcons.DAY_HOT => "wic-sun.png",
                WeatherIcons.DAY_CLOUDY_HIGH => "wic-sun-cloud.png",
                WeatherIcons.DAY_LIGHT_WIND => "wic-wind.png",

                // Night
                WeatherIcons.NIGHT_CLEAR => "wic-moon.png",
                WeatherIcons.NIGHT_ALT_CLOUDY => "wic-moon-cloud.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS => "wic-moon-cloud-wind.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY => "wic-moon-cloud-wind.png",
                WeatherIcons.NIGHT_ALT_HAIL => "wic-hail.png",
                WeatherIcons.NIGHT_ALT_LIGHTNING => "wic-moon-cloud-lightning.png",
                WeatherIcons.NIGHT_ALT_RAIN => "wic-moon-cloud-rain.png",
                WeatherIcons.NIGHT_ALT_RAIN_MIX => "wic-moon-cloud-rain.png",
                WeatherIcons.NIGHT_ALT_RAIN_WIND => "wic-moon-cloud-rain.png",
                WeatherIcons.NIGHT_ALT_SHOWERS => "wic-moon-cloud-rain.png",
                WeatherIcons.NIGHT_ALT_SLEET => "wic-moon-cloud-rain.png",
                WeatherIcons.NIGHT_ALT_SLEET_STORM => "wic-moon-cloud-rain.png",
                WeatherIcons.NIGHT_ALT_SNOW => "wic-moon-cloud-snow.png",
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM => "wic-moon-cloud-snow.png",
                WeatherIcons.NIGHT_ALT_SNOW_WIND => "wic-moon-cloud-snow.png",
                WeatherIcons.NIGHT_ALT_SPRINKLE => "wic-moon-cloud-rain.png",
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS => "wic-moon-cloud-lightning.png",
                WeatherIcons.NIGHT_ALT_THUNDERSTORM => "wic-moon-cloud-lightning.png",
                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY => "wic-moon-cloud.png",
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "wic-moon-cloud.png",
                WeatherIcons.NIGHT_FOG => "wic-moon-fog.png",
                WeatherIcons.NIGHT_OVERCAST => "wic-moon-cloud.png",
                WeatherIcons.NIGHT_HAZE => "wic-moon-fog.png",
                WeatherIcons.NIGHT_WINDY => "wic-wind.png",
                WeatherIcons.NIGHT_HOT => "wic-moon.png",
                WeatherIcons.NIGHT_LIGHT_WIND => "wic-wind.png",

                // Neutral
                WeatherIcons.CLOUD => "wic-cloud.png",
                WeatherIcons.CLOUDY => "wic-clouds.png",
                WeatherIcons.CLOUDY_GUSTS => "wic-cloud-wind.png",
                WeatherIcons.CLOUDY_WINDY => "wic-cloud-wind.png",
                WeatherIcons.FOG => "wic-fog.png",
                WeatherIcons.HAIL => "wic-hail.png",
                WeatherIcons.HAZE => "wic-fog.png",
                WeatherIcons.HOT => "wic-thermometer-hot.png",
                WeatherIcons.LIGHT_WIND => "wic-wind.png",
                WeatherIcons.RAIN => "wic-cloud-rain.png",
                WeatherIcons.RAIN_MIX => "wic-cloud-rain.png",
                WeatherIcons.RAIN_WIND => "wic-cloud-rain.png",
                WeatherIcons.OVERCAST => "wic-clouds.png",
                WeatherIcons.SHOWERS => "wic-cloud-rain.png",
                WeatherIcons.SLEET => "wic-cloud-rain.png",
                WeatherIcons.SLEET_STORM => "wic-cloud-rain.png",
                WeatherIcons.SNOW => "wic-cloud-snow.png",
                WeatherIcons.SNOW_THUNDERSTORM => "wic-cloud-snow.png",
                WeatherIcons.SPRINKLE => "wic-cloud-rain-single.png",
                WeatherIcons.STORM_SHOWERS => "wic-lightning.png",
                WeatherIcons.THUNDERSTORM => "wic-lightning.png",
                WeatherIcons.SNOW_WIND => "wic-cloud-snow.png",
                WeatherIcons.SMOG => "wi-smog.png",
                WeatherIcons.SMOKE => "wi-smoke.png",
                WeatherIcons.LIGHTNING => "wic-lightning.png",
                WeatherIcons.DUST => "wi-dust.png",
                WeatherIcons.SNOWFLAKE_COLD => "wic-snowflake.png",
                WeatherIcons.WINDY => "wic-wind.png",
                WeatherIcons.STRONG_WIND => "wic-wind-high.png",
                WeatherIcons.SANDSTORM => "wi-sandstorm.png",
                WeatherIcons.HURRICANE => "wi-hurricane.png",
                WeatherIcons.TORNADO => "wic-tornado.png",
                WeatherIcons.FIRE => "wi-fire.png",
                WeatherIcons.FLOOD => "wi-flood.png",
                WeatherIcons.VOLCANO => "wi-volcano.png",
                WeatherIcons.BAROMETER => "wic-barometer.png",
                WeatherIcons.HUMIDITY => "wic-raindrop.png",
                WeatherIcons.MOONRISE => "wi-moonrise.png",
                WeatherIcons.MOONSET => "wi-moonset.png",
                WeatherIcons.RAINDROP => "wi-raindrop.png",
                WeatherIcons.RAINDROPS => "wi-raindrops.png",
                WeatherIcons.SUNRISE => "wic-sunrise.png",
                WeatherIcons.SUNSET => "wic-sunset.png",
                WeatherIcons.THERMOMETER => "wic-thermometer-medium.png",
                WeatherIcons.UMBRELLA => "wic-umbrella.png",
                WeatherIcons.WIND_DIRECTION => "wic-compass.png",
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
                WeatherIcons.MOON_NEW => "wic-moon-fullmoon.png",
                WeatherIcons.MOON_WAXING_CRESCENT_3 => "wic-moon-waxing-crescent.png",
                WeatherIcons.MOON_FIRST_QUARTER => "wic-moon-first-quarter.png",
                WeatherIcons.MOON_WAXING_GIBBOUS_3 => "wic-moon-waxing-gibbous.png",
                WeatherIcons.MOON_FULL => "wic-moon-newmoon.png",
                WeatherIcons.MOON_WANING_GIBBOUS_3 => "wic-moon-waning-gibbous.png",
                WeatherIcons.MOON_THIRD_QUARTER => "wic-moon-last-quarter.png",
                WeatherIcons.MOON_WANING_CRESCENT_3 => "wic-moon-waning-crescent.png",

                WeatherIcons.MOON_ALT_NEW => "wic-moon-fullmoon.png",
                WeatherIcons.MOON_ALT_WAXING_CRESCENT_3 => "wic-moon-waxing-crescent.png",
                WeatherIcons.MOON_ALT_FIRST_QUARTER => "wic-moon-first-quarter.png",
                WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3 => "wic-moon-waxing-gibbous.png",
                WeatherIcons.MOON_ALT_FULL => "wic-moon-newmoon.png",
                WeatherIcons.MOON_ALT_WANING_GIBBOUS_3 => "wic-moon-waning-gibbous.png",
                WeatherIcons.MOON_ALT_THIRD_QUARTER => "wic-moon-last-quarter.png",
                WeatherIcons.MOON_ALT_WANING_CRESCENT_3 => "wic-moon-waning-crescent.png",

                WeatherIcons.FAHRENHEIT => "wic-fahrenheit.png",
                WeatherIcons.CELSIUS => "wic-celsius.png",

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
                WeatherIcons.UV_INDEX_11 => "wic-sun.png",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.png",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.png",
                WeatherIcons.RAGWEED_POLLEN => "ic_ragweed_pollen.png",

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
                WeatherIcons.DAY_SUNNY => "wic-sun.svg",
                WeatherIcons.DAY_CLOUDY => "wic-sun-cloud.svg",
                WeatherIcons.DAY_CLOUDY_GUSTS => "wic-sun-cloud-wind.svg",
                WeatherIcons.DAY_CLOUDY_WINDY => "wic-sun-cloud-wind.svg",
                WeatherIcons.DAY_FOG => "wic-sun-fog.svg",
                WeatherIcons.DAY_HAIL => "wic-hail.svg",
                WeatherIcons.DAY_HAZE => "wic-sun-fog.svg",
                WeatherIcons.DAY_LIGHTNING => "wic-sun-cloud-lightning.svg",
                WeatherIcons.DAY_PARTLY_CLOUDY => "wic-sun-cloud.svg",
                WeatherIcons.DAY_RAIN => "wic-sun-cloud-rain.svg",
                WeatherIcons.DAY_RAIN_MIX => "wic-sun-cloud-rain.svg",
                WeatherIcons.DAY_RAIN_WIND => "wic-sun-cloud-rain.svg",
                WeatherIcons.DAY_SHOWERS => "wic-sun-cloud-rain.svg",
                WeatherIcons.DAY_SLEET => "wic-sun-cloud-rain.svg",
                WeatherIcons.DAY_SLEET_STORM => "wic-sun-cloud-rain.svg",
                WeatherIcons.DAY_SNOW => "wic-sun-cloud-snow.svg",
                WeatherIcons.DAY_SNOW_THUNDERSTORM => "wic-sun-cloud-snow.svg",
                WeatherIcons.DAY_SNOW_WIND => "wic-sun-cloud-snow.svg",
                WeatherIcons.DAY_SPRINKLE => "wic-sun-cloud-rain.svg",
                WeatherIcons.DAY_STORM_SHOWERS => "wic-sun-cloud-lightning.svg",
                WeatherIcons.DAY_SUNNY_OVERCAST => "wic-sun-cloud.svg",
                WeatherIcons.DAY_THUNDERSTORM => "wic-sun-cloud-lightning.svg",
                WeatherIcons.DAY_WINDY => "wic-wind.svg",
                WeatherIcons.DAY_HOT => "wic-sun.svg",
                WeatherIcons.DAY_CLOUDY_HIGH => "wic-sun-cloud.svg",
                WeatherIcons.DAY_LIGHT_WIND => "wic-wind.svg",

                // Night
                WeatherIcons.NIGHT_CLEAR => "wic-moon.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY => "wic-moon-cloud.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS => "wic-moon-cloud-wind.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY => "wic-moon-cloud-wind.svg",
                WeatherIcons.NIGHT_ALT_HAIL => "wic-hail.svg",
                WeatherIcons.NIGHT_ALT_LIGHTNING => "wic-moon-cloud-lightning.svg",
                WeatherIcons.NIGHT_ALT_RAIN => "wic-moon-cloud-rain.svg",
                WeatherIcons.NIGHT_ALT_RAIN_MIX => "wic-moon-cloud-rain.svg",
                WeatherIcons.NIGHT_ALT_RAIN_WIND => "wic-moon-cloud-rain.svg",
                WeatherIcons.NIGHT_ALT_SHOWERS => "wic-moon-cloud-rain.svg",
                WeatherIcons.NIGHT_ALT_SLEET => "wic-moon-cloud-rain.svg",
                WeatherIcons.NIGHT_ALT_SLEET_STORM => "wic-moon-cloud-rain.svg",
                WeatherIcons.NIGHT_ALT_SNOW => "wic-moon-cloud-snow.svg",
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM => "wic-moon-cloud-snow.svg",
                WeatherIcons.NIGHT_ALT_SNOW_WIND => "wic-moon-cloud-snow.svg",
                WeatherIcons.NIGHT_ALT_SPRINKLE => "wic-moon-cloud-rain.svg",
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS => "wic-moon-cloud-lightning.svg",
                WeatherIcons.NIGHT_ALT_THUNDERSTORM => "wic-moon-cloud-lightning.svg",
                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY => "wic-moon-cloud.svg",
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH => "wic-moon-cloud.svg",
                WeatherIcons.NIGHT_FOG => "wic-moon-fog.svg",
                WeatherIcons.NIGHT_OVERCAST => "wic-moon-cloud.svg",
                WeatherIcons.NIGHT_HAZE => "wic-moon-fog.svg",
                WeatherIcons.NIGHT_WINDY => "wic-wind.svg",
                WeatherIcons.NIGHT_HOT => "wic-moon.svg",
                WeatherIcons.NIGHT_LIGHT_WIND => "wic-wind.svg",

                // Neutral
                WeatherIcons.CLOUD => "wic-cloud.svg",
                WeatherIcons.CLOUDY => "wic-clouds.svg",
                WeatherIcons.CLOUDY_GUSTS => "wic-cloud-wind.svg",
                WeatherIcons.CLOUDY_WINDY => "wic-cloud-wind.svg",
                WeatherIcons.FOG => "wic-fog.svg",
                WeatherIcons.HAIL => "wic-hail.svg",
                WeatherIcons.HAZE => "wic-fog.svg",
                WeatherIcons.HOT => "wic-thermometer-hot.svg",
                WeatherIcons.LIGHT_WIND => "wic-wind.svg",
                WeatherIcons.RAIN => "wic-cloud-rain.svg",
                WeatherIcons.RAIN_MIX => "wic-cloud-rain.svg",
                WeatherIcons.RAIN_WIND => "wic-cloud-rain.svg",
                WeatherIcons.OVERCAST => "wic-clouds.svg",
                WeatherIcons.SHOWERS => "wic-cloud-rain.svg",
                WeatherIcons.SLEET => "wic-cloud-rain.svg",
                WeatherIcons.SLEET_STORM => "wic-cloud-rain.svg",
                WeatherIcons.SNOW => "wic-cloud-snow.svg",
                WeatherIcons.SNOW_THUNDERSTORM => "wic-cloud-snow.svg",
                WeatherIcons.SPRINKLE => "wic-cloud-rain-single.svg",
                WeatherIcons.STORM_SHOWERS => "wic-lightning.svg",
                WeatherIcons.THUNDERSTORM => "wic-lightning.svg",
                WeatherIcons.SNOW_WIND => "wic-cloud-snow.svg",
                WeatherIcons.SMOG => "wi-smog.svg",
                WeatherIcons.SMOKE => "wi-smoke.svg",
                WeatherIcons.LIGHTNING => "wic-lightning.svg",
                WeatherIcons.DUST => "wi-dust.svg",
                WeatherIcons.SNOWFLAKE_COLD => "wic-snowflake.svg",
                WeatherIcons.WINDY => "wic-wind.svg",
                WeatherIcons.STRONG_WIND => "wic-wind-high.svg",
                WeatherIcons.SANDSTORM => "wi-sandstorm.svg",
                WeatherIcons.HURRICANE => "wi-hurricane.svg",
                WeatherIcons.TORNADO => "wic-tornado.svg",
                WeatherIcons.FIRE => "wi-fire.svg",
                WeatherIcons.FLOOD => "wi-flood.svg",
                WeatherIcons.VOLCANO => "wi-volcano.svg",
                WeatherIcons.BAROMETER => "wic-barometer.svg",
                WeatherIcons.HUMIDITY => "wic-raindrop.svg",
                WeatherIcons.MOONRISE => "wi-moonrise.svg",
                WeatherIcons.MOONSET => "wi-moonset.svg",
                WeatherIcons.RAINDROP => "wi-raindrop.svg",
                WeatherIcons.RAINDROPS => "wi-raindrops.svg",
                WeatherIcons.SUNRISE => "wic-sunrise.svg",
                WeatherIcons.SUNSET => "wic-sunset.svg",
                WeatherIcons.THERMOMETER => "wic-thermometer-medium.svg",
                WeatherIcons.UMBRELLA => "wic-umbrella.svg",
                WeatherIcons.WIND_DIRECTION => "wic-compass.svg",
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
                WeatherIcons.MOON_NEW => "wic-moon-fullmoon.svg",
                WeatherIcons.MOON_WAXING_CRESCENT_3 => "wic-moon-waxing-crescent.svg",
                WeatherIcons.MOON_FIRST_QUARTER => "wic-moon-first-quarter.svg",
                WeatherIcons.MOON_WAXING_GIBBOUS_3 => "wic-moon-waxing-gibbous.svg",
                WeatherIcons.MOON_FULL => "wic-moon-newmoon.svg",
                WeatherIcons.MOON_WANING_GIBBOUS_3 => "wic-moon-waning-gibbous.svg",
                WeatherIcons.MOON_THIRD_QUARTER => "wic-moon-last-quarter.svg",
                WeatherIcons.MOON_WANING_CRESCENT_3 => "wic-moon-waning-crescent.svg",

                WeatherIcons.MOON_ALT_NEW => "wic-moon-fullmoon.svg",
                WeatherIcons.MOON_ALT_WAXING_CRESCENT_3 => "wic-moon-waxing-crescent.svg",
                WeatherIcons.MOON_ALT_FIRST_QUARTER => "wic-moon-first-quarter.svg",
                WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3 => "wic-moon-waxing-gibbous.svg",
                WeatherIcons.MOON_ALT_FULL => "wic-moon-newmoon.svg",
                WeatherIcons.MOON_ALT_WANING_GIBBOUS_3 => "wic-moon-waning-gibbous.svg",
                WeatherIcons.MOON_ALT_THIRD_QUARTER => "wic-moon-last-quarter.svg",
                WeatherIcons.MOON_ALT_WANING_CRESCENT_3 => "wic-moon-waning-crescent.svg",

                WeatherIcons.FAHRENHEIT => "wic-fahrenheit.svg",
                WeatherIcons.CELSIUS => "wic-celsius.svg",

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
                WeatherIcons.UV_INDEX_11 => "wic-sun.svg",

                WeatherIcons.TREE_POLLEN => "ic_outline_tree.svg",
                WeatherIcons.GRASS_POLLEN => "ic_baseline_grass.svg",
                WeatherIcons.RAGWEED_POLLEN => "ic_ragweed_pollen.svg",

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
