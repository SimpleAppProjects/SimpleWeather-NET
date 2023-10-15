//
//  widgetBundle.swift
//  widget
//
//  Created by Dave Antoine on 10/2/23.
//

import WidgetKit
import SwiftUI

@main
struct WeatherWidgetBundle: WidgetBundle {
    @WidgetBundleBuilder
    var body: some Widget {
        WeatherWidget()
    }
}
