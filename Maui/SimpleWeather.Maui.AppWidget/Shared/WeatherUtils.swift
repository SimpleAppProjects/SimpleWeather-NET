//
//  WeatherUtils.swift
//  SimpleWeather
//
//  Created by Dave Antoine on 12/29/23.
//

import Foundation

class WeatherUtils {
    static func getWeatherCondition(forIcon icon: String) -> String {
        switch icon {
        case WeatherIcons.DAY_SUNNY: return WeatherConditions.weather_sunny
        case WeatherIcons.NIGHT_CLEAR: return WeatherConditions.weather_clear
        case WeatherIcons.DAY_SUNNY_OVERCAST,
            WeatherIcons.NIGHT_OVERCAST,
            WeatherIcons.OVERCAST: return WeatherConditions.weather_overcast
        case WeatherIcons.DAY_PARTLY_CLOUDY,
            WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY: return WeatherConditions.weather_partlycloudy
        case WeatherIcons.DAY_CLOUDY,
            WeatherIcons.NIGHT_ALT_CLOUDY,
            WeatherIcons.CLOUDY,
            WeatherIcons.NIGHT_ALT_CLOUDY_HIGH,
            WeatherIcons.DAY_CLOUDY_HIGH: return WeatherConditions.weather_cloudy
        case WeatherIcons.DAY_SPRINKLE,
            WeatherIcons.NIGHT_ALT_SPRINKLE,
            WeatherIcons.SPRINKLE,
            WeatherIcons.DAY_SHOWERS,
            WeatherIcons.NIGHT_ALT_SHOWERS,
            WeatherIcons.SHOWERS: return WeatherConditions.weather_rainshowers
        case WeatherIcons.DAY_THUNDERSTORM,
            WeatherIcons.NIGHT_ALT_THUNDERSTORM,
            WeatherIcons.THUNDERSTORM,
            WeatherIcons.DAY_STORM_SHOWERS,
            WeatherIcons.NIGHT_ALT_STORM_SHOWERS,
            WeatherIcons.STORM_SHOWERS,
            WeatherIcons.DAY_LIGHTNING,
            WeatherIcons.NIGHT_ALT_LIGHTNING,
            WeatherIcons.LIGHTNING: return WeatherConditions.weather_tstorms
        case WeatherIcons.DAY_SLEET,
            WeatherIcons.NIGHT_ALT_SLEET,
            WeatherIcons.SLEET: return WeatherConditions.weather_sleet
        case WeatherIcons.DAY_SNOW,
            WeatherIcons.NIGHT_ALT_SNOW,
            WeatherIcons.SNOW: return WeatherConditions.weather_snow
        case WeatherIcons.DAY_SNOW_WIND,
            WeatherIcons.NIGHT_ALT_SNOW_WIND,
            WeatherIcons.SNOW_WIND: return WeatherConditions.weather_heavysnow
        case WeatherIcons.DAY_SNOW_THUNDERSTORM,
            WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM,
            WeatherIcons.SNOW_THUNDERSTORM: return WeatherConditions.weather_snow_tstorms
        case WeatherIcons.HAIL,
            WeatherIcons.DAY_HAIL,
            WeatherIcons.NIGHT_ALT_HAIL: return WeatherConditions.weather_hail
        case WeatherIcons.DAY_RAIN,
            WeatherIcons.NIGHT_ALT_RAIN,
            WeatherIcons.RAIN: return WeatherConditions.weather_rain
        case WeatherIcons.DAY_FOG,
            WeatherIcons.NIGHT_FOG,
            WeatherIcons.FOG: return WeatherConditions.weather_fog
        case WeatherIcons.DAY_SLEET_STORM,
            WeatherIcons.NIGHT_ALT_SLEET_STORM,
            WeatherIcons.SLEET_STORM: return WeatherConditions.weather_sleet_tstorms
        case WeatherIcons.SNOWFLAKE_COLD: return WeatherConditions.weather_cold
        case WeatherIcons.DAY_HOT,
            WeatherIcons.NIGHT_HOT,
            WeatherIcons.HOT: return WeatherConditions.weather_hot
        case WeatherIcons.DAY_HAZE,
            WeatherIcons.NIGHT_HAZE,
            WeatherIcons.HAZE: return WeatherConditions.weather_haze
        case WeatherIcons.SMOKE: return WeatherConditions.weather_smoky
        case WeatherIcons.SANDSTORM,
            WeatherIcons.DUST: return WeatherConditions.weather_dust
        case WeatherIcons.TORNADO: return WeatherConditions.weather_tornado
        case WeatherIcons.DAY_RAIN_MIX,
            WeatherIcons.NIGHT_ALT_RAIN_MIX,
            WeatherIcons.RAIN_MIX: return WeatherConditions.weather_rainandsnow
        case WeatherIcons.DAY_CLOUDY_WINDY,
            WeatherIcons.NIGHT_ALT_CLOUDY_WINDY,
            WeatherIcons.CLOUDY_WINDY,
            WeatherIcons.DAY_CLOUDY_GUSTS,
            WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS,
            WeatherIcons.CLOUDY_GUSTS,
            WeatherIcons.STRONG_WIND: return WeatherConditions.weather_windy
        case WeatherIcons.HURRICANE: return WeatherConditions.weather_tropicalstorm
        case WeatherIcons.DAY_RAIN_WIND,
            WeatherIcons.NIGHT_ALT_RAIN_WIND,
            WeatherIcons.RAIN_WIND: return WeatherConditions.weather_heavyrain
        case WeatherIcons.DAY_LIGHT_WIND,
            WeatherIcons.NIGHT_LIGHT_WIND,
            WeatherIcons.LIGHT_WIND: return WeatherConditions.weather_lightwind
        case WeatherIcons.SMOG: return WeatherConditions.weather_smog
        default: return WeatherConditions.weather_notavailable
        }
    }
    
    static func isNight(icon: String) -> Bool {
        switch icon {
        case WeatherIcons.NIGHT_CLEAR,
            WeatherIcons.NIGHT_ALT_CLOUDY,
            WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS,
            WeatherIcons.NIGHT_ALT_CLOUDY_WINDY,
            WeatherIcons.NIGHT_FOG,
            WeatherIcons.NIGHT_ALT_HAIL,
            WeatherIcons.NIGHT_HAZE,
            WeatherIcons.NIGHT_ALT_LIGHTNING,
            WeatherIcons.NIGHT_ALT_RAIN,
            WeatherIcons.NIGHT_ALT_RAIN_MIX,
            WeatherIcons.NIGHT_ALT_RAIN_WIND,
            WeatherIcons.NIGHT_ALT_SHOWERS,
            WeatherIcons.NIGHT_ALT_SLEET,
            WeatherIcons.NIGHT_ALT_SLEET_STORM,
            WeatherIcons.NIGHT_ALT_SNOW,
            WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM,
            WeatherIcons.NIGHT_ALT_SNOW_WIND,
            WeatherIcons.NIGHT_ALT_SPRINKLE,
            WeatherIcons.NIGHT_ALT_STORM_SHOWERS,
            WeatherIcons.NIGHT_OVERCAST,
            WeatherIcons.NIGHT_ALT_THUNDERSTORM,
            WeatherIcons.NIGHT_WINDY,
            WeatherIcons.NIGHT_HOT,
            WeatherIcons.NIGHT_ALT_CLOUDY_HIGH,
            WeatherIcons.NIGHT_LIGHT_WIND,
            WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
            return true
        default:
            return false
        }
    }
    
    static func getBackgroundCodeFromIcon(icon: String) -> String {
        return getBackgroundCodeFromIcon(icon: icon, isNight: isNight(icon: icon))
    }
    
    static func getBackgroundCodeFromIcon(icon: String, isNight: Bool) -> String {
        var backgroundCode: String
        
        switch icon {
            // Rain
        case WeatherIcons.DAY_HAIL,
            WeatherIcons.DAY_RAIN,
            WeatherIcons.DAY_RAIN_MIX,
            WeatherIcons.DAY_RAIN_WIND,
            WeatherIcons.DAY_SHOWERS,
            WeatherIcons.DAY_SLEET,
            WeatherIcons.DAY_SPRINKLE:
            backgroundCode = WeatherBackground.RAIN
            
        case WeatherIcons.NIGHT_ALT_HAIL,
            WeatherIcons.NIGHT_ALT_RAIN,
            WeatherIcons.NIGHT_ALT_RAIN_MIX,
            WeatherIcons.NIGHT_ALT_RAIN_WIND,
            WeatherIcons.NIGHT_ALT_SHOWERS,
            WeatherIcons.NIGHT_ALT_SLEET,
            WeatherIcons.NIGHT_ALT_SPRINKLE,
            WeatherIcons.RAIN,
            WeatherIcons.RAIN_MIX,
            WeatherIcons.RAIN_WIND,
            WeatherIcons.SHOWERS,
            WeatherIcons.SLEET,
            WeatherIcons.SPRINKLE:
            backgroundCode = WeatherBackground.RAIN_NIGHT
            
            // Tornado / Hurricane / Thunderstorm / Tropical Storm
        case WeatherIcons.DAY_LIGHTNING,
            WeatherIcons.DAY_THUNDERSTORM,
            WeatherIcons.NIGHT_ALT_LIGHTNING,
            WeatherIcons.NIGHT_ALT_THUNDERSTORM,
            WeatherIcons.LIGHTNING,
            WeatherIcons.THUNDERSTORM:
            backgroundCode = WeatherBackground.TSTORMS_NIGHT
            
        case WeatherIcons.DAY_STORM_SHOWERS,
            WeatherIcons.DAY_SLEET_STORM,
            WeatherIcons.STORM_SHOWERS,
            WeatherIcons.SLEET_STORM,
            WeatherIcons.NIGHT_ALT_STORM_SHOWERS,
            WeatherIcons.NIGHT_ALT_SLEET_STORM,
            WeatherIcons.HAIL,
            WeatherIcons.HURRICANE,
            WeatherIcons.TORNADO:
            backgroundCode = WeatherBackground.STORMS
            
            // Dust
        case WeatherIcons.DUST,
            WeatherIcons.SANDSTORM:
            backgroundCode = WeatherBackground.DUST
            
            // Foggy / Haze
        case WeatherIcons.DAY_FOG,
            WeatherIcons.DAY_HAZE,
            WeatherIcons.FOG,
            WeatherIcons.HAZE,
            WeatherIcons.NIGHT_FOG,
            WeatherIcons.NIGHT_HAZE,
            WeatherIcons.SMOG,
            WeatherIcons.SMOKE:
            backgroundCode = WeatherBackground.FOG
            
            // Snow / Snow Showers/Storm
        case WeatherIcons.DAY_SNOW,
            WeatherIcons.DAY_SNOW_THUNDERSTORM,
            WeatherIcons.DAY_SNOW_WIND,
            WeatherIcons.NIGHT_ALT_SNOW,
            WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM,
            WeatherIcons.NIGHT_ALT_SNOW_WIND,
            WeatherIcons.SNOW,
            WeatherIcons.SNOW_THUNDERSTORM,
            WeatherIcons.SNOW_WIND:
            backgroundCode = WeatherBackground.SNOW
            
            // (Mostly) Cloudy
        case WeatherIcons.CLOUD,
            WeatherIcons.CLOUDY,
            WeatherIcons.CLOUDY_GUSTS,
            WeatherIcons.CLOUDY_WINDY,
            WeatherIcons.DAY_CLOUDY,
            WeatherIcons.DAY_CLOUDY_GUSTS,
            WeatherIcons.DAY_CLOUDY_HIGH,
            WeatherIcons.DAY_CLOUDY_WINDY,
            WeatherIcons.NIGHT_ALT_CLOUDY,
            WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS,
            WeatherIcons.NIGHT_ALT_CLOUDY_HIGH,
            WeatherIcons.NIGHT_ALT_CLOUDY_WINDY,
            WeatherIcons.DAY_SUNNY_OVERCAST,
            WeatherIcons.NIGHT_OVERCAST,
            WeatherIcons.OVERCAST:
            backgroundCode = isNight ? WeatherBackground.MOSTLYCLOUDY_NIGHT : WeatherBackground.MOSTLYCLOUDY_DAY
            
            // Partly Cloudy
        case WeatherIcons.DAY_PARTLY_CLOUDY,
            WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
            backgroundCode = isNight ? WeatherBackground.PARTLYCLOUDY_NIGHT : WeatherBackground.PARTLYCLOUDY_DAY
            
        case WeatherIcons.DAY_SUNNY,
            WeatherIcons.NA,
            WeatherIcons.NIGHT_CLEAR,
            WeatherIcons.SNOWFLAKE_COLD,
            WeatherIcons.DAY_HOT,
            WeatherIcons.NIGHT_HOT,
            WeatherIcons.HOT,
            WeatherIcons.DAY_WINDY,
            WeatherIcons.NIGHT_WINDY,
            WeatherIcons.WINDY,
            WeatherIcons.DAY_LIGHT_WIND,
            WeatherIcons.NIGHT_LIGHT_WIND,
            WeatherIcons.LIGHT_WIND,
            WeatherIcons.STRONG_WIND:
            
            // Set background based on sunset/rise times
            if (isNight) {
                backgroundCode = WeatherBackground.NIGHT
            } else {
                backgroundCode = WeatherBackground.DAY
            }
            
        default:
            // Set background based on sunset/rise times
            if (isNight) {
                backgroundCode = WeatherBackground.NIGHT
            } else {
                backgroundCode = WeatherBackground.DAY
            }
        }
        
        return backgroundCode
    }
}
