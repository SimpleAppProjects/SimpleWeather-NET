#!/bin/bash

config=$1
cleanWidgets=$2
publish=$3

KeyCheck/bin/Release/net8.0/publish/KeyCheck SimpleWeather.Shared/Keys
KeyCheck/bin/Release/net8.0/publish/KeyCheck SimpleWeather.Weather-API/Keys

cd Maui/SimpleWeather.Maui.AppWidget

if [ "$cleanWidgets" = "clean"  ]; then
    xcodebuild clean; rm -rf DerivedData
fi

scheme="SimpleWeather-Extensions-Dev"

if [ "$publish" = "publish" ]; then
    scheme="SimpleWeather-Extensions"
fi

xcodebuild build -configuration $config -scheme "$scheme" -sdk iphonesimulator
xcodebuild build -configuration $config -scheme "$scheme" -sdk iphoneos
xcodebuild build -configuration $config -scheme "$scheme" -sdk macosx -destination 'platform=macOS,variant=Mac Catalyst' SUPPORTS_MACCATALYST=YES SKIP_INSTALL=NO

cd ../SimpleWeather.Maui

../../EditManifest/bin/Release/net8.0/publish/EditManifest Platforms/iOS/Info.plist "$config"
../../EditManifest/bin/Release/net8.0/publish/EditManifest Platforms/iOS/Entitlements.plist "$config"
../../EditManifest/bin/Release/net8.0/publish/EditManifest Platforms/MacCatalyst/Info.plist "$config"
../../EditManifest/bin/Release/net8.0/publish/EditManifest Platforms/MacCatalyst/Entitlements.plist "$config"
