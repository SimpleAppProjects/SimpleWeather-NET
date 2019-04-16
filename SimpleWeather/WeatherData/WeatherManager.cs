﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using SimpleWeather.Controls;
using SimpleWeather.HERE;
using SimpleWeather.Location;
using SimpleWeather.Metno;
using SimpleWeather.OpenWeather;
using SimpleWeather.Utils;
using SimpleWeather.WeatherUnderground;
using SimpleWeather.WeatherYahoo;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.WeatherData
{
    public static class WeatherAPI
    {
        // APIs
        public const string Yahoo = "Yahoo";
        public const string WeatherUnderground = "WUnderground";
        public const string OpenWeatherMap = "OpenWeather";
        public const string MetNo = "Metno";
        public const string Here = "Here";
        public const string BingMaps = "Bing";

        public static List<ProviderEntry> APIs = new List<ProviderEntry>()
        {
            new ProviderEntry("HERE Weather", Here,
                "https://www.here.com/en", "https://developer.here.com/?create=Freemium-Basic&keepState=true&step=account"),
            new ProviderEntry("Yahoo Weather", Yahoo,
                "https://www.yahoo.com/weather?ilc=401", "https://www.yahoo.com/weather?ilc=401"),
            new ProviderEntry("WeatherUnderground", WeatherUnderground,
                "https://www.wunderground.com", "https://www.wunderground.com/signup?mode=api_signup"),
            new ProviderEntry("OpenWeatherMap", OpenWeatherMap,
                "http://www.openweathermap.org", "https://home.openweathermap.org/users/sign_up"),
            new ProviderEntry("MET Norway", MetNo,
                "https://www.met.no/en", "https://www.met.no/en"),
        };

        public static List<ProviderEntry> LocationAPIs = new List<ProviderEntry>()
        {
                new ProviderEntry("HERE Maps", Here,
                        "https://www.here.com/en", "https://developer.here.com/"),
                new ProviderEntry("WeatherUnderground", WeatherUnderground,
                        "https://www.wunderground.com", "https://www.wunderground.com"),
                new ProviderEntry("Bing Maps", BingMaps,
                        "https://bing.com/maps", "https://bing.com/maps")
        };
    }

    // Wrapper class for supported Weather Providers
    public sealed class WeatherManager : IWeatherProviderImpl
    {
        private static WeatherManager Instance;
        private static WeatherProviderImpl WeatherProvider;
        private static Weather WeatherData;

        // Prevent instance from being created outside of this class
        private WeatherManager()
        {
            UpdateAPI();
        }

        public static WeatherManager GetInstance()
        {
            if (Instance == null)
                Instance = new WeatherManager();

            return Instance;
        }

        public void UpdateAPI()
        {
            string API = Settings.API;

            WeatherProvider = GetProvider(API);
        }

        // Shouldn't be used in anything other than WeatherNow
        public void UpdateWeather(Weather weather)
        {
            WeatherData = weather;
        }

        // Static Methods
        public static WeatherProviderImpl GetProvider(string API)
        {
            WeatherProviderImpl providerImpl = null;

            switch (API)
            {
                case SimpleWeather.WeatherData.WeatherAPI.Yahoo:
                    providerImpl = new YahooWeatherProvider();
                    break;
                case SimpleWeather.WeatherData.WeatherAPI.WeatherUnderground:
                    providerImpl = new WeatherUndergroundProvider();
                    break;
                case SimpleWeather.WeatherData.WeatherAPI.Here:
                    providerImpl = new HEREWeatherProvider();
                    break;
                case SimpleWeather.WeatherData.WeatherAPI.OpenWeatherMap:
                    providerImpl = new OpenWeatherMapProvider();
                    break;
                case SimpleWeather.WeatherData.WeatherAPI.MetNo:
                    providerImpl = new MetnoWeatherProvider();
                    break;
                default:
                    break;
            }

            if (providerImpl == null)
                throw new ArgumentNullException(nameof(API), "Invalid API name! This API is not supported");

            return providerImpl;
        }

        public static bool IsKeyRequired(string API)
        {
            WeatherProviderImpl provider = null;
            bool needsKey = false;

            provider = GetProvider(API);

            needsKey = provider.KeyRequired;
            provider = null;
            return needsKey;
        }

        public static async Task<bool> IsKeyValid(string key, string API)
        {
            var provider = GetProvider(API);
            return await provider.IsKeyValid(key);
        }

        // Provider dependent methods
        public string WeatherAPI => WeatherProvider.WeatherAPI;

        public LocationProviderImpl LocationProvider => WeatherProvider.LocationProvider;

        public bool KeyRequired => WeatherProvider.KeyRequired;

        public bool SupportsWeatherLocale => WeatherProvider.SupportsWeatherLocale;

        public bool SupportsAlerts => WeatherProvider.SupportsAlerts;

        public bool NeedsExternalAlertData => WeatherProvider.NeedsExternalAlertData;

        public async Task UpdateLocationData(LocationData location)
        {
            await WeatherProvider.UpdateLocationData(location);
        }

        public async Task<string> UpdateLocationQuery(Weather weather)
        {
            return await WeatherProvider.UpdateLocationQuery(weather);
        }

        public async Task<string> UpdateLocationQuery(LocationData location)
        {
            return await WeatherProvider.UpdateLocationQuery(location);
        }

        public async Task<ObservableCollection<LocationQueryViewModel>> GetLocations(string ac_query)
        {
            return await WeatherProvider.GetLocations(ac_query);
        }

        public async Task<LocationQueryViewModel> GetLocation(Windows.Devices.Geolocation.Geoposition geoPos)
        {
            return await GetLocation(new WeatherUtils.Coordinate(geoPos));
        }

        public async Task<LocationQueryViewModel> GetLocation(WeatherUtils.Coordinate coord)
        {
            return await WeatherProvider.GetLocation(coord);
        }

        public async Task<LocationQueryViewModel> GetLocation(String query)
        {
            return await WeatherProvider.GetLocation(query);
        }

        public async Task<Weather> GetWeather(string query)
        {
            return await WeatherProvider.GetWeather(query);
        }

        public async Task<Weather> GetWeather(LocationData location)
        {
            return await WeatherProvider.GetWeather(location);
        }

        public async Task<List<WeatherAlert>> GetAlerts(LocationData location)
        {
            return await WeatherProvider.GetAlerts(location);
        }

        public String LocaleToLangCode(String iso, String name)
        {
            return WeatherProvider.LocaleToLangCode(iso, name);
        }

        public string GetWeatherIcon(string icon)
        {
            return WeatherProvider.GetWeatherIcon(icon);
        }

        public string GetWeatherIcon(bool isNight, string icon)
        {
            return WeatherProvider.GetWeatherIcon(isNight, icon);
        }

        public async Task<bool> IsKeyValid(string key)
        {
            return await WeatherProvider.IsKeyValid(key);
        }

        public string GetAPIKey()
        {
            return WeatherProvider.GetAPIKey();
        }

        public bool IsNight(Weather weather)
        {
            return WeatherProvider.IsNight(weather);
        }

        public string GetWeatherIconURI(string icon)
        {
            return WeatherProvider.GetWeatherIconURI(icon);
        }

        public string GetBackgroundURI(Weather weather)
        {
            return WeatherProvider.GetBackgroundURI(weather);
        }

        public Color GetWeatherBackgroundColor(Weather weather)
        {
            return WeatherProvider.GetWeatherBackgroundColor(weather);
        }

        public void SetBackground(ImageBrush bg, Weather weather)
        {
            WeatherProvider.SetBackground(bg, weather);
        }
    }
}
