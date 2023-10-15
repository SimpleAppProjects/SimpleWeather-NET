//
//  WeatherModel.swift
//  widgetExtension
//
//  Created by Dave Antoine on 10/2/23.
//

import Foundation

struct WeatherModel {
    var current: Current
    var forecasts: [Forecast] = []
    var showForecast: Bool
    var hourlyForecasts: [HourlyForecast] = []
    var showHourlyForecast: Bool
}

struct Current {
    var weatherIcon: String
    var temp: String
    var condition: String
    var chance: String?
    var location: String
    var isGPS: Bool
    var backgroundColor: String?
    var backgroundCode: String?
    var hi: String?
    var lo: String?
    var showHiLo: Bool
}

struct Forecast {
    var date: String
    var weatherIcon: String
    var hi: String
    var lo: String
    var chance: String?
}

struct HourlyForecast {
    var date: String
    var weatherIcon: String
    var hi: String
}

func mockWeatherModel() -> WeatherModel {
    return WeatherModel(
        current: Current(
            weatherIcon: WeatherIcons.DAY_SUNNY,
            temp: "70°",
            condition: "Sunny with a chance of rain",
            chance: "30%",
            location: "New York, New York, USA",
            isGPS: true,
            backgroundColor: "#ff88b0c8",
            backgroundCode: WeatherBackground.DAY,
            hi: "75°",
            lo: "65°",
            showHiLo: true
        ),
        forecasts: (0...6).map {index in
            let formatter = DateFormatter()
            formatter.dateFormat = "EEE"
            
            let date = Date(timeIntervalSinceNow: Double(index) * 86400)
            
            return Forecast(
                date: formatter.string(from: date),
                weatherIcon: WeatherIcons.DAY_SUNNY,
                hi: "\((75 + Int.random(in: -5...5)))°",
                lo: "\((65 + Int.random(in: -5...5)))°",
                chance: "\(Int.random(in: 0...100))%"
            )
        },
        showForecast: true,
        hourlyForecasts: (0...12).map {index in
            let formatter = DateFormatter()
            formatter.dateFormat = Locale.current.is24Hour ? "HH:00" : "ha"
            
            let date = Date(timeIntervalSinceNow: Double(index) * 3600)
            
            return HourlyForecast(
                date: formatter.string(from: date),
                weatherIcon: WeatherIcons.NIGHT_CLEAR,
                hi: "\((60 + Int.random(in: -5...5)))°"
            )
        },
        showHourlyForecast: true
    )
}
