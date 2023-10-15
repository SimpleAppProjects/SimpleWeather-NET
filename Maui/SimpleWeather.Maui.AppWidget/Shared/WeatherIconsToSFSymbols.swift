//
//  WeatherIconsToSFSymbols.swift
//  SimpleWeather
//
//  Created by Dave Antoine on 10/6/23.
//

import Foundation

func iconToSFSymbol(icon: String) -> String {
    switch icon {
    // Day
    case WeatherIcons.DAY_SUNNY:
        return "sun.max.fill"

    case WeatherIcons.DAY_CLOUDY,
        WeatherIcons.DAY_CLOUDY_GUSTS,
        WeatherIcons.DAY_CLOUDY_WINDY,
        WeatherIcons.DAY_CLOUDY_HIGH,
        WeatherIcons.DAY_SUNNY_OVERCAST,
        WeatherIcons.DAY_PARTLY_CLOUDY:
        return "cloud.sun.fill"

    case WeatherIcons.DAY_FOG:
        return "cloud.fog.fill"

    case WeatherIcons.DAY_HAIL:
        return "cloud.hail.fill"

    case WeatherIcons.DAY_HAZE:
        return "sun.haze.fill"

    case WeatherIcons.DAY_LIGHTNING,
        WeatherIcons.DAY_THUNDERSTORM:
        return "cloud.sun.bolt.fill"

    case WeatherIcons.DAY_RAIN,
        WeatherIcons.DAY_RAIN_MIX,
        WeatherIcons.DAY_SHOWERS,
        WeatherIcons.DAY_STORM_SHOWERS:
        return "cloud.sun.rain.fill"
        
    case WeatherIcons.DAY_RAIN_WIND:
        return "cloud.heavyrain.fill"
        
    case WeatherIcons.DAY_SPRINKLE:
        return "cloud.drizzle.fill"

    case WeatherIcons.DAY_SLEET,
        WeatherIcons.DAY_SLEET_STORM:
        return "cloud.sleet.fill"

    case WeatherIcons.DAY_SNOW,
        WeatherIcons.DAY_SNOW_THUNDERSTORM:
        return "sun.snow.fill"

    case WeatherIcons.DAY_SNOW_WIND:
        return "wind.snow"

    case WeatherIcons.DAY_LIGHT_WIND,
        WeatherIcons.DAY_WINDY:
        return "wind"

    case WeatherIcons.DAY_HOT:
        return "thermometer.sun.fill"

    case WeatherIcons.DAY_SUNNY:
        return "sun.max.fill"
        
    // Night
    case WeatherIcons.NIGHT_CLEAR:
        return "moon.stars.fill"

    case WeatherIcons.NIGHT_ALT_CLOUDY,
        WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS,
        WeatherIcons.NIGHT_ALT_CLOUDY_WINDY,
        WeatherIcons.NIGHT_ALT_CLOUDY_HIGH,
        WeatherIcons.NIGHT_OVERCAST,
        WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
        return "cloud.moon.fill"

    case WeatherIcons.NIGHT_FOG:
        return "cloud.fog.fill"

    case WeatherIcons.NIGHT_ALT_HAIL:
        return "cloud.hail.fill"

    case WeatherIcons.NIGHT_HAZE:
        return "moon.haze.fill"

    case WeatherIcons.NIGHT_ALT_LIGHTNING,
        WeatherIcons.NIGHT_ALT_THUNDERSTORM:
        return "cloud.moon.bolt.fill"

    case WeatherIcons.NIGHT_ALT_RAIN,
        WeatherIcons.NIGHT_ALT_RAIN_MIX,
        WeatherIcons.NIGHT_ALT_SHOWERS,
        WeatherIcons.NIGHT_ALT_SPRINKLE,
        WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
        return "cloud.moon.rain.fill"
        
    case WeatherIcons.NIGHT_ALT_RAIN_WIND:
        return "cloud.heavyrain.fill"
        
    case WeatherIcons.NIGHT_ALT_SPRINKLE:
        return "cloud.drizzle.fill"

    case WeatherIcons.NIGHT_ALT_SLEET,
        WeatherIcons.NIGHT_ALT_SLEET_STORM:
        return "cloud.sleet.fill"

    case WeatherIcons.NIGHT_ALT_SNOW,
        WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
        return "cloud.snow.fill"

    case WeatherIcons.NIGHT_ALT_SNOW_WIND:
        return "wind.snow"

    case WeatherIcons.NIGHT_LIGHT_WIND,
        WeatherIcons.NIGHT_WINDY:
        return "wind"

    case WeatherIcons.NIGHT_HOT:
        return "thermometer.high"
        
    // Neutral
    case WeatherIcons.CLOUD,
        WeatherIcons.CLOUDY,
        WeatherIcons.OVERCAST:
        return "cloud.fill"

    case WeatherIcons.CLOUDY_GUSTS,
        WeatherIcons.CLOUDY_WINDY,
        WeatherIcons.WINDY,
        WeatherIcons.LIGHT_WIND,
        WeatherIcons.STRONG_WIND:
        return "wind"
        
    case WeatherIcons.FOG:
        return "cloud.fog.fill"
        
    case WeatherIcons.HAIL:
        return "cloud.hail.fill"
        
    case WeatherIcons.HAZE:
        return "cloud.fog.fill"
        
    case WeatherIcons.LIGHTNING:
        return "bolt.fill"
        
    case WeatherIcons.RAIN,
        WeatherIcons.RAIN_MIX,
        WeatherIcons.SHOWERS,
        WeatherIcons.STORM_SHOWERS:
        return "cloud.rain.fill"
        
    case WeatherIcons.RAIN_WIND:
        return "cloud.heavyrain.fill"
        
    case WeatherIcons.SLEET,
        WeatherIcons.SLEET_STORM:
        return "cloud.sleet.fill"
        
    case WeatherIcons.SNOW,
        WeatherIcons.SNOW_THUNDERSTORM:
        return "cloud.snow.fill"
        
    case WeatherIcons.SNOW_WIND:
        return "wind.snow"
        
    case WeatherIcons.SPRINKLE:
        return "cloud.drizzle.fill"
        
    case WeatherIcons.THUNDERSTORM:
        return "cloud.bolt.fill"
        
    case WeatherIcons.HOT:
        return "thermometer.high"
        
    case WeatherIcons.SMOG,
        WeatherIcons.SMOKE:
        return "smoke.fill"
        
    case WeatherIcons.DUST,
        WeatherIcons.SANDSTORM:
        return "cloud.fog.fill"
        
    case WeatherIcons.SNOWFLAKE_COLD:
        return "thermometer.snowflake"
        
    case WeatherIcons.HURRICANE:
        return "hurricane"
        
    case WeatherIcons.TORNADO:
        return "tornado"
        
    case WeatherIcons.FIRE:
        return "flame.fill"
    case WeatherIcons.FLOOD:
        return "water.waves"
    case WeatherIcons.VOLCANO:
        return "mountain.2.fill"
        
    case WeatherIcons.BAROMETER:
        return "barometer"
    case WeatherIcons.HUMIDITY:
        return "humidity.fill"
    case WeatherIcons.MOONRISE:
        return "moonrise.fill"
    case WeatherIcons.MOONSET:
        return "moonset.fill"
    case WeatherIcons.RAINDROP,
        WeatherIcons.RAINDROPS:
        return "drop.fill"
    case WeatherIcons.SUNRISE:
        return "sunrise.fill"
    case WeatherIcons.SUNSET:
        return "sunset.fill"
    case WeatherIcons.THERMOMETER:
        return "thermometer.medium"
    case WeatherIcons.UMBRELLA:
        return "umbrella.fill"
    case WeatherIcons.WIND_DIRECTION:
        return "arrow.up.circle"
    case WeatherIcons.DIRECTION_UP:
        return "arrow.up"
    case WeatherIcons.DIRECTION_DOWN:
        return "arrow.down"

    default:
        return "questionmark.circle"
    }
}
