using Microsoft.UI.Xaml;
using SimpleWeather.NET.Localization;

namespace SimpleWeather.NET
{
    public interface IApplication
    {
        public IResourceLoader ResLoader { get; }
        public AppState AppState { get; }

        public bool IsSystemDarkTheme { get; }
        public ElementTheme CurrentTheme { get; }

        public void UpdateAppTheme();
        public void RegisterSettingsListener();
        public void UnregisterSettingsListener();
    }
}
