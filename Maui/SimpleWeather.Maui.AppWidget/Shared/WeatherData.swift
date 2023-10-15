//
//  WeatherData.swift
//  widgetExtension
//
//  Created by Dave Antoine on 10/2/23.
//

import Foundation

struct WeatherData : Codable {
    var current: _Current
    var forecasts: [_Forecast]?
    var hr_forecasts: [_HourlyForecast]?
}

internal struct _Current : Codable {
    var locationName: String
    var tz_long: String
    var isGPS: Bool

    var temp: String
    var condition: String
    var icon: String
    var hi: String?
    var lo: String?
    var showHiLo: Bool

    var chance: String?
    
    var backgroundColor: String?
    var backgroundCode: String?
}

internal struct _Forecast : Codable {
    var epochTime: Double
    var hi: String
    var lo: String
    var icon: String
    var chance: String?
}

internal struct _HourlyForecast : Codable {
    var epochTime: Double
    var temp: String
    var icon: String
}

extension Locale {
    var is24Hour: Bool {
        let dateFormat = DateFormatter.dateFormat(fromTemplate: "j", options: 0, locale: self)!
        return dateFormat.firstIndex(of: "a") == nil
    }
}

extension WeatherData {
    func toModel() -> WeatherModel {
        let tzStr = current.tz_long
        let tz = TimeZone(identifier: tzStr)

        let fcastDateFmt = DateFormatter()
        fcastDateFmt.timeZone = tz
        fcastDateFmt.dateFormat = "EEE"
        
        let hrfcastDateFmt = DateFormatter()
        hrfcastDateFmt.timeZone = tz
        hrfcastDateFmt.dateFormat = Locale.current.is24Hour ? "HH:00" : "ha"
        
        let fcasts = forecasts?.map { f in
            Forecast(
                date: fcastDateFmt.string(from: Date(timeIntervalSince1970: f.epochTime)),
                weatherIcon: f.icon,
                hi: f.hi,
                lo: f.lo,
                chance: f.chance
            )
        }
        
        let hrfcasts = hr_forecasts?.map { f in
            HourlyForecast(
                date: hrfcastDateFmt.string(from: Date(timeIntervalSince1970: f.epochTime)),
                weatherIcon: f.icon,
                hi: f.temp
            )
        }
        
        let cur = Current(
            weatherIcon: current.icon,
            temp: current.temp,
            condition: current.condition,
            chance: current.chance,
            location: current.locationName,
            isGPS: current.isGPS,
            backgroundColor: current.backgroundColor,
            backgroundCode: current.backgroundCode,
            hi: current.hi,
            lo: current.lo,
            showHiLo: current.showHiLo
        )
        
        return WeatherModel(
            current: cur,
            forecasts: fcasts ?? [],
            showForecast: !(fcasts?.isEmpty ?? true),
            hourlyForecasts: hrfcasts ?? [],
            showHourlyForecast: !(hrfcasts?.isEmpty ?? true)
        )
    }
}
