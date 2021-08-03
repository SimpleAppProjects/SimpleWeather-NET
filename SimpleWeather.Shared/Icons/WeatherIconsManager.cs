using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Icons
{
    public sealed partial class WeatherIconsManager : IWeatherIconsProvider
    {
        private static WeatherIconsManager sInstance;
        private static IWeatherIconsProvider sIconsProvider;

        public static IReadOnlyDictionary<string, WeatherIconProvider> DefaultIcons;

        static WeatherIconsManager()
        {
            var defaultIcons = new Dictionary<string, WeatherIconProvider>(3);
            AddIconProvider(defaultIcons, new WeatherIconsProvider());
            AddIconProvider(defaultIcons, new WUndergroundIconsProvider());
            AddIconProvider(defaultIcons, new WeatherIconicProvider());
            DefaultIcons = defaultIcons;
        }

        private static void AddIconProvider(IDictionary<string, WeatherIconProvider> map, WeatherIconProvider provider)
        {
            map.TryAdd(provider.Key, provider);
        }

        // Prevent instance from being created outside of this class
        private WeatherIconsManager()
        {
            UpdateIconProvider();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static WeatherIconsManager GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new WeatherIconsManager();
            }

            return sInstance;
        }

        public void UpdateIconProvider()
        {
            var iconsSource = Settings.IconProvider;
            sIconsProvider = GetProvider(iconsSource);
        }

        public static IWeatherIconsProvider GetProvider(string iconsSource)
        {
            return SimpleLibrary.GetInstance().GetIconProvider(iconsSource);
        }

        public bool IsFontIcon
        {
            get { return sIconsProvider.IsFontIcon; }
        }
    }
}
