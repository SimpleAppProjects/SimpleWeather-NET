using static SimpleWeather.Preferences.SettingsChangedEventArgs;

namespace SimpleWeather.Preferences
{
    public abstract partial class BaseSettingsManager : ISettingsService
    {
        protected virtual partial void Init();
        public event SettingsChangedEventHandler OnSettingsChanged;

        public BaseSettingsManager()
        {
            Init();
        }
    }
}