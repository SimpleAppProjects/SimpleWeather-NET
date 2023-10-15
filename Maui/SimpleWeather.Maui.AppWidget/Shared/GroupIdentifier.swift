//
//  GroupIdentifier.swift
//  SimpleWeather
//
//  Created by Dave Antoine on 10/14/23.
//

import Foundation

func getGroupIdentifier() -> String {
#if DEBUG
        return "group.com.thewizrd.simpleweather.debug"
#else
        return "group.com.thewizrd.simpleweather"
#endif
}
