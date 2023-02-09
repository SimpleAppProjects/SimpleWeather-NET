using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SimpleWeather.Maui.Preferences
{
    public class SettingsChangedMessage : ValueChangedMessage<SettingsChanged>
    {
        public SettingsChangedMessage(SettingsChanged value) : base(value)
        {
        }
    }

    public class SettingsChangedMessage<T> : ValueChangedMessage<SettingsChanged<T>>
    {
        public SettingsChangedMessage(SettingsChanged<T> value) : base(value)
        {
        }
    }

    public sealed record SettingsChanged(string PreferenceKey, object NewValue);
    public sealed record SettingsChanged<T>(string PreferenceKey, T NewValue);
}

