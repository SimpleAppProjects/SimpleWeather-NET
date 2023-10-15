//
//  IntentHandler.swift
//  widgetintentExt
//
//  Created by Dave Antoine on 10/6/23.
//

import Intents

// As an example, this class is set up to handle Message intents.
// You will want to replace this or add other intents as appropriate.
// The intents you wish to handle must be declared in the extension's Info.plist.

// You can test your example integration by saying things to Siri like:
// "Send a message using <myApp>"
// "<myApp> John saying hello"
// "Search for messages in <myApp>"

class IntentHandler: INExtension, WeatherWidgetConfigurationIntentHandling {
    func provideLocationOptionsCollection(for intent: WeatherWidgetConfigurationIntent) async throws -> INObjectCollection<Location> {
        let weatherMap = readWeatherData()
        
        var locations: [Location] = []
        
        guard weatherMap != nil else {
            fileGroupLogger.write("Error: provideLocationOptionsCollection - Weather map null ...")
            return INObjectCollection(items: locations)
        }
        
        locations += weatherMap!.map { key, value in
            return Location(
                identifier: key,
                display: key == "GPS" ? "My Location" : value.current.locationName
            )
        }
        
        let collection = INObjectCollection(items: locations)
        
        return collection
    }
    
    func defaultLocation(for intent: WeatherWidgetConfigurationIntent) -> Location? {
        let weatherMap = readWeatherData()
        
        guard let weatherMap = weatherMap else {
            fileGroupLogger.write("Error: defaultLocation - Weather map null ...")
            return nil
        }
        
        if (!weatherMap.isEmpty) {
            if (weatherMap["GPS"] != nil) {
                return Location(identifier: "GPS", display: "My Location")
            } else {
                let first = weatherMap.first
                return Location(identifier: first!.key, display: first!.value.current.locationName)
            }
        }
        
        return nil
    }
    
    override func handler(for intent: INIntent) -> Any {
        return self
    }
}
