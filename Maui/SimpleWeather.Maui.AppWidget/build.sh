#!/bin/bash

xcodebuild clean
rm -rf DerivedData

xcodebuild build -configuration Debug -target widgetExtension -sdk iphonesimulator
xcodebuild build -configuration Debug -target widgetExtension -sdk iphoneos
xcodebuild build -configuration Debug -target widgetExtension -sdk macosx -destination 'platform=macOS,variant=Mac Catalyst' SUPPORTS_MACCATALYST=YES
xcodebuild build -configuration Debug -target widgetintentExt -sdk iphonesimulator
xcodebuild build -configuration Debug -target widgetintentExt -sdk iphoneos
xcodebuild build -configuration Debug -target widgetintentExt -sdk macosx -destination 'platform=macOS,variant=Mac Catalyst' SUPPORTS_MACCATALYST=YES

xcodebuild build -configuration Release -target widgetExtension -sdk iphonesimulator
xcodebuild build -configuration Release -target widgetExtension -sdk iphoneos
xcodebuild build -configuration Release -target widgetExtension -sdk macosx -destination 'platform=macOS,variant=Mac Catalyst' SUPPORTS_MACCATALYST=YES
xcodebuild build -configuration Release -target widgetintentExt -sdk iphonesimulator
xcodebuild build -configuration Release -target widgetintentExt -sdk iphoneos
xcodebuild build -configuration Release -target widgetintentExt -sdk macosx -destination 'platform=macOS,variant=Mac Catalyst' SUPPORTS_MACCATALYST=YES
