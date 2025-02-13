#if __IOS__

using System.Collections;
using System.Globalization;
using Foundation;
using SimpleWeather.Resources.Strings;
using SimpleWeather.Utils;
using FirebaseApp = Firebase.Core.App;
using FirebaseAnalytics = Firebase.Analytics.Analytics;
#if IOS
using FirebaseCrashlytics = Firebase.Crashlytics.Crashlytics;
#endif
using FirebaseRemoteConfig = Firebase.RemoteConfig.RemoteConfig;
using FirebaseMessaging = Firebase.CloudMessaging.Messaging;

namespace SimpleWeather.Maui;

public static class FirebaseConfigurator
{
    private static bool sMessagingRegistered = false;
#if DEBUG
    private static bool sDbgMessagingRegistered = false;
#endif

    public static void Initialize()
    {
        FirebaseApp.Configure();

        if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
        {
            FirebaseAnalytics.SetUserProperty(name: AnalyticsProps.DEVICE_TYPE, value: "desktop");
        }
        else if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
        {
            FirebaseAnalytics.SetUserProperty(name: AnalyticsProps.DEVICE_TYPE, value: "tablet");
        }
        else if (DeviceInfo.Idiom == DeviceIdiom.Phone)
        {
            FirebaseAnalytics.SetUserProperty(name: AnalyticsProps.DEVICE_TYPE, value: "phone");
        }
        
        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            FirebaseAnalytics.SetUserProperty(name: AnalyticsProps.PLATFORM, value: "iOS");
        }
        else if (DeviceInfo.Platform == DevicePlatform.MacCatalyst)
        {
            FirebaseAnalytics.SetUserProperty(name: AnalyticsProps.PLATFORM, value: "MacCatalyst");
        }
        else if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            FirebaseAnalytics.SetUserProperty(name: AnalyticsProps.PLATFORM, value: "Android");
        }
        else if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            FirebaseAnalytics.SetUserProperty(name: AnalyticsProps.PLATFORM, value: "Windows");
        }

#if IOS
        FirebaseCrashlytics.SharedInstance?.Apply(it =>
        {
            it.SetCrashlyticsCollectionEnabled(true);
            it.SendUnsentReports();
        });
#endif

#if __IOS__
        try
        {
            var remoteConfigDefaults = ConfigiOS.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true)
                ?.Cast<DictionaryEntry>()
                ?.ToDictionary(r => r.Key, r => r.Value);

            if (remoteConfigDefaults != null)
            {
                FirebaseRemoteConfig.SharedInstance?.SetDefaults(remoteConfigDefaults);
            }
        }
        catch { }
#endif

#if IOS && RELEASE
        Logger.RegisterLogger(new CrashlyticsLoggingTree());
#endif

        // SubscribeToTopics(); -- Do this after apns token is received
    }

    public static void SubscribeToTopics()
    {
        // Received Firebase messages
        if (!sMessagingRegistered)
        {
            FirebaseMessaging.SharedInstance?.Subscribe("all", (error) =>
            {
                if (error != null)
                {
                    Logger.WriteLine(LoggerLevel.Warn, new NSErrorException(error));
                }
                else
                {
                    sMessagingRegistered = true;
                }
            });
        }

#if DEBUG
        if (!sDbgMessagingRegistered)
        {
            FirebaseMessaging.SharedInstance?.Subscribe("debug_all", (error) =>
            {
                if (error != null)
                {
                    Logger.WriteLine(LoggerLevel.Warn, new NSErrorException(error));
                }
                else
                {
                    sDbgMessagingRegistered = true;
                }
            });
        }
#endif
    }
}
#endif