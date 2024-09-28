using SimpleWeather.Weather_API.AccuWeather;
using SimpleWeather.Weather_API.BrightSky;
using SimpleWeather.Weather_API.HERE;
using SimpleWeather.Weather_API.MeteoFrance;
using SimpleWeather.Weather_API.Metno;
using SimpleWeather.Weather_API.NWS;
using SimpleWeather.Weather_API.OpenWeather;
using SimpleWeather.Weather_API.OpenWeather.OneCall;
using SimpleWeather.Weather_API.TomorrowIO;
using SimpleWeather.Weather_API.WeatherApi;
using SimpleWeather.Weather_API.WeatherBit;
using SimpleWeather.Weather_API.WeatherKit;
using SimpleWeather.Weather_API.WeatherUnlocked;
using SimpleWeather.WeatherData;
using System;

namespace SimpleWeather.Weather_API.WeatherData
{
    public class WeatherProviderFactoryImpl : IWeatherProviderFactory
    {
        public IWeatherProvider GetWeatherProvider(string provider)
        {
            switch (provider)
            {
                case WeatherAPI.Here:
                    return new HEREWeatherProvider();

                case WeatherAPI.OpenWeatherMap:
                    {
                        var SettingsManager = DI.Utils.SettingsManager;
                        var RemoteConfigService = DI.Utils.RemoteConfigService;

                        if (SettingsManager.UsePersonalKeys[WeatherAPI.OpenWeatherMap] &&
                            RemoteConfigService.IsProviderEnabled(WeatherAPI.OpenWeatherMap_OneCall))
                        {
                            return new OWMOneCallWeatherProvider();
                        }

                        return new OpenWeatherMapProvider();
                    }

                case WeatherAPI.MetNo:
                    return new MetnoWeatherProvider();

                case WeatherAPI.NWS:
                    return new NWSWeatherProvider();

                case WeatherAPI.WeatherApi:
                    return new WeatherApiProvider();

                case WeatherAPI.WeatherUnlocked:
                    return new WeatherUnlockedProvider();

                case WeatherAPI.MeteoFrance:
                    return new MeteoFranceProvider();

                case WeatherAPI.TomorrowIo:
                    return new TomorrowIOWeatherProvider();

                case WeatherAPI.AccuWeather:
                    return new AccuWeatherProvider();

                case WeatherAPI.WeatherBitIo:
                    return new WeatherBitIOProvider();

                case WeatherAPI.Apple:
                    return new WeatherKitProvider();

                case WeatherAPI.DWD:
                    return new BrightSkyProvider();

                default:
                    {
#if !DEBUG
                        return new WeatherApiProvider();
#else
                        throw new ArgumentNullException($"Weather provider not supported ({provider})");
#endif
                    }
            }
        }
    }
}
