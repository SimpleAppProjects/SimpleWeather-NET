//
//  WeatherLocationWidget.swift
//  widgetExtension
//
//  Created by Dave Antoine on 2/16/24.
//

import Foundation
import SwiftUI
import WidgetKit

struct WeatherLocationWidgetEntryView : View {
    var entry: Provider.Entry
    
    @Environment(\.widgetFamily) var family

    @ViewBuilder
    var body: some View {
        switch entry.status {
        case .locationMissing:
            Text("Location missing")
        case .noWeather:
            Text("No Weather")
        case .weatherValid:
            switch family {
            default:
                WeatherLocationWidgetInline(model: entry.model!)
            }
        }
    }
}

struct WeatherLocationWidget: Widget {
    let kind: String = "locationWidget"
    
    func makeWidgetConfiguration() -> some WidgetConfiguration {
        /*
        if #available(iOS 17.0, macOS 14.0, *) {
            return AppIntentConfiguration(kind: kind, intent: WeatherWidgetConfigurationIntent.self, provider: Provider()) { entry in
                WeatherLocationWidgetEntryView(entry: entry)
            }
        } else {*/
            return IntentConfiguration(kind: kind, intent: WeatherWidgetConfigurationIntent.self, provider: Provider()) { entry in
                WeatherLocationWidgetEntryView(entry: entry)
            }
        //}
    }

    var body: some WidgetConfiguration {
        IntentConfiguration(kind: kind, intent: WeatherWidgetConfigurationIntent.self, provider: Provider()) { entry in
            WeatherLocationWidgetEntryView(entry: entry)
        }
        .configurationDisplayName("Location")
        .description("Check the current weather conditions for a location")
        .supportedFamilies([.accessoryInline])
    }
}

struct WeatherLocationWidgetPreviews: PreviewProvider {
    static var previews: some View {
        let entry = WeatherTimelineEntry(date: Date(), model: mockWeatherModel(), configuration: WeatherWidgetConfigurationIntent())

        WeatherLocationWidgetEntryView(entry: entry)
            .previewContext(WidgetPreviewContext(family: .accessoryInline))
    }
}
