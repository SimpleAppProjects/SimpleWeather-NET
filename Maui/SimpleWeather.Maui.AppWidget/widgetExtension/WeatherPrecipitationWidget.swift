//
//  WeatherPrecipitationWidget.swift
//  SimpleWeather
//
//  Created by Dave Antoine on 2/16/24.
//

import Foundation
import SwiftUI
import WidgetKit

struct WeatherPrecipitationWidgetEntryView : View {
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
            case .accessoryCircular:
                WeatherPrecipitationWidgetCircular(model: entry.model!)
            default:
                WeatherPrecipitationWidgetInline(model: entry.model!)
            }
        }
    }
}

struct WeatherPrecipitationWidget: Widget {
    let kind: String = "precipitationWidget"
    
    func makeWidgetConfiguration() -> some WidgetConfiguration {
        /*
        if #available(iOS 17.0, macOS 14.0, *) {
            return AppIntentConfiguration(kind: kind, intent: WeatherWidgetConfigurationIntent.self, provider: Provider()) { entry in
                WeatherPrecipitationWidgetEntryView(entry: entry)
            }
        } else {*/
            return IntentConfiguration(kind: kind, intent: WeatherWidgetConfigurationIntent.self, provider: Provider()) { entry in
                WeatherPrecipitationWidgetEntryView(entry: entry)
            }
        //}
    }

    var body: some WidgetConfiguration {
        IntentConfiguration(kind: kind, intent: WeatherWidgetConfigurationIntent.self, provider: Provider()) { entry in
            WeatherPrecipitationWidgetEntryView(entry: entry)
        }
        .configurationDisplayName("Precipitation")
        .description("Check the chance of precipitation for a location")
        .supportedFamilies([
            .accessoryInline,
            .accessoryCircular
        ])
    }
}

struct WeatherPrecipitationWidgetPreviews: PreviewProvider {
    static var previews: some View {
        let entry = WeatherTimelineEntry(date: Date(), model: mockWeatherModel(), configuration: WeatherWidgetConfigurationIntent())

        WeatherPrecipitationWidgetEntryView(entry: entry)
            .previewContext(WidgetPreviewContext(family: .accessoryInline))

        WeatherPrecipitationWidgetEntryView(entry: entry)
            .previewContext(WidgetPreviewContext(family: .accessoryCircular))
    }
}
