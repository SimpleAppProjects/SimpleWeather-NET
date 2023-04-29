namespace SimpleWeather.Preferences
{
    public interface IAPIKeyMap
    {
        string this[string provider]
        {
            get;
            set;
        }
    }

    public interface IProviderMap
    {
        bool this[string provider]
        {
            get;
            set;
        }
    }

    public partial class SettingsManager
    {
        private class APIKeyMap : IAPIKeyMap
        {
            private readonly SettingsManager settingsMgr;

            internal APIKeyMap(SettingsManager settingsManager) => this.settingsMgr = settingsManager;

            public string this[string provider]
            {
                get => settingsMgr.GetAPIKey(provider);
                set => settingsMgr.SetAPIKey(provider, value);
            }
        }

        private class KeyVerifiedMap : IProviderMap
        {
            private readonly SettingsManager settingsMgr;

            internal KeyVerifiedMap(SettingsManager settingsManager) => this.settingsMgr = settingsManager;

            public bool this[string provider]
            {
                get => settingsMgr.IsKeyVerified(provider);
                set => settingsMgr.SetKeyVerified(provider, value);
            }
        }

        private class UsePersonalKeyMap : IProviderMap
        {
            private readonly SettingsManager settingsMgr;

            internal UsePersonalKeyMap(SettingsManager settingsManager) => this.settingsMgr = settingsManager;

            public bool this[string provider]
            {
                get => settingsMgr.IsUsePersonalKey(provider);
                set => settingsMgr.SetUsePersonalKey(provider, value);
            }
        }
    }
}
