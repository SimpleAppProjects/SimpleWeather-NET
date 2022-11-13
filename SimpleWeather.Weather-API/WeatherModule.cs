using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SimpleWeather.Preferences;
using SimpleWeather.Weather_API.LocationData;
using SimpleWeather.Weather_API.TZDB;
using SimpleWeather.Weather_API.WeatherData;
using System;

namespace SimpleWeather.Weather_API
{
    public class WeatherModule
    {
        private static readonly Lazy<WeatherModule> lazy = new(() =>
        {
            return new WeatherModule();
        });

        public static WeatherModule Instance => lazy.Value;

        private WeatherModule() { }

        public WeatherProviderManager WeatherManager => Ioc.Default.GetService<WeatherProviderManager>();
        public IWeatherProviderFactory WeatherProviderFactory => Ioc.Default.GetService<IWeatherProviderFactory>();
        public IWeatherLocationProviderFactory LocationProviderFactory => Ioc.Default.GetService<IWeatherLocationProviderFactory>();
        public ITimeZoneProvider TZProvider => Ioc.Default.GetService<ITimeZoneProvider>();
        public ITZDBService TZDBService => Ioc.Default.GetService<ITZDBService>();

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<WeatherProviderManager>(_ =>
            {
                return new WeatherProviderManager(_.GetService<SettingsManager>());
            });
            serviceCollection.AddSingleton<IWeatherProviderFactory, WeatherProviderFactoryImpl>();
            serviceCollection.AddSingleton<IWeatherLocationProviderFactory, WeatherLocationProviderFactoryImpl>();
            serviceCollection.AddSingleton<ITimeZoneProvider, TimeZoneProviderImpl>();
            serviceCollection.AddSingleton<ITZDBService, TZDBServiceImpl>();
        }
    }
}
