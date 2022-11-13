using SimpleWeather.Preferences;
using SimpleWeather.RemoteConfig;
using System;

namespace SimpleWeather.DI
{
    public static class Utils
    {
        private static readonly Lazy<SettingsManager> _settingsMgr = new(() =>
        {
            return new SettingsManager();
        });
        public static SettingsManager SettingsManager => _settingsMgr.Value;

        private static readonly Lazy<IRemoteConfigService> _remoteConfigService = new(() =>
        {
            return new RemoteConfigServiceImpl();
        });
        public static IRemoteConfigService RemoteConfigService => _remoteConfigService.Value;
    }
}
