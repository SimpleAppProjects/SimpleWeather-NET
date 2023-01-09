using Microsoft.UI.Xaml;
using Windows.ApplicationModel.Resources;

namespace SimpleWeather.Uno
{
    public interface IApplication
    {
        public ResourceLoader ResLoader { get; }
        public AppState AppState { get; }

        public bool IsSystemDarkTheme { get; }
        public ElementTheme CurrentTheme { get; }

        public void UpdateAppTheme();
        public void RegisterSettingsListener();
        public void UnregisterSettingsListener();
    }
}
