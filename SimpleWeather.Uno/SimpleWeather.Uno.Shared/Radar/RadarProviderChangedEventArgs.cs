namespace SimpleWeather.Uno.Radar
{
    public sealed class RadarProviderChangedEventArgs
    {
        public RadarProvider.RadarProviders NewValue { get; internal set; }

        /// <summary>
        /// The delegate to use for handlers that receive the RadarProviderChanged event.
        /// </summary>
        public delegate void RadarProviderChangedEventHandler(RadarProviderChangedEventArgs e);
    }
}
