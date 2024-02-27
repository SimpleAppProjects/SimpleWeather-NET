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
    func checkForOutdatedObservation() -> WeatherData {
        var now = Date()
        guard let tz = TimeZone(identifier: current.tz_long) else {
            return self
        }
        
        let tzDiffFromUTCinSecs = tz.secondsFromGMT(for: now)
        
        now = now.addingTimeInterval(Double(tzDiffFromUTCinSecs))

        let _forecasts = forecasts?.filter { f in
            let date = Date(timeIntervalSince1970: f.epochTime)
            
            // ex) from 12PM - to 3PM
            let daysDiff = Calendar.current.dateComponents([.day], from: now, to: date).day ?? 0
            
            return daysDiff >= 0
        }
        
        var _current = self.current

        let hrForecasts = hr_forecasts?.filter { f in
            let date = Date(timeIntervalSince1970: f.epochTime)
            
            // ex) from 12PM - to 3PM
            let hoursDiff = Calendar.current.dateComponents([.hour], from: now, to: date).hour ?? 0
            
            // Update current condition if needed
            if (f.epochTime == hr_forecasts!.first!.epochTime && hoursDiff > 1) {
                _current = _Current(
                    locationName: current.locationName,
                    tz_long: current.tz_long,
                    isGPS: current.isGPS,
                    temp: f.temp,
                    condition: WeatherUtils.getWeatherCondition(forIcon: f.icon),
                    icon: f.icon,
                    showHiLo: false
                )
                _current.backgroundCode = WeatherUtils.getBackgroundCodeFromIcon(icon: f.icon)
            }
            
            return hoursDiff > 1
        }
        
        return WeatherData(
            current: _current,
            forecasts: _forecasts,
            hr_forecasts: hrForecasts
        )
    }
    
    func toModel() -> WeatherModel {
        let tzStr = current.tz_long
        let tz = TimeZone(identifier: tzStr)

        let fcastDateFmt = DateFormatter()
        fcastDateFmt.timeZone = TimeZone(secondsFromGMT: 0) // use date as is
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
