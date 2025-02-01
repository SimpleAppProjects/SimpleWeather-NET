using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using System.Collections.Generic;
using System.Linq;

namespace SimpleWeather.Icons
{
    public sealed partial class WeatherIconsManager : IWeatherIconsProvider
    {
        private IWeatherIconsProvider _IconsProvider;
        private readonly OrderedDictionary<string, WeatherIconProvider> _IconProviders = [];
        public IReadOnlyDictionary<string, WeatherIconProvider> DefaultIcons;

        private readonly SettingsManager SettingsManager = DI.Utils.SettingsManager;

        internal WeatherIconsManager()
        {
            var defaultIcons = new Dictionary<string, WeatherIconProvider>(3);
            AddIconProvider(defaultIcons, new WeatherIconsEFProvider());
            AddIconProvider(defaultIcons, new WUndergroundIconsProvider());
            AddIconProvider(defaultIcons, new WeatherIconicProvider());
            AddIconProvider(defaultIcons, new JDWIIconsProvider());
            DefaultIcons = defaultIcons;

            // Register default icon providers
            ResetIconProviders();

            UpdateIconProvider();
        }

        public void UpdateIconProvider()
        {
            var iconsSource = SettingsManager.IconProvider;
            _IconsProvider = GetIconProvider(iconsSource);
        }

        public void RegisterIconProvider(WeatherIconProvider provider)
        {
            if (!_IconProviders.ContainsKey(provider.Key))
                _IconProviders.Add(provider.Key, provider);
        }

        public WeatherIconProvider GetIconProvider(string key)
        {
            WeatherIconProvider provider = _IconProviders[key];
            if (provider == null)
            {
                // Can't find the provider for this key; fallback to default/first available
                if (_IconProviders.Count > 0)
                {
                    provider = _IconProviders.Values.First();
                }
                else
                {
                    RegisterIconProvider(provider = new WeatherIconsEFProvider());
                }
            }
            return provider;
        }

        public IReadOnlyDictionary<string, WeatherIconProvider> GetIconProviders()
        {
            return _IconProviders;
        }

        public void ResetIconProviders()
        {
            _IconProviders.Clear();
            foreach (var provider in DefaultIcons)
            {
                _IconProviders.Add(provider.Key, provider.Value);
            }
        }

        private static void AddIconProvider(IDictionary<string, WeatherIconProvider> map, WeatherIconProvider provider)
        {
            map.TryAdd(provider.Key, provider);
        }

        public IWeatherIconsProvider Provider
        {
            get
            {
                if (_IconsProvider == null)
                {
                    UpdateIconProvider();
                }
                return _IconsProvider;
            }
        }

        public bool IsFontIcon
        {
            get { return _IconsProvider.IsFontIcon; }
        }

        public bool ShouldUseMonochrome() => ShouldUseMonochrome(SettingsManager.IconProvider);

        public bool ShouldUseMonochrome(string wip)
        {
            switch (wip)
            {
                case "wi-erik-flowers":
                case "wui-ashley-jager":
                case "w-iconic-jackd248":
                case "pixeden-icons_set-weather":
                default:
                    return true;
                case "meteocons-basmilius":
                case "wci_sliu_iconfinder":
                    return false;
            }
        }
    }
}
