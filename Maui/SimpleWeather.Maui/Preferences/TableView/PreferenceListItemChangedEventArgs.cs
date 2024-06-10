namespace SimpleWeather.Maui.Preferences
{
    public class PreferenceListItemChangedEventArgs : EventArgs
    {
        public PreferenceListItem NewValue { get; internal set; }
    }
}

