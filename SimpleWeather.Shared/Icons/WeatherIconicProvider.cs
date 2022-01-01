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
            string fileIcon = string.Empty;

            switch (icon)
            {
                // Day
                case WeatherIcons.DAY_SUNNY:
                case WeatherIcons.DAY_HOT:
                    fileIcon = "wic-sun.png";
                    break;

                case WeatherIcons.DAY_CLOUDY:
                case WeatherIcons.DAY_SUNNY_OVERCAST:
                case WeatherIcons.DAY_CLOUDY_HIGH:
                    fileIcon = "wic-sun-cloud.png";
                    break;

                case WeatherIcons.DAY_CLOUDY_GUSTS:
                case WeatherIcons.DAY_CLOUDY_WINDY:
                    fileIcon = "wic-sun-cloud-wind.png";
                    break;

                case WeatherIcons.DAY_FOG:
                    fileIcon = "wic-sun-fog.png";
                    break;

                case WeatherIcons.DAY_HAIL:
                    fileIcon = "wic-hail.png";
                    break;

                case WeatherIcons.DAY_HAZE:
                    fileIcon = "wic-sun-fog.png";
                    break;

                case WeatherIcons.DAY_LIGHTNING:
                    fileIcon = "wic-sun-cloud-lightning.png";
                    break;

                case WeatherIcons.DAY_PARTLY_CLOUDY:
                    fileIcon = "wic-sun-cloud.png";
                    break;

                case WeatherIcons.DAY_RAIN:
                case WeatherIcons.DAY_RAIN_MIX:
                case WeatherIcons.DAY_RAIN_WIND:
                case WeatherIcons.DAY_SHOWERS:
                case WeatherIcons.DAY_SLEET:
                case WeatherIcons.DAY_SLEET_STORM:
                case WeatherIcons.DAY_SPRINKLE:
                    fileIcon = "wic-sun-cloud-rain.png";
                    break;

                case WeatherIcons.DAY_SNOW:
                case WeatherIcons.DAY_SNOW_THUNDERSTORM:
                case WeatherIcons.DAY_SNOW_WIND:
                    fileIcon = "wic-sun-cloud-snow.png";
                    break;

                case WeatherIcons.DAY_STORM_SHOWERS:
                case WeatherIcons.DAY_THUNDERSTORM:
                    fileIcon = "wic-sun-cloud-lightning.png";
                    break;

                case WeatherIcons.DAY_WINDY:
                case WeatherIcons.DAY_LIGHT_WIND:
                    fileIcon = "wic-sun-cloud-wind.png";
                    break;

                // Night
                case WeatherIcons.NIGHT_CLEAR:
                    fileIcon = "wic-moon.png";
                    break;

                case WeatherIcons.NIGHT_ALT_CLOUDY:
                case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY_HIGH:
                    fileIcon = "wic-moon-cloud.png";
                    break;

                case WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS:
                case WeatherIcons.NIGHT_ALT_CLOUDY_WINDY:
                    fileIcon = "wic-moon-cloud-wind.png";
                    break;

                case WeatherIcons.NIGHT_ALT_HAIL:
                    fileIcon = "wic-hail.png";
                    break;

                case WeatherIcons.NIGHT_ALT_LIGHTNING:
                    fileIcon = "wic-moon-cloud-lightning.png";
                    break;

                case WeatherIcons.NIGHT_ALT_RAIN:
                case WeatherIcons.NIGHT_ALT_RAIN_MIX:
                case WeatherIcons.NIGHT_ALT_RAIN_WIND:
                case WeatherIcons.NIGHT_ALT_SHOWERS:
                case WeatherIcons.NIGHT_ALT_SLEET:
                case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                case WeatherIcons.NIGHT_ALT_SPRINKLE:
                    fileIcon = "wic-moon-cloud-rain.png";
                    break;

                case WeatherIcons.NIGHT_ALT_SNOW:
                case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                    fileIcon = "wic-moon-cloud-snow.png";
                    break;

                case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                    fileIcon = "wic-moon-cloud-lightning.png";
                    break;

                case WeatherIcons.NIGHT_FOG:
                    fileIcon = "wic-moon-fog.png";
                    break;

                // Neutral
                case WeatherIcons.CLOUD:
                    fileIcon = "wic-cloud.png";
                    break;

                case WeatherIcons.CLOUDY:
                    fileIcon = "wic-clouds.png";
                    break;

                case WeatherIcons.CLOUDY_GUSTS:
                case WeatherIcons.CLOUDY_WINDY:
                    fileIcon = "wic-cloud-wind.png";
                    break;

                case WeatherIcons.FOG:
                    fileIcon = "wic-fog.png";
                    break;

                case WeatherIcons.HAIL:
                    fileIcon = "wic-hail.png";
                    break;

                case WeatherIcons.RAIN:
                case WeatherIcons.RAIN_MIX:
                case WeatherIcons.RAIN_WIND:
                case WeatherIcons.SHOWERS:
                case WeatherIcons.SLEET:
                    fileIcon = "wic-cloud-rain.png";
                    break;

                case WeatherIcons.SNOW:
                case WeatherIcons.SNOW_WIND:
                    fileIcon = "wic-cloud-snow.png";
                    break;

                case WeatherIcons.SPRINKLE:
                    fileIcon = "wic-cloud-rain-single.png";
                    break;

                case WeatherIcons.STORM_SHOWERS:
                case WeatherIcons.THUNDERSTORM:
                case WeatherIcons.LIGHTNING:
                    fileIcon = "wic-lightning.png";
                    break;

                case WeatherIcons.SMOG:
                    fileIcon = "wi-smog.png";
                    break;

                case WeatherIcons.SMOKE:
                    fileIcon = "wi-smoke.png";
                    break;

                case WeatherIcons.DUST:
                    fileIcon = "wi-dust.png";
                    break;

                case WeatherIcons.SNOWFLAKE_COLD:
                    fileIcon = "wic-snowflake.png";
                    break;

                case WeatherIcons.WINDY:
                    fileIcon = "wic-wind.png";
                    break;

                case WeatherIcons.STRONG_WIND:
                    fileIcon = "wic-wind-high.png";
                    break;

                case WeatherIcons.SANDSTORM:
                    fileIcon = "wi-sandstorm.png";
                    break;

                case WeatherIcons.HURRICANE:
                    fileIcon = "wi-hurricane.png";
                    break;

                case WeatherIcons.TORNADO:
                    fileIcon = "wic-tornado.png";
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
                    fileIcon = "wic-barometer.png";
                    break;

                case WeatherIcons.HUMIDITY:
                    fileIcon = "wic-raindrop.png";
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
                    fileIcon = "wic-sunrise.png";
                    break;

                case WeatherIcons.SUNSET:
                    fileIcon = "wic-sunset.png";
                    break;

                case WeatherIcons.THERMOMETER:
                    fileIcon = "wic-thermometer-medium.png";
                    break;

                case WeatherIcons.UMBRELLA:
                    fileIcon = "wic-umbrella.png";
                    break;

                case WeatherIcons.WIND_DIRECTION:
                    fileIcon = "wic-compass.png";
                    break;

                case WeatherIcons.DIRECTION_UP:
                    fileIcon = "wi-direction-up.png";
                    break;

                case WeatherIcons.DIRECTION_DOWN:
                    fileIcon = "wi-direction-down.png";
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
                case WeatherIcons.MOON_ALT_NEW:
                    fileIcon = "wic-moon-newmoon.png";
                    break;

                case WeatherIcons.MOON_WAXING_CRESCENT_3:
                case WeatherIcons.MOON_ALT_WAXING_CRESCENT_3:
                    fileIcon = "wic-moon-waxing-crescent.png";
                    break;

                case WeatherIcons.MOON_FIRST_QUARTER:
                case WeatherIcons.MOON_ALT_FIRST_QUARTER:
                    fileIcon = "wic-moon-first-quarter.png";
                    break;

                case WeatherIcons.MOON_WAXING_GIBBOUS_3:
                case WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3:
                    fileIcon = "wic-moon-waxing-gibbous.png";
                    break;

                case WeatherIcons.MOON_FULL:
                case WeatherIcons.MOON_ALT_FULL:
                    fileIcon = "wic-moon-fullmoon.png";
                    break;

                case WeatherIcons.MOON_WANING_GIBBOUS_3:
                case WeatherIcons.MOON_ALT_WANING_GIBBOUS_3:
                    fileIcon = "wic-moon-waning-gibbous.png";
                    break;

                case WeatherIcons.MOON_THIRD_QUARTER:
                case WeatherIcons.MOON_ALT_THIRD_QUARTER:
                    fileIcon = "wic-moon-last-quarter.png";
                    break;

                case WeatherIcons.MOON_WANING_CRESCENT_3:
                case WeatherIcons.MOON_ALT_WANING_CRESCENT_3:
                    fileIcon = "wic-moon-waning-crescent.png";
                    break;

                case WeatherIcons.FAHRENHEIT:
                    fileIcon = "wic-fahrenheit.png";
                    break;

                case WeatherIcons.CELSIUS:
                    fileIcon = "wic-celsius.png";
                    break;

                case WeatherIcons.UV_INDEX:
                case WeatherIcons.UV_INDEX_1:
                case WeatherIcons.UV_INDEX_2:
                case WeatherIcons.UV_INDEX_3:
                case WeatherIcons.UV_INDEX_4:
                case WeatherIcons.UV_INDEX_5:
                case WeatherIcons.UV_INDEX_6:
                case WeatherIcons.UV_INDEX_7:
                case WeatherIcons.UV_INDEX_8:
                case WeatherIcons.UV_INDEX_9:
                case WeatherIcons.UV_INDEX_10:
                case WeatherIcons.UV_INDEX_11:
                    fileIcon = "wic-sun.png";
                    break;

                case WeatherIcons.NA:
                    fileIcon = "wui-unknown.png";
                    break;
            }

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
            string fileIcon = string.Empty;

            switch (icon)
            {
                // Day
                case WeatherIcons.DAY_SUNNY:
                case WeatherIcons.DAY_HOT:
                    fileIcon = "wic-sun.svg";
                    break;

                case WeatherIcons.DAY_CLOUDY:
                case WeatherIcons.DAY_SUNNY_OVERCAST:
                case WeatherIcons.DAY_CLOUDY_HIGH:
                    fileIcon = "wic-sun-cloud.svg";
                    break;

                case WeatherIcons.DAY_CLOUDY_GUSTS:
                case WeatherIcons.DAY_CLOUDY_WINDY:
                    fileIcon = "wic-sun-cloud-wind.svg";
                    break;

                case WeatherIcons.DAY_FOG:
                    fileIcon = "wic-sun-fog.svg";
                    break;

                case WeatherIcons.DAY_HAIL:
                    fileIcon = "wic-hail.svg";
                    break;

                case WeatherIcons.DAY_HAZE:
                    fileIcon = "wic-sun-fog.svg";
                    break;

                case WeatherIcons.DAY_LIGHTNING:
                    fileIcon = "wic-sun-cloud-lightning.svg";
                    break;

                case WeatherIcons.DAY_PARTLY_CLOUDY:
                    fileIcon = "wic-sun-cloud.svg";
                    break;

                case WeatherIcons.DAY_RAIN:
                case WeatherIcons.DAY_RAIN_MIX:
                case WeatherIcons.DAY_RAIN_WIND:
                case WeatherIcons.DAY_SHOWERS:
                case WeatherIcons.DAY_SLEET:
                case WeatherIcons.DAY_SLEET_STORM:
                case WeatherIcons.DAY_SPRINKLE:
                    fileIcon = "wic-sun-cloud-rain.svg";
                    break;

                case WeatherIcons.DAY_SNOW:
                case WeatherIcons.DAY_SNOW_THUNDERSTORM:
                case WeatherIcons.DAY_SNOW_WIND:
                    fileIcon = "wic-sun-cloud-snow.svg";
                    break;

                case WeatherIcons.DAY_STORM_SHOWERS:
                case WeatherIcons.DAY_THUNDERSTORM:
                    fileIcon = "wic-sun-cloud-lightning.svg";
                    break;

                case WeatherIcons.DAY_WINDY:
                case WeatherIcons.DAY_LIGHT_WIND:
                    fileIcon = "wic-sun-cloud-wind.svg";
                    break;

                // Night
                case WeatherIcons.NIGHT_CLEAR:
                    fileIcon = "wic-moon.svg";
                    break;

                case WeatherIcons.NIGHT_ALT_CLOUDY:
                case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY_HIGH:
                    fileIcon = "wic-moon-cloud.svg";
                    break;

                case WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS:
                case WeatherIcons.NIGHT_ALT_CLOUDY_WINDY:
                    fileIcon = "wic-moon-cloud-wind.svg";
                    break;

                case WeatherIcons.NIGHT_ALT_HAIL:
                    fileIcon = "wic-hail.svg";
                    break;

                case WeatherIcons.NIGHT_ALT_LIGHTNING:
                    fileIcon = "wic-moon-cloud-lightning.svg";
                    break;

                case WeatherIcons.NIGHT_ALT_RAIN:
                case WeatherIcons.NIGHT_ALT_RAIN_MIX:
                case WeatherIcons.NIGHT_ALT_RAIN_WIND:
                case WeatherIcons.NIGHT_ALT_SHOWERS:
                case WeatherIcons.NIGHT_ALT_SLEET:
                case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                case WeatherIcons.NIGHT_ALT_SPRINKLE:
                    fileIcon = "wic-moon-cloud-rain.svg";
                    break;

                case WeatherIcons.NIGHT_ALT_SNOW:
                case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                    fileIcon = "wic-moon-cloud-snow.svg";
                    break;

                case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                    fileIcon = "wic-moon-cloud-lightning.svg";
                    break;

                case WeatherIcons.NIGHT_FOG:
                    fileIcon = "wic-moon-fog.svg";
                    break;

                // Neutral
                case WeatherIcons.CLOUD:
                    fileIcon = "wic-cloud.svg";
                    break;

                case WeatherIcons.CLOUDY:
                    fileIcon = "wic-clouds.svg";
                    break;

                case WeatherIcons.CLOUDY_GUSTS:
                case WeatherIcons.CLOUDY_WINDY:
                    fileIcon = "wic-cloud-wind.svg";
                    break;

                case WeatherIcons.FOG:
                    fileIcon = "wic-fog.svg";
                    break;

                case WeatherIcons.HAIL:
                    fileIcon = "wic-hail.svg";
                    break;

                case WeatherIcons.RAIN:
                case WeatherIcons.RAIN_MIX:
                case WeatherIcons.RAIN_WIND:
                case WeatherIcons.SHOWERS:
                case WeatherIcons.SLEET:
                    fileIcon = "wic-cloud-rain.svg";
                    break;

                case WeatherIcons.SNOW:
                case WeatherIcons.SNOW_WIND:
                    fileIcon = "wic-cloud-snow.svg";
                    break;

                case WeatherIcons.SPRINKLE:
                    fileIcon = "wic-cloud-rain-single.svg";
                    break;

                case WeatherIcons.STORM_SHOWERS:
                case WeatherIcons.THUNDERSTORM:
                case WeatherIcons.LIGHTNING:
                    fileIcon = "wic-lightning.svg";
                    break;

                case WeatherIcons.SMOG:
                    fileIcon = "wi-smog.svg";
                    break;

                case WeatherIcons.SMOKE:
                    fileIcon = "wi-smoke.svg";
                    break;

                case WeatherIcons.DUST:
                    fileIcon = "wi-dust.svg";
                    break;

                case WeatherIcons.SNOWFLAKE_COLD:
                    fileIcon = "wic-snowflake.svg";
                    break;

                case WeatherIcons.WINDY:
                    fileIcon = "wic-wind.svg";
                    break;

                case WeatherIcons.STRONG_WIND:
                    fileIcon = "wic-wind-high.svg";
                    break;

                case WeatherIcons.SANDSTORM:
                    fileIcon = "wi-sandstorm.svg";
                    break;

                case WeatherIcons.HURRICANE:
                    fileIcon = "wi-hurricane.svg";
                    break;

                case WeatherIcons.TORNADO:
                    fileIcon = "wic-tornado.svg";
                    break;

                case WeatherIcons.FIRE:
                    fileIcon = "wi-fire.svg";
                    break;

                case WeatherIcons.FLOOD:
                    fileIcon = "wi-flood.svg";
                    break;

                case WeatherIcons.VOLCANO:
                    fileIcon = "wi-volcano.svg";
                    break;

                case WeatherIcons.BAROMETER:
                    fileIcon = "wic-barometer.svg";
                    break;

                case WeatherIcons.HUMIDITY:
                    fileIcon = "wic-raindrop.svg";
                    break;

                case WeatherIcons.MOONRISE:
                    fileIcon = "wi-moonrise.svg";
                    break;

                case WeatherIcons.MOONSET:
                    fileIcon = "wi-moonset.svg";
                    break;

                case WeatherIcons.RAINDROP:
                    fileIcon = "wi-raindrop.svg";
                    break;

                case WeatherIcons.RAINDROPS:
                    fileIcon = "wi-raindrops.svg";
                    break;

                case WeatherIcons.SUNRISE:
                    fileIcon = "wic-sunrise.svg";
                    break;

                case WeatherIcons.SUNSET:
                    fileIcon = "wic-sunset.svg";
                    break;

                case WeatherIcons.THERMOMETER:
                    fileIcon = "wic-thermometer-medium.svg";
                    break;

                case WeatherIcons.UMBRELLA:
                    fileIcon = "wic-umbrella.svg";
                    break;

                case WeatherIcons.WIND_DIRECTION:
                    fileIcon = "wic-compass.svg";
                    break;

                case WeatherIcons.DIRECTION_UP:
                    fileIcon = "wi-direction-up.svg";
                    break;

                case WeatherIcons.DIRECTION_DOWN:
                    fileIcon = "wi-direction-down.svg";
                    break;

                // Beaufort
                case WeatherIcons.WIND_BEAUFORT_0:
                    fileIcon = "wi-wind-beaufort-0.svg";
                    break;

                case WeatherIcons.WIND_BEAUFORT_1:
                    fileIcon = "wi-wind-beaufort-1.svg";
                    break;

                case WeatherIcons.WIND_BEAUFORT_2:
                    fileIcon = "wi-wind-beaufort-2.svg";
                    break;

                case WeatherIcons.WIND_BEAUFORT_3:
                    fileIcon = "wi-wind-beaufort-3.svg";
                    break;

                case WeatherIcons.WIND_BEAUFORT_4:
                    fileIcon = "wi-wind-beaufort-4.svg";
                    break;

                case WeatherIcons.WIND_BEAUFORT_5:
                    fileIcon = "wi-wind-beaufort-5.svg";
                    break;

                case WeatherIcons.WIND_BEAUFORT_6:
                    fileIcon = "wi-wind-beaufort-6.svg";
                    break;

                case WeatherIcons.WIND_BEAUFORT_7:
                    fileIcon = "wi-wind-beaufort-7.svg";
                    break;

                case WeatherIcons.WIND_BEAUFORT_8:
                    fileIcon = "wi-wind-beaufort-8.svg";
                    break;

                case WeatherIcons.WIND_BEAUFORT_9:
                    fileIcon = "wi-wind-beaufort-9.svg";
                    break;

                case WeatherIcons.WIND_BEAUFORT_10:
                    fileIcon = "wi-wind-beaufort-10.svg";
                    break;

                case WeatherIcons.WIND_BEAUFORT_11:
                    fileIcon = "wi-wind-beaufort-11.svg";
                    break;

                case WeatherIcons.WIND_BEAUFORT_12:
                    fileIcon = "wi-wind-beaufort-12.svg";
                    break;

                // Moon Phase
                case WeatherIcons.MOON_NEW:
                case WeatherIcons.MOON_ALT_NEW:
                    fileIcon = "wic-moon-newmoon.svg";
                    break;

                case WeatherIcons.MOON_WAXING_CRESCENT_3:
                case WeatherIcons.MOON_ALT_WAXING_CRESCENT_3:
                    fileIcon = "wic-moon-waxing-crescent.svg";
                    break;

                case WeatherIcons.MOON_FIRST_QUARTER:
                case WeatherIcons.MOON_ALT_FIRST_QUARTER:
                    fileIcon = "wic-moon-first-quarter.svg";
                    break;

                case WeatherIcons.MOON_WAXING_GIBBOUS_3:
                case WeatherIcons.MOON_ALT_WAXING_GIBBOUS_3:
                    fileIcon = "wic-moon-waxing-gibbous.svg";
                    break;

                case WeatherIcons.MOON_FULL:
                case WeatherIcons.MOON_ALT_FULL:
                    fileIcon = "wic-moon-fullmoon.svg";
                    break;

                case WeatherIcons.MOON_WANING_GIBBOUS_3:
                case WeatherIcons.MOON_ALT_WANING_GIBBOUS_3:
                    fileIcon = "wic-moon-waning-gibbous.svg";
                    break;

                case WeatherIcons.MOON_THIRD_QUARTER:
                case WeatherIcons.MOON_ALT_THIRD_QUARTER:
                    fileIcon = "wic-moon-last-quarter.svg";
                    break;

                case WeatherIcons.MOON_WANING_CRESCENT_3:
                case WeatherIcons.MOON_ALT_WANING_CRESCENT_3:
                    fileIcon = "wic-moon-waning-crescent.svg";
                    break;

                case WeatherIcons.FAHRENHEIT:
                    fileIcon = "wic-fahrenheit.svg";
                    break;

                case WeatherIcons.CELSIUS:
                    fileIcon = "wic-celsius.svg";
                    break;

                case WeatherIcons.UV_INDEX:
                case WeatherIcons.UV_INDEX_1:
                case WeatherIcons.UV_INDEX_2:
                case WeatherIcons.UV_INDEX_3:
                case WeatherIcons.UV_INDEX_4:
                case WeatherIcons.UV_INDEX_5:
                case WeatherIcons.UV_INDEX_6:
                case WeatherIcons.UV_INDEX_7:
                case WeatherIcons.UV_INDEX_8:
                case WeatherIcons.UV_INDEX_9:
                case WeatherIcons.UV_INDEX_10:
                case WeatherIcons.UV_INDEX_11:
                    fileIcon = "wic-sun.svg";
                    break;

                case WeatherIcons.NA:
                    fileIcon = "wui-unknown.svg";
                    break;
            }

            if (String.IsNullOrWhiteSpace(fileIcon))
            {
                // Not Available
                fileIcon = "wui-unknown.svg";
            }

            return baseuri + fileIcon;
        }
    }
}
