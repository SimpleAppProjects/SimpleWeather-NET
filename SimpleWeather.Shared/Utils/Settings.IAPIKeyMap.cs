using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
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

    public static partial class Settings
    {
        private class APIKeyMap : IAPIKeyMap
        {
            public string this[string provider]
            {
                get => GetAPIKEY(provider);
                set
                {
                    SetAPIKEY(provider, value);
                    OnSettingsChanged?.Invoke(new SettingsChangedEventArgs {
                        Key = $"{KEY_APIKEY_PREFIX}_{provider}", NewValue = value
                    });
                }
            }
        }

        private class KeyVerifiedMap : IKeyVerifiedMap
        {
            public bool this[string provider]
            {
                get => IsKeyVerified(provider);
                set
                {
                    SetKeyVerified(provider, value);
                    OnSettingsChanged?.Invoke(new SettingsChangedEventArgs
                    {
                        Key = $"{KEY_APIKEY_VERIFIED}_{provider}",
                        NewValue = value
                    });
                }
            }
        }
    }
}
