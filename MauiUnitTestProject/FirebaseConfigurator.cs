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

namespace UnitTestProject;

public static class FirebaseConfigurator
{
    public static void Initialize()
    {
        FirebaseApp.Configure();

#if IOS
        FirebaseCrashlytics.SharedInstance?.Apply(it => { it.SetCrashlyticsCollectionEnabled(false); });
#endif

#if __IOS__
        try
        {
            var remoteConfigDefaults = ConfigiOS.ResourceManager
                .GetResourceSet(CultureInfo.CurrentUICulture, true, true)
                ?.Cast<DictionaryEntry>()
                ?.ToDictionary(r => r.Key, r => r.Value);

            if (remoteConfigDefaults != null)
            {
                FirebaseRemoteConfig.SharedInstance?.SetDefaults(remoteConfigDefaults);
            }
        }
        catch
        {
        }
#endif
    }
}
#endif