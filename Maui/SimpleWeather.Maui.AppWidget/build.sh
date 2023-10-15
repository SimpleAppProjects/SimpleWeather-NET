#!/bin/bash

xcodebuild clean
rm -rf DerivedData

xcodebuild build -configuration Debug -scheme widgetExtension -sdk iphonesimulator
xcodebuild build -configuration Debug -scheme widgetExtension -sdk iphoneos
xcodebuild build -configuration Debug -scheme widgetExtension -sdk macosx -destination 'platform=macOS,variant=Mac Catalyst' SUPPORTS_MACCATALYST=YES

xcodebuild build -configuration Release -scheme widgetExtension -sdk iphonesimulator
xcodebuild build -configuration Release -scheme widgetExtension -sdk iphoneos
xcodebuild build -configuration Release -scheme widgetExtension -sdk macosx -destination 'platform=macOS,variant=Mac Catalyst' SUPPORTS_MACCATALYST=YES
