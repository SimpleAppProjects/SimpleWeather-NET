//
//  widget.swift
//  widget
//
//  Created by Dave Antoine on 10/2/23.
//

import WidgetKit
import SwiftUI
import Intents

enum EntryStatus {
    case weatherValid
    case locationMissing
    case noWeather
}

struct WeatherTimelineEntry: TimelineEntry {
    let date: Date
    let model: WeatherModel?
    let configuration: WeatherWidgetConfigurationIntent
    var status: EntryStatus = .weatherValid
}

struct Provider: IntentTimelineProvider {
    func placeholder(in context: Context) -> WeatherTimelineEntry {
        WeatherTimelineEntry(date: Date(), model: mockWeatherModel(), configuration: WeatherWidgetConfigurationIntent())
    }

    func getSnapshot(for configuration: WeatherWidgetConfigurationIntent, in context: Context, completion: @escaping (WeatherTimelineEntry) -> ()) {
        let entry = WeatherTimelineEntry(date: Date(), model: mockWeatherModel(), configuration: configuration)
        completion(entry)
    }

    func getTimeline(for configuration: WeatherWidgetConfigurationIntent, in context: Context, completion: @escaping (Timeline<WeatherTimelineEntry>) -> ()) {
        let locationKey = configuration.location?.identifier ?? "GPS"
        
        let weatherMap = readWeatherData()
        
        guard weatherMap != nil else {
            let timeline = createTimeline(entry: createTimelineEntry(model: nil, status: .noWeather, config: configuration))
            completion(timeline)
            return
        }
        
        let data = weatherMap?[locationKey]?.checkForOutdatedObservation()
        let model = data?.toModel()
        
        guard model != nil else {
            let timeline = createTimeline(entry: createTimelineEntry(model: nil, status: .noWeather, config: configuration))
            completion(timeline)
            return
        }
        
        let entry = WeatherTimelineEntry(date: Date(), model: model, configuration: configuration)

        let timeline = createTimeline(entry: entry)
        completion(timeline)
    }
    
    func createTimeline(entry: WeatherTimelineEntry) -> Timeline<WeatherTimelineEntry> {
        return Timeline(entries: [entry], policy: .after (Date().addingTimeInterval(15 * 60)))
    }
    
    func createTimelineEntry(model: WeatherModel?, status: EntryStatus, config: WeatherWidgetConfigurationIntent) -> WeatherTimelineEntry {
        return WeatherTimelineEntry(date: Date(), model: model, configuration: config, status: status)
    }
}

struct WeatherWidgetEntryView : View {
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
            case .systemSmall:
                WeatherWidgetSmall(model: entry.model!)
            case .systemMedium:
                if (!entry.model!.showHourlyForecast) {
                    WeatherWidgetSmallForecast(model: entry.model!)
                } else {
                    WeatherWidgetMedium(model: entry.model!)
                }
            case .systemLarge:
                WeatherWidgetLarge(model: entry.model!)
            case .accessoryRectangular:
                WeatherWidgetRectangular(model: entry.model!)
            case .accessoryInline:
                WeatherWidgetInline(model: entry.model!)
            @unknown default:
                fatalError("Should not happen")
            }
        }
    }
}

struct WeatherWidget: Widget {
    let kind: String = "widget"
    
    func makeWidgetConfiguration() -> some WidgetConfiguration {
        /*
        if #available(iOS 17.0, macOS 14.0, *) {
            return AppIntentConfiguration(kind: kind, intent: WeatherWidgetConfigurationIntent.self, provider: Provider()) { entry in
                WeatherWidgetEntryView(entry: entry)
            }
        } else {*/
            return IntentConfiguration(kind: kind, intent: WeatherWidgetConfigurationIntent.self, provider: Provider()) { entry in
                WeatherWidgetEntryView(entry: entry)
            }
        //}
    }

    var body: some WidgetConfiguration {
        IntentConfiguration(kind: kind, intent: WeatherWidgetConfigurationIntent.self, provider: Provider()) { entry in
            WeatherWidgetEntryView(entry: entry)
        }
        .configurationDisplayName("Weather")
        .description("Check the current weather conditions and forecast for a location")
        .supportedFamilies([
            .systemSmall,
            .systemMedium,
            .systemLarge,
            // Lock Screen Widgets
            .accessoryInline,
            .accessoryRectangular
        ])
    }
}

struct WeatherWidgetPreviews: PreviewProvider {
    static var previews: some View {
        let entry = WeatherTimelineEntry(date: Date(), model: mockWeatherModel(), configuration: WeatherWidgetConfigurationIntent())

        WeatherWidgetEntryView(entry: entry)
            .previewContext(WidgetPreviewContext(family: .systemSmall))
        WeatherWidgetEntryView(entry: entry)
            .previewContext(WidgetPreviewContext(family: .systemMedium))
        WeatherWidgetEntryView(entry: entry)
            .previewContext(WidgetPreviewContext(family: .systemLarge))
        WeatherWidgetEntryView(entry: entry)
            .previewContext(WidgetPreviewContext(family: .accessoryInline))
        WeatherWidgetEntryView(entry: entry)
            .previewContext(WidgetPreviewContext(family: .accessoryRectangular))
    }
}
