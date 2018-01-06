using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherUnderground;
using SimpleWeather.WeatherYahoo;
#if WINDOWS_UWP
using Windows.UI;
using Windows.UI.Xaml.Media;
#elif __ANDROID__
using Android.Graphics;
#endif

namespace SimpleWeather.WeatherData
{
    public static class WeatherAPI
    {
        // APIs
        public const string WeatherUnderground = "WUnderground";
        public const string Yahoo = "Yahoo";
    }

    public class WeatherManager : IWeatherProviderImpl
    {
        private static WeatherManager Instance;
        private static WeatherProviderImpl WeatherProvider;

        public WeatherManager()
        {
            UpdateAPI();
        }

        public static WeatherManager GetInstance()
        {
            if (Instance == null)
                Instance = new WeatherManager();

            return Instance;
        }

        public bool KeyRequired => WeatherProvider.KeyRequired;

        public bool SupportsWeatherLocale => WeatherProvider.SupportsWeatherLocale;

        public void UpdateAPI()
        {
            string API = Settings.API;

            switch (API)
            {
                case WeatherAPI.WeatherUnderground:
                    WeatherProvider = new WeatherUndergroundProvider();
                    break;
                case WeatherAPI.Yahoo:
                    WeatherProvider = new YahooWeatherProvider();
                    break;
            }
        }

        public async Task<string> UpdateLocationQuery(Weather weather)
        {
            return await WeatherProvider.UpdateLocationQuery(weather);
        }

        public async Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string ac_query)
        {
            return await WeatherProvider.GetLocations(ac_query);
        }

#if WINDOWS_UWP
        public async Task<LocationQueryViewModel> GetLocation(Windows.Devices.Geolocation.Geoposition geoPos)
        {
            return await GetLocation(new WeatherUtils.Coordinate(geoPos));
        }
#elif __ANDROID__
        public async Task<LocationQueryViewModel> GetLocation(Android.Locations.Location location)
        {
            return await GetLocation(new WeatherUtils.Coordinate(location));
        }
#endif
        public async Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coord)
        {
            return await WeatherProvider.GetLocation(coord);
        }

        public async Task<Weather> GetWeather(string query)
        {
            return await WeatherProvider.GetWeather(query);
        }

        public String LocaleToLangCode(String iso, String name)
        {
            return WeatherProvider.LocaleToLangCode(iso, name);
        }

        public string GetWeatherIcon(string icon)
        {
            return WeatherProvider.GetWeatherIcon(icon);
        }

        public async Task<bool> IsKeyValid(string key)
        {
            return await WeatherProvider.IsKeyValid(key);
        }

        public bool IsNight(Weather weather)
        {
            return WeatherProvider.IsNight(weather);
        }

#if WINDOWS_UWP
        public string GetWeatherIconURI(string icon)
        {
            return WeatherProvider.GetWeatherIconURI(icon);
        }
#endif

        public string GetBackgroundURI(Weather weather)
        {
            return WeatherProvider.GetBackgroundURI(weather);
        }

#if WINDOWS_UWP
        public Color GetWeatherBackgroundColor(Weather weather)
#elif __ANDROID__
        public Color GetWeatherBackgroundColor(Weather weather)
#endif
        {
            return WeatherProvider.GetWeatherBackgroundColor(weather);
        }

#if __ANDROID__
        public int GetWeatherIconResource(string icon)
        {
            return WeatherProvider.GetWeatherIconResource(icon);
        }
#endif

#if WINDOWS_UWP
        public void SetBackground(ImageBrush bg, Weather weather)
        {
            WeatherProvider.SetBackground(bg, weather);
        }
#endif
    }
}
