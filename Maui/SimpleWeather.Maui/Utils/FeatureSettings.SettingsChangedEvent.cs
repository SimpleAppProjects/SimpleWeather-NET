namespace SimpleWeather.Maui.Utils
{
    public sealed class FeatureSettingsChangedEventArgs
    {
        public string Key { get; internal set; }
        public object NewValue { get; internal set; }

        /// <summary>
        /// The delegate to use for handlers that receive the SettingsChanged event.
        /// </summary>
        public delegate void FeatureSettingsChangedEventHandler(FeatureSettingsChangedEventArgs e);
    }
}
