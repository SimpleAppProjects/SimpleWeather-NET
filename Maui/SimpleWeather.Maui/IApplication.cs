namespace SimpleWeather.Maui
{
    public interface IApplication
    {
        public AppState AppState { get; }

        public bool IsSystemDarkTheme { get; }
        public AppTheme CurrentTheme { get; }

        public void UpdateAppTheme();
        public void RegisterSettingsListener();
        public void UnregisterSettingsListener();
    }
}
