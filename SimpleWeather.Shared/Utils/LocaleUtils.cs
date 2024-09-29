using System.Globalization;
using System.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Preferences;
#if WINDOWS
using CommunityToolkit.WinUI;
using Windows.System.UserProfile;
#elif __ANDROID__
using AndroidX.Core.App;
#endif
#if !WINDOWS
using Microsoft.Maui.ApplicationModel;
#endif

namespace SimpleWeather.Utils
{
    public static class LocaleUtils
    {
        public const string KEY_LANGUAGE = "key_language";
        private static CultureInfo sLangInfo = null;

        public static void Init() => UpdateAppLocale();

        private static void UpdateAppLocale()
        {
            var localeAction = () =>
            {
                var locale = GetLocale();

                Thread.CurrentThread.CurrentCulture = locale;
                Thread.CurrentThread.CurrentUICulture = locale;
                CultureInfo.CurrentCulture = locale;
                CultureInfo.CurrentUICulture = locale;

                SharedModule.Instance.RequestAction(CommonActions.ACTION_LOCALE_CHANGED);
            };

#if WINDOWS || WINUI
            if (SharedModule.Instance.DispatcherQueue?.HasThreadAccess ?? false)
#else
			if (MainThread.IsMainThread)
#endif
            {
                localeAction();
            }
            else
            {
                // Update current thread and Main Thread
                localeAction();
#if WINDOWS || WINUI
                SharedModule.Instance.DispatcherQueue?.EnqueueAsync(localeAction);
#else
                MainThread.BeginInvokeOnMainThread(localeAction);
#endif
            }
        }

        public static string GetLocaleCode()
        {
            var settingsMgr = Ioc.Default.GetService<SettingsManager>();
            return settingsMgr.GetValue<string>(KEY_LANGUAGE, "");
        }

        public static void SetLocaleCode(string localeCode)
        {
            var settingsMgr = Ioc.Default.GetService<SettingsManager>();
            settingsMgr.SetValue<string>(KEY_LANGUAGE, localeCode);
            UpdateLocale(localeCode);
            UpdateAppLocale();

            AnalyticsLogger.SetUserProperty(AnalyticsProps.USER_LOCALE, GetLocale()?.Name);
        }

        public static CultureInfo GetLocale()
        {
            if (sLangInfo == null)
                UpdateLocale(GetLocaleCode());

            return sLangInfo;
        }

        private static void UpdateLocale(string localeCode)
        {
            sLangInfo = GetLocaleForTag(localeCode);
        }

        public static string GetLocaleDisplayName()
        {
            return GetLocale().GetNativeDisplayName();
        }

        public static CultureInfo GetLocaleForTag(string localeCode)
        {
            if (string.IsNullOrWhiteSpace(localeCode))
            {
                return GetDefault();
            }
            else
            {
                var culture = new CultureInfo(localeCode);
                return culture;
            }
        }

        public static CultureInfo GetDefault()
        {
#if WINDOWS
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);
            return culture;
#elif __IOS__
			var userlang = Foundation.NSLocale.PreferredLanguages[0];
			var culture = new CultureInfo(userlang);
			return culture;
#elif __ANDROID__
			Java.Util.Locale locale = null;
			try
			{
				locale = LocaleManagerCompat.GetSystemLocales(Platform.AppContext).Get(0) ?? Java.Util.Locale.Default;
			}
			catch
			{
				locale = Java.Util.Locale.Default;
            }

			var culture = new CultureInfo(locale.ToLanguageTag());
			return culture;
#else
			return CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture;
#endif
        }
    }
}

