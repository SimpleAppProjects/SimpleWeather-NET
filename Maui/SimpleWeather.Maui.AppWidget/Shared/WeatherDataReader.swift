//
//  WeatherDataReader.swift
//  widgetExtension
//
//  Created by Dave Antoine on 10/2/23.
//

import Foundation

func readWeatherData() -> Dictionary<String, WeatherData>? {
    if let url = FileManager.default.containerURL(forSecurityApplicationGroupIdentifier: getGroupIdentifier()) {
        let path = url.appendingPathComponent("widget/weatherData.json")
        let data = try? String(contentsOf: path)
        if let data = data {
            let jsonData = data.data(using: .utf8)!
            let value = try? JSONDecoder().decode(Dictionary<String, WeatherData>.self, from: jsonData)
            
            if let value = value {
                return value;
            } else {
                fileGroupLogger.write("Unable to decode weatherData.json")
                fileGroupLogger.write("JSON: \(data)")
            }
        } else {
            fileGroupLogger.write("Unable to read contents of weatherData.json")
        }
    }
    return nil
}
