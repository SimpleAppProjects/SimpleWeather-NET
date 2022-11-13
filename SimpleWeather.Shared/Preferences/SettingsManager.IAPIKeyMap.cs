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

    public interface IKeyVerifiedMap
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

        private class KeyVerifiedMap : IKeyVerifiedMap
        {
            private readonly SettingsManager settingsMgr;

            internal KeyVerifiedMap(SettingsManager settingsManager) => this.settingsMgr = settingsManager;

            public bool this[string provider]
            {
                get => settingsMgr.IsKeyVerified(provider);
                set => settingsMgr.SetKeyVerified(provider, value);
            }
        }
    }
}
