//
//  WeatherWidgetSmall.swift
//  widgetExtension
//
//  Created by Dave Antoine on 10/2/23.
//

import SwiftUI
import WidgetKit

// Text Sizes
#if targetEnvironment(macCatalyst) || os(macOS)
private let locationFont: Font = .footnote
private let textFont: Font = .caption2
private let forecastFont: Font = .caption
private let tempFont: Font = .largeTitle
#else
private let locationFont: Font = .footnote
private let textFont: Font = .caption2
private let forecastFont: Font = .caption
private let tempFont: Font = .largeTitle
#endif

// Icon Sizes
private let largeIconSize: Font = .system(size: 24)
private let mediumIconSize: Font = .system(size: 20)
private let smallIconSize: Font = .system(size: 16)
private let locationIconSize: Font = .system(size: 12)

struct WeatherWidgetSmall: View {
    let model: WeatherModel
    
    var body: some View {
        ZStack(alignment: .center) {
            VStack(alignment: .leading) {
                HStack {
                    Text(model.current.location)
                        .lineLimit(1)
                        .truncationMode(.tail)
                        .fontWeight(.medium)
                        .font(locationFont)
                    if (model.current.isGPS) {
                        Image(systemName: "location.fill")
                            .font(locationIconSize)
                    }
                }
                Text(model.current.temp)
                    .font(.largeTitle)
                Spacer()
                    .frame(maxHeight: .infinity, alignment: .top)
                Image(systemName: iconToSFSymbol(icon: model.current.weatherIcon))
                    .symbolRenderingMode(.multicolor)
                    .font(mediumIconSize)
                Text(model.current.condition)
                    .lineLimit(2)
                    .truncationMode(.tail)
                    .font(textFont)
                    .fontWeight(.medium)
            }
            .frame(maxWidth: /*@START_MENU_TOKEN@*/.infinity/*@END_MENU_TOKEN@*/, alignment: .leading)
            .widgetContentPaddingCompat()
        }.foregroundColor(
            (model.current.backgroundColor != nil || model.current.backgroundCode != nil) ? Color.white : nil
        ).widgetBackground(view: WeatherBackgroundOverlay(current: model.current))
    }
}

struct WeatherWidgetSmallForecast: View {
    let model: WeatherModel
    
    var body: some View {
        ZStack(alignment: .center) {
            VStack(alignment: .leading) {
                HStack(alignment: .center) {
                    VStack(alignment: .leading) {
                        HStack {
                            Text(model.current.location)
                                .lineLimit(1)
                                .truncationMode(.tail)
                                .fontWeight(.medium)
                                .font(locationFont)
                            if (model.current.isGPS) {
                                Image(systemName: "location.fill")
                                    .font(locationIconSize)
                            }
                        }
                        Text(model.current.temp)
                            .font(.largeTitle)
                        Spacer()
                            .frame(maxHeight: .infinity, alignment: .top)
                        Image(systemName: iconToSFSymbol(icon: model.current.weatherIcon))
                            .symbolRenderingMode(.multicolor)
                            .font(largeIconSize)
                        Text(model.current.condition)
                            .lineLimit(2)
                            .truncationMode(.tail)
                            .font(textFont)
                            .fontWeight(.medium)
                    }.frame(maxWidth: .infinity, alignment: .leading)
                    VStack(alignment: .trailing, spacing: 8) {
                        ForEach(0 ..< min(model.forecasts.count, 5)) { i in
                            WeatherWidgetDailyForecastSmall(model: model.forecasts[i])
                        }
                    }.frame(maxWidth: .infinity, alignment: .leading)
                }
            }
            .widgetContentPaddingCompat()
        }.foregroundColor(
            (model.current.backgroundColor != nil || model.current.backgroundCode != nil) ? Color.white : nil
        ).widgetBackground(view: WeatherBackgroundOverlay(current: model.current))
    }
}

struct WeatherWidgetMedium: View {
    let model: WeatherModel
    
    var body: some View {
        ZStack(alignment: .center) {
            VStack(alignment: .leading, spacing: 0) {
                HStack(alignment: .center) {
                    // Location & Temp
                    VStack(alignment: .leading) {
                        HStack {
                            Text(model.current.location)
                                .lineLimit(1)
                                .truncationMode(.tail)
                                .fontWeight(.medium)
                                .font(locationFont)
                            if (model.current.isGPS) {
                                Image(systemName: "location.fill")
                                    .font(locationIconSize)
                            }
                        }
                        Text(model.current.temp)
                            .font(.largeTitle)
                    }.frame(maxWidth: .infinity, alignment: .leading)
                    // Current Condition
                    VStack(alignment: .trailing, spacing: 0) {
                        Image(systemName: iconToSFSymbol(icon: model.current.weatherIcon))
                            .symbolRenderingMode(.multicolor)
                            .font(.system(size: 16))
                        Text(model.current.condition)
                            .lineLimit(2)
                            .truncationMode(.tail)
                            .font(textFont)
                            .fontWeight(.medium)
                        if (model.current.showHiLo) {
                            Text(
                                (model.current.hi ?? WeatherIcons.PLACEHOLDER) +
                                " / " +
                                (model.current.lo ?? WeatherIcons.PLACEHOLDER)
                            )
                                .lineLimit(1)
                                .truncationMode(.tail)
                                .font(textFont)
                                .fontWeight(.medium)
                        }
                    }
                }
                if (model.showHourlyForecast) {
                    HStack(alignment: .center) {
                        ForEach(0 ..< min(model.hourlyForecasts.count, 6)) { i in
                            WeatherWidgetHourlyForecast(model: model.hourlyForecasts[i], imageFont: smallIconSize)
                                .frame(maxWidth: .infinity, alignment: .center)
                        }
                    }
                } else if (model.showForecast) {
                    HStack(alignment: .center) {
                        ForEach(0 ..< min(model.forecasts.count, 5)) { i in
                            WeatherWidgetDailyForecastMedium(model: model.forecasts[i])
                                .frame(maxWidth: .infinity, alignment: .center)
                        }
                    }
                }
            }
            .widgetContentPaddingCompat()
        }.foregroundColor(
            (model.current.backgroundColor != nil || model.current.backgroundCode != nil) ? Color.white : nil
        ).widgetBackground(view: WeatherBackgroundOverlay(current: model.current))
    }
}

struct WeatherWidgetLarge: View {
    let model: WeatherModel
    
    var body: some View {
        ZStack(alignment: .center) {
            VStack(alignment: .center) {
                HStack(alignment: .center) {
                    // Location & Temp
                    VStack(alignment: .leading) {
                        HStack {
                            Text(model.current.location)
                                .lineLimit(1)
                                .truncationMode(.tail)
                                .fontWeight(.medium)
                                .font(locationFont)
                            if (model.current.isGPS) {
                                Image(systemName: "location.fill")
                                    .font(locationIconSize)
                            }
                        }
                        Text(model.current.temp)
                            .font(.largeTitle)
                        if (model.current.showHiLo) {
                            Text(
                                (model.current.hi ?? WeatherIcons.PLACEHOLDER) +
                                " / " +
                                (model.current.lo ?? WeatherIcons.PLACEHOLDER)
                            )
                                .lineLimit(1)
                                .truncationMode(.tail)
                                .font(textFont)
                                .fontWeight(.medium)
                        }
                    }.frame(maxWidth: .infinity, alignment: .leading)
                    // Current Condition
                    VStack(alignment: .trailing, spacing: 0) {
                        Image(systemName: iconToSFSymbol(icon: model.current.weatherIcon))
                            .symbolRenderingMode(.multicolor)
                            .font(largeIconSize)
                        Text(model.current.condition)
                            .lineLimit(1)
                            .truncationMode(.tail)
                            .font(textFont)
                            .fontWeight(.medium)
                        if (model.current.chance != nil) {
                            Spacer().frame(height: 4)
                            HStack(alignment: .center) {
                                Image(systemName: "umbrella.percent.fill")
                                    .symbolRenderingMode(.multicolor)
                                    .font(smallIconSize)
                                    .frame(width: 8, height: 8)
                                Text(model.current.chance!)
                                    .lineLimit(1)
                                    .font(textFont)
                                    .fontWeight(.medium)
                            }
                        }
                    }
                }
                if (model.showHourlyForecast) {
                    HorizontalDivider()
                    HStack(alignment: .center) {
                        ForEach(0 ..< min(model.hourlyForecasts.count, 6)) { i in
                            WeatherWidgetHourlyForecast(model: model.hourlyForecasts[i])
                                .frame(maxWidth: .infinity, alignment: .center)
                        }
                    }
                }
                if (model.showForecast) {
                    HorizontalDivider()
                    VStack {
                        ForEach(0 ..< min(model.forecasts.count, 5)) { i in
                            WeatherWidgetDailyForecastLarge(model: model.forecasts[i])
                                .padding(EdgeInsets(top: 0, leading: 8, bottom: 0, trailing: 8))
                                .frame(maxWidth: .infinity, minHeight: 12, alignment: .leading)
                        }
                    }
                }
            }
            .widgetContentPaddingCompat()
        }.foregroundColor(
            (model.current.backgroundColor != nil || model.current.backgroundCode != nil) ? Color.white : nil
        ).widgetBackground(view: WeatherBackgroundOverlay(current: model.current))
    }
}

struct WeatherWidgetHourlyForecast: View {
    let model: HourlyForecast
    var imageFont: Font = mediumIconSize
    
    var body: some View {
        VStack(alignment: .center, spacing: 8) {
            Text(model.date)
                .lineLimit(1)
                .font(textFont)
                .fontWeight(.medium)
                .opacity(0.8)
            Image(systemName: iconToSFSymbol(icon: model.weatherIcon))
                .symbolRenderingMode(.multicolor)
                .font(imageFont)
            Text(model.hi)
                .lineLimit(1)
                .font(forecastFont)
                .fontWeight(.medium)
        }
    }
}

struct WeatherWidgetDailyForecastMedium: View {
    let model: Forecast
    
    var body: some View {
        VStack(alignment: .center, spacing: 0) {
            Text(model.date)
                .lineLimit(1)
                .font(textFont)
                .fontWeight(.medium)
                .opacity(0.8)
            Spacer().frame(height: 4)
            Image(systemName: iconToSFSymbol(icon: model.weatherIcon))
                .symbolRenderingMode(.multicolor)
                .font(mediumIconSize)
            Spacer().frame(height: 4)
            HStack(spacing: 0) {
                Text(model.hi)
                    .lineLimit(1)
                    .font(textFont)
                    .fontWeight(.medium)
                Text("|")
                    .lineLimit(1)
                    .font(textFont)
                    .fontWeight(.medium)
                    .padding(EdgeInsets(top: 0, leading: 2, bottom: 0, trailing: 2))
                Text(model.lo)
                    .lineLimit(1)
                    .font(textFont)
                    .fontWeight(.medium)
                    .opacity(0.8)
            }
        }
    }
}

struct WeatherWidgetDailyForecastSmall: View {
    let model: Forecast
    
    var body: some View {
        HStack(alignment: .center, spacing: 0) {
            Text(model.date)
                .lineLimit(1)
                .font(textFont)
                .fontWeight(.medium)
                .frame(minWidth: 32, alignment: .leading)
            Spacer().frame(width: 4)
            Image(systemName: iconToSFSymbol(icon: model.weatherIcon))
                .symbolRenderingMode(.multicolor)
                .font(smallIconSize)
            Spacer().frame(width: 8)
            if (model.chance != nil && (Int(model.chance?.replacing("%", with: "") ?? "") ?? 0) >= 30) {
                Text(model.chance!)
                    .lineLimit(1)
                    .font(textFont)
                    .fontWeight(.medium)
                    .foregroundColor(Color(hexString: "#599ef3"))
                    .brightness(0.25)
                    .frame(maxWidth: .infinity, alignment: .leading)
            } else {
                Spacer()
                    .frame(maxWidth: .infinity)
            }
            Text(model.hi)
                .lineLimit(1)
                .font(textFont)
                .fontWeight(.medium)
            Text("|")
                .lineLimit(1)
                .font(textFont)
                .padding(EdgeInsets(top: 0, leading: 2, bottom: 0, trailing: 2))
            Text(model.lo)
                .lineLimit(1)
                .font(textFont)
                .fontWeight(.medium)
                .opacity(0.8)
        }
    }
}

struct WeatherWidgetDailyForecastLarge: View {
    let model: Forecast
    
    var body: some View {
        HStack(alignment: .center, spacing: 0) {
            Text(model.date)
                .lineLimit(1)
                .font(forecastFont)
                .fontWeight(.medium)
                .frame(minWidth: 36, alignment: .leading)
            Spacer()
                .frame(width: 60)
            Image(systemName: iconToSFSymbol(icon: model.weatherIcon))
                .symbolRenderingMode(.multicolor)
                .font(mediumIconSize)
            if (model.chance != nil && (Int(model.chance?.replacing("%", with: "") ?? "") ?? 0) >= 30) {
                HStack(alignment: .center, spacing: 0) {
                    Image(systemName: "umbrella.percent.fill")
                        .symbolRenderingMode(.multicolor)
                        .font(smallIconSize)
                        .padding(EdgeInsets(top: 0, leading: 0, bottom: 0, trailing: 4))
                    Text(model.chance!)
                        .lineLimit(1)
                        .font(forecastFont)
                        .fontWeight(.medium)
                        .frame(minWidth: 36, alignment: .trailing)
                }
                .frame(maxWidth: .infinity)
            } else {
                Spacer()
                    .frame(maxWidth: .infinity)
            }
            Text(model.hi)
                .lineLimit(1)
                .font(forecastFont)
                .fontWeight(.medium)
                .frame(minWidth: 26, alignment: .trailing)
            Text("|")
                .lineLimit(1)
                .font(forecastFont)
                .padding(EdgeInsets(top: 0, leading: 4, bottom: 0, trailing: 4))
            Text(model.lo)
                .lineLimit(1)
                .font(forecastFont)
                .fontWeight(.medium)
                .opacity(0.8)
                .frame(minWidth: 26, alignment: .leading)
        }
    }
}

struct WeatherWidgetInline: View {
    let model: WeatherModel
    
    var body: some View {
        HStack(alignment: /*@START_MENU_TOKEN@*/.center/*@END_MENU_TOKEN@*/, spacing: 4) {
            Image(systemName: iconToSFSymbol(icon: model.current.weatherIcon))
                .symbolRenderingMode(.multicolor)
                .font(smallIconSize)
            Text(model.current.temp)
        }
    }
}

struct WeatherLocationWidgetInline: View {
    let model: WeatherModel
    
    var body: some View {
        HStack(alignment: /*@START_MENU_TOKEN@*/.center/*@END_MENU_TOKEN@*/, spacing: 4) {
            Image(systemName: iconToSFSymbol(icon: model.current.weatherIcon))
                .symbolRenderingMode(.multicolor)
                .font(smallIconSize)
            Text(model.current.location)
        }
    }
}

struct WeatherPrecipitationWidgetInline: View {
    let model: WeatherModel
    
    var body: some View {
        HStack(alignment: /*@START_MENU_TOKEN@*/.center/*@END_MENU_TOKEN@*/, spacing: 4) {
            Image(systemName: iconToSFSymbol(icon: WeatherIcons.UMBRELLA))
                .symbolRenderingMode(.multicolor)
                .font(smallIconSize)
            Text(model.current.chance ?? WeatherIcons.EM_DASH)
        }
    }
}

struct WeatherPrecipitationWidgetCircular: View {
    let model: WeatherModel
    
    var body: some View {
        let value = if (model.current.chance != nil) {
            Double(model.current.chance!.trimmingCharacters(in: CharacterSet(charactersIn: "0123456789.").inverted))
        } else {
            0.0
        }

        ZStack {
            Gauge(
                value: value ?? 0.0,
                in: 0...100,
                label: {
                    Image(systemName: iconToSFSymbol(icon: WeatherIcons.UMBRELLA))
                        .symbolRenderingMode(.multicolor)
                }
            ) {
                Text(model.current.chance ?? WeatherIcons.EM_DASH)
                    .font(smallIconSize)
            }
            .gaugeStyle(.accessoryCircular)
        }.widgetBackground(view: Rectangle().foregroundColor(.clear))
    }
}

struct WeatherWidgetRectangular: View {
    let model: WeatherModel
    
    var body: some View {
        ZStack {
            VStack(alignment: .leading, spacing: 4) {
                Text(model.current.location)
                    .lineLimit(1)
                    .fontWeight(.bold)
                Text(model.current.temp + " " + model.current.condition)
                    .lineLimit(1)
                if (model.current.showHiLo) {
                    Text(
                        (model.current.hi ?? WeatherIcons.PLACEHOLDER) +
                        " / " +
                        (model.current.lo ?? WeatherIcons.PLACEHOLDER)
                    )
                        .lineLimit(1)
                        .opacity(0.75)
                }
            }
        }.widgetBackground(view: Rectangle().foregroundColor(.clear))
    }
}

struct HorizontalDivider : View {
    let color: Color
    let height: CGFloat
    
    init(color: Color = .white, height: CGFloat = 1) {
        self.color = color
        self.height = height
    }
    
    var body: some View {
        color
            .opacity(0.5)
            .frame(height: height)
    }
}

struct WeatherBackgroundOverlay : View {
    let current: Current
    
    var body: some View {
        if (current.backgroundCode != nil) {
            backgroundCodeToGradientOverlay(backgroundCode: current.backgroundCode!)
        } else if (current.backgroundColor != nil) {
            Rectangle()
                .foregroundColor(Color(hexString: current.backgroundColor!))
                .brightness(-0.05)
        } else {
            Rectangle()
        }
    }
}

struct GradientOverlay : View {
    var stops: [Gradient.Stop] = [
        .init(color: .black.opacity(0.25), location: 0.25),
        .init(color: .black.opacity(0.1), location: 0.5),
        .init(color: .black.opacity(0.25), location: 0.75)
    ]
    var startPoint: UnitPoint = .init(x: 0.5, y: 0)
    var endPoint: UnitPoint = .init(x: 0.5, y: 1)
    
    var body: some View {
        let gradient: Gradient = Gradient(stops: stops)

        Rectangle()
            .foregroundStyle(
                LinearGradient(gradient: gradient, startPoint: startPoint, endPoint: endPoint)
            )
    }
}

func backgroundCodeToGradientOverlay(backgroundCode: String) -> GradientOverlay {
    var stops: [Gradient.Stop]

    switch backgroundCode {
    case WeatherBackground.DUST:
        stops = [
            .init(color: Color(hexString: "#916a3d"), location: 0),
            .init(color: Color(hexString: "#7f6548"), location: 0.5),
            .init(color: Color(hexString: "#553f2e"), location: 1)
        ]
        break
    case WeatherBackground.SNOW,
        WeatherBackground.SNOW_WINDY:
        stops = [
            .init(color: Color(hexString: "#ffb3b3b3"), location: 0),
            .init(color: Color(hexString: "#ff353e4a"), location: 0.5),
            .init(color: Color(hexString: "#ff47575e"), location: 1)
        ]
        break
    case WeatherBackground.RAIN:
        stops = [
            .init(color: Color(hexString: "#475374"), location: 0),
            .init(color: Color(hexString: "#416e94"), location: 1)
        ]
        break
    case WeatherBackground.RAIN_NIGHT:
        stops = [
            .init(color: Color(hexString: "#2e4343"), location: 0),
            .init(color: Color(hexString: "#181010"), location: 1)
        ]
        break
    case WeatherBackground.TSTORMS_DAY,
        WeatherBackground.TSTORMS_NIGHT:
        stops = [
            .init(color: Color(hexString: "#301934"), location: 0),
            .init(color: Color(hexString: "#ff182830"), location: 0.5)
        ]
        break
    case WeatherBackground.STORMS:
        stops = [
            .init(color: Color(hexString: "#ff232c2f"), location: 0),
            .init(color: Color(hexString: "#ff182830"), location: 1)
        ]
        break
    case WeatherBackground.FOG:
        stops = [
            .init(color: Color(hexString: "#575b76"), location: 0),
            .init(color: Color(hexString: "#606680"), location: 0.5),
            .init(color: Color(hexString: "#575b76"), location: 1)
        ]
        break
    case WeatherBackground.PARTLYCLOUDY_DAY:
        stops = [
            .init(color: Color(hexString: "#6699ff"), location: 0),
            .init(color: Color(hexString: "#007bff"), location: 0.5),
            .init(color: Color(hexString: "#0044cc"), location: 1)
        ]
        break
    case WeatherBackground.MOSTLYCLOUDY_DAY:
        stops = [
            .init(color: Color(hexString: "#6a85b1"), location: 0),
            .init(color: Color(hexString: "#97b1d3"), location: 0.5),
            .init(color: Color(hexString: "#6a85b1"), location: 1)
        ]
        break
    case WeatherBackground.PARTLYCLOUDY_NIGHT,
        WeatherBackground.MOSTLYCLOUDY_NIGHT:
        stops = [
            .init(color: Color(hexString: "#2b3762"), location: 0),
            .init(color: Color(hexString: "#2b3762"), location: 0.5),
            .init(color: Color(hexString: "#141e30"), location: 1)
        ]
        break
    case WeatherBackground.NIGHT:
        stops = [
            .init(color: Color(hexString: "#141e30"), location: 0),
            .init(color: Color(hexString: "#2b3762"), location: 1)
        ]
        break
    case WeatherBackground.DAY:
        stops = [
            .init(color: Color(hexString: "#007bff"), location: 0),
            .init(color: Color(hexString: "#0044cc"), location: 1)
        ]
        break
    default:
        stops = [.init(color: .accentColor, location: 0)]
        break
    }
    
    return GradientOverlay(stops: stops)
}

extension View {
    func widgetBackground(view: some View) -> some View {
        if #available(iOSApplicationExtension 17.0, *) {
            return containerBackground(for: .widget) { view }
        } else {
            return self.background { view }
        }
    }
    
    func widgetContentPaddingCompat() -> some View {
        if #available(iOSApplicationExtension 17.0, *) {
            return self
        } else {
            return self.frame(maxWidth: /*@START_MENU_TOKEN@*/.infinity/*@END_MENU_TOKEN@*/, maxHeight: .infinity).padding(16)
        }
    }
}
