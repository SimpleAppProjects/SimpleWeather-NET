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

        private static IWeatherIconsProvider GetProvider(string iconsSource)
        {
            IWeatherIconsProvider iconsProvider = null;

            switch (iconsSource)
            {
                default:
                    iconsProvider = new WeatherIconsProvider();
                    break;
            }

            return iconsProvider;
        }
    }
}
