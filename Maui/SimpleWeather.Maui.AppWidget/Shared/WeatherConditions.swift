//
//  WeatherConditions.swift
//  SimpleWeather
//
//  Created by Dave Antoine on 12/29/23.
//

import Foundation

public struct WeatherConditions
{
    private init() {}
    
    static let weather_blizzard: String = "Blizzard"
    static let weather_blowingsnow: String = "Blowing Snow"
    static let weather_clear: String = "Clear"
    static let weather_clearsky: String = "Clear sky"
    static let weather_cloudy: String = "Cloudy"
    static let weather_cold: String = "Cold"
    static let weather_drizzle: String = "Drizzle"
    static let weather_dust: String = "Dust"
    static let weather_fair: String = "Fair"
    static let weather_fog: String = "Fog"
    static let weather_foggy: String = "Foggy"
    static let weather_freezingrain: String = "Freezing Rain"
    static let weather_hail: String = "Hail"
    static let weather_haze: String = "Haze"
    static let weather_heavyrain: String = "Heavy Rain"
    static let weather_heavysnow: String = "Heavy Snow"
    static let weather_hot: String = "Hot"
    static let weather_hurricane: String = "Hurricane"
    static let weather_isotstorms: String = "Isolated Thunderstorms"
    static let weather_lightfog: String = "Light Fog"
    static let weather_lightrain: String = "Light Rain"
    static let weather_lightsnowshowers: String = "Light Snow"
    static let weather_lightwind: String = "Light Wind"
    static let weather_mostlyclear: String = "Mostly Clear"
    static let weather_mostlycloudy: String = "Mostly Cloudy"
    static let weather_notavailable: String = "N/A"
    static let weather_overcast: String = "Overcast"
    static let weather_partlycloudy: String = "Partly Cloudy"
    static let weather_rain: String = "Rain"
    static let weather_rainandhail: String = "Mixed Rain and Hail"
    static let weather_rainandsleet: String = "Mixed Rain and Sleet"
    static let weather_rainandsnow: String = "Mixed Rain and Snow"
    static let weather_rainshowers: String = "Rain Showers"
    static let weather_scatteredshowers: String = "Scattered Showers"
    static let weather_scatteredtstorms: String = "Scattered Thunderstorms"
    static let weather_severetstorms: String = "Severe Thunderstorms"
    static let weather_sleet: String = "Sleet"
    static let weather_sleet_tstorms: String = "Sleet and Thunder"
    static let weather_smog: String = "Smog"
    static let weather_smoky: String = "Smoky"
    static let weather_snow: String = "Snow"
    static let weather_snow_tstorms: String = "Snow and Thunder"
    static let weather_snowandsleet: String = "Mixed Snow and Sleet"
    static let weather_snowflurries: String = "Snow Flurries"
    static let weather_sunny: String = "Sunny"
    static let weather_tornado: String = "Tornado"
    static let weather_tropicalstorm: String = "Tropical Storm"
    static let weather_tstorms: String = "Thunderstorms"
    static let weather_windy: String = "Windy"
}
