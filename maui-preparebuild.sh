#!/bin/bash

config=$1

KeyCheck/bin/Release/net7.0/publish/KeyCheck SimpleWeather.Shared/Keys
KeyCheck/bin/Release/net7.0/publish/KeyCheck SimpleWeather.Weather-API/Keys

cd Maui/SimpleWeather.Maui.AppWidget

xcodebuild build -configuration $config -scheme widgetExtension -sdk iphonesimulator
xcodebuild build -configuration $config -scheme widgetExtension -sdk iphoneos
xcodebuild build -configuration $config -scheme widgetExtension -sdk macosx -destination 'platform=macOS,variant=Mac Catalyst' SUPPORTS_MACCATALYST=YES SKIP_INSTALL=NO

cd ../SimpleWeather.Maui

../../EditManifest/bin/Release/net7.0/publish/EditManifest Platforms/iOS/Info.plist "$config"
../../EditManifest/bin/Release/net7.0/publish/EditManifest Platforms/iOS/Entitlements.plist "$config"
../../EditManifest/bin/Release/net7.0/publish/EditManifest Platforms/MacCatalyst/Info.plist "$config"
../../EditManifest/bin/Release/net7.0/publish/EditManifest Platforms/MacCatalyst/Entitlements.plist "$config"
