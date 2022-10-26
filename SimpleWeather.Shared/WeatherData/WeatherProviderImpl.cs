﻿using SimpleWeather.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;

namespace SimpleWeather.WeatherData
{
    public abstract partial class WeatherProviderImpl : IWeatherProviderImpl, IRateLimitedRequest
    {
        // Variables
        public LocationProviderImpl LocationProvider { get; protected set; }
        public abstract string WeatherAPI { get; }
        public abstract bool SupportsWeatherLocale { get; }
        public abstract bool KeyRequired { get; }
        public virtual bool SupportsAlerts => true;
        public virtual bool NeedsExternalAlertData => true;

        public virtual int HourlyForecastInterval => 1;

        public virtual AuthType AuthType => AuthType.None;

        public virtual long GetRetryTime() => 5000;

        public virtual bool IsRegionSupported(string countryCode)
        {
            return true;
        }

        // Methods
        // AutoCompleteQuery
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public Task<ObservableCollection<LocationQuery>> GetLocations(String ac_query)
        {
            return LocationProvider.GetLocations(ac_query, WeatherAPI);
        }
        // GeopositionQuery
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coordinate)
        {
            return LocationProvider.GetLocation(coordinate, WeatherAPI);
        }
        // Weather
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<Weather> GetWeather(String location_query, String country_code);
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public virtual async Task<Weather> GetWeather(LocationData location)
        {
            if (location == null || location.query == null)
                throw new WeatherException(WeatherUtils.ErrorStatus.Unknown);

            var weather = await GetWeather(location.query, location.country_code);

            if (String.IsNullOrWhiteSpace(location.tz_long))
            {
                if (!String.IsNullOrWhiteSpace(weather.location.tz_long))
                {
                    location.tz_long = weather.location.tz_long;
                }
                else if (location.latitude != 0 && location.longitude != 0)
                {
                    String tzId = await TZDB.TZDBCache.GetTimeZone(location.latitude, location.longitude);
                    if (!Equals("unknown", tzId))
                        location.tz_long = tzId;
                }

#if !UNIT_TEST
                // Update DB here or somewhere else
                await Settings.UpdateLocation(location);
#endif
            }

            if (String.IsNullOrWhiteSpace(weather.location.tz_long))
                weather.location.tz_long = location.tz_long;

            if (String.IsNullOrWhiteSpace(weather.location.name))
                weather.location.name = location.name;

            weather.location.latitude = (float)location.latitude;
            weather.location.longitude = (float)location.longitude;

            // Provider-specific updates/fixes
            await UpdateWeatherData(location, weather);

            // Additional external data
            if (weather.condition.airQuality == null && weather.aqi_forecast == null)
            {
                await UpdateAQIData(location, weather);
                /*
                if (this is IAirQualityProvider aqiProvider)
                {
                    var aqiData = await aqiProvider.GetAirQualityData(location);
                    await UpdateAQIData(location, weather, aqiData);
                }
                */
            }

            if (weather.condition.pollen == null)
            {
                if (DevSettingsEnabler.DevSettingsEnabled)
                {
                    weather.condition.pollen = await new TomorrowIO.TomorrowIOWeatherProvider().GetPollenData(location);
                }
            }

            return weather;
        }

        /// <summary>
        /// Providers weather provider specific updates to the weather object; For example, location tz offset
        /// fixes, etc.
        /// </summary>
        /// <param name="location">Location data</param>
        /// <param name="weather">The weather data to update</param>
        /// <returns></returns>
        protected abstract Task UpdateWeatherData(LocationData location, Weather weather);

        private async Task UpdateAQIData(LocationData location, Weather weather)
        {
            var aqicnData = await new AQICN.AQICNProvider().GetAirQualityData(location);
            UpdateAQIData(location, weather, aqicnData);
        }

        private void UpdateAQIData(LocationData location, Weather weather, AirQualityData aqiData)
        {
            weather.condition.airQuality = aqiData?.current;

            if (aqiData is AQICN.AQICNData aqicnData)
            {
                try
                {
                    if (aqicnData?.uvi_forecast?.Count > 0)
                    {
                        for (int i = 0; i < aqicnData.uvi_forecast.Count; i++)
                        {
                            var uviData = aqicnData.uvi_forecast[i];
                            var date = DateTime.ParseExact(uviData.day, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                            if (i == 0 && weather.condition.uv == null && date.Equals(weather.condition.observation_time.Date))
                            {
                                if (weather.astronomy.sunrise != null && weather.astronomy.sunset != null)
                                {
                                    var obsLocalTime = weather.condition.observation_time.DateTime.TimeOfDay;
                                    // if before sunrise or after sunset, uv min
                                    if (obsLocalTime < weather.astronomy.sunrise.TimeOfDay || obsLocalTime > weather.astronomy.sunset.TimeOfDay)
                                    {
                                        weather.condition.uv = new UV(uviData.min);
                                    }
                                    else
                                    {
                                        var totalSunlightTime = weather.astronomy.sunset - weather.astronomy.sunrise;
                                        var solarNoon = weather.astronomy.sunrise + (totalSunlightTime / 2);

                                        // If +/- 2hrs within solar noon, UV max
                                        if (Math.Abs((obsLocalTime - solarNoon.TimeOfDay).TotalHours) <= 2)
                                        {
                                            weather.condition.uv = new UV(uviData.max);
                                        }
                                        // else uv avg
                                        else
                                        {
                                            weather.condition.uv = new UV(uviData.avg);
                                        }
                                    }
                                }
                            }

                            var forecastObj = weather.forecast.FirstOrDefault(f => f.date.Date.Equals(date));
                            if (forecastObj != null && forecastObj.extras?.uv_index == null)
                            {
                                if (forecastObj.extras == null)
                                {
                                    forecastObj.extras = new ForecastExtras();
                                }

                                forecastObj.extras.uv_index = uviData.max;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e, "Error parsing AQI data");
                }
            }

            weather.aqi_forecast = aqiData?.aqiForecast;
        }

        // Alerts
        public virtual Task<ICollection<WeatherAlert>> GetAlerts(LocationData location)
        {
            if (LocationUtils.IsUS(location.country_code))
            {
                return new NWS.NWSAlertProvider().GetAlerts(location);
            }
            else
            {
                // return new WeatherApi.WeatherApiProvider().GetAlerts(location);
                return Task.FromResult<ICollection<WeatherAlert>>(null);
            }
        }

        // KeyCheck
        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public abstract Task<bool> IsKeyValid(String key);
        public abstract String GetAPIKey();

        // Utils Methods
        public Task UpdateLocationData(LocationData location)
        {
            return LocationProvider.UpdateLocationData(location, WeatherAPI);
        }
        public abstract String UpdateLocationQuery(Weather weather);
        public abstract String UpdateLocationQuery(LocationData location);
        public virtual String LocaleToLangCode(String iso, String name) { return "EN"; }
        public abstract String GetWeatherIcon(String icon);
        // Used in some providers for hourly forecast
        public virtual string GetWeatherIcon(bool isNight, String icon)
        {
            return GetWeatherIcon(icon);
        }

        /// <summary>
        /// Map the passed icon string to a localized weather condition string (if available)
        /// </summary>
        /// <param name="icon">The WeatherIcons to map</param>
        /// <returns>A localized weather condition string (if available); returns NULL if provider already supports localized data</returns>
        public virtual string GetWeatherCondition(String icon)
        {
            return icon switch
            {
                WeatherIcons.DAY_SUNNY
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_sunny"),

                WeatherIcons.NIGHT_CLEAR
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_clear"),

                WeatherIcons.DAY_SUNNY_OVERCAST or
                WeatherIcons.NIGHT_OVERCAST or
                WeatherIcons.OVERCAST
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_overcast"),

                WeatherIcons.DAY_PARTLY_CLOUDY or
                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_partlycloudy"),

                WeatherIcons.DAY_CLOUDY or
                WeatherIcons.NIGHT_ALT_CLOUDY or
                WeatherIcons.CLOUDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH or
                WeatherIcons.DAY_CLOUDY_HIGH
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_cloudy"),

                WeatherIcons.DAY_SPRINKLE or
                WeatherIcons.NIGHT_ALT_SPRINKLE or
                WeatherIcons.SPRINKLE or
                WeatherIcons.DAY_SHOWERS or
                WeatherIcons.NIGHT_ALT_SHOWERS or
                WeatherIcons.SHOWERS
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_rainshowers"),

                WeatherIcons.DAY_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_THUNDERSTORM or
                WeatherIcons.THUNDERSTORM or
                WeatherIcons.DAY_STORM_SHOWERS or
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_tstorms"),

                WeatherIcons.DAY_SLEET or
                WeatherIcons.NIGHT_ALT_SLEET or
                WeatherIcons.SLEET
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_sleet"),

                WeatherIcons.DAY_SNOW or
                WeatherIcons.NIGHT_ALT_SNOW or
                WeatherIcons.SNOW
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_snow"),

                WeatherIcons.DAY_SNOW_WIND or
                WeatherIcons.NIGHT_ALT_SNOW_WIND or
                WeatherIcons.SNOW_WIND
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_heavysnow"),

                WeatherIcons.DAY_SNOW_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM or
                WeatherIcons.SNOW_THUNDERSTORM
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_snow_tstorms"),

                WeatherIcons.HAIL or
                WeatherIcons.DAY_HAIL or
                WeatherIcons.NIGHT_ALT_HAIL
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_hail"),

                WeatherIcons.DAY_RAIN or
                WeatherIcons.NIGHT_ALT_RAIN or
                WeatherIcons.RAIN
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_rain"),

                WeatherIcons.DAY_FOG or
                WeatherIcons.NIGHT_FOG or
                WeatherIcons.FOG
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_fog"),

                WeatherIcons.DAY_SLEET_STORM or
                WeatherIcons.NIGHT_ALT_SLEET_STORM or
                WeatherIcons.SLEET_STORM
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_sleet_tstorms"),

                WeatherIcons.SNOWFLAKE_COLD
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_cold"),

                WeatherIcons.DAY_HOT or
                WeatherIcons.NIGHT_HOT or
                WeatherIcons.HOT
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_hot"),

                WeatherIcons.DAY_HAZE or
                WeatherIcons.NIGHT_HAZE or
                WeatherIcons.HAZE
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_haze"),

                WeatherIcons.SMOKE
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_smoky"),

                WeatherIcons.SANDSTORM or
                WeatherIcons.DUST
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_dust"),

                WeatherIcons.TORNADO
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_tornado"),

                WeatherIcons.DAY_RAIN_MIX or
                WeatherIcons.NIGHT_ALT_RAIN_MIX or
                WeatherIcons.RAIN_MIX
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_rainandsnow"),

                WeatherIcons.DAY_WINDY or
                WeatherIcons.NIGHT_WINDY or
                WeatherIcons.WINDY or
                WeatherIcons.DAY_CLOUDY_WINDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY or
                WeatherIcons.CLOUDY_WINDY or
                WeatherIcons.DAY_CLOUDY_GUSTS or
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS or
                WeatherIcons.CLOUDY_GUSTS or
                WeatherIcons.STRONG_WIND
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_windy"),

                WeatherIcons.HURRICANE
                => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_tropicalstorm"),

                _ => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_notavailable"),
            };
        }

        // Used for current condition
        public virtual bool IsNight(Weather weather)
        {
            bool isNight = weather.condition.icon switch
            {
                WeatherIcons.NIGHT_CLEAR or
                WeatherIcons.NIGHT_ALT_CLOUDY or
                WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS or
                WeatherIcons.NIGHT_ALT_CLOUDY_WINDY or
                WeatherIcons.NIGHT_FOG or
                WeatherIcons.NIGHT_ALT_HAIL or
                WeatherIcons.NIGHT_HAZE or
                WeatherIcons.NIGHT_ALT_LIGHTNING or
                WeatherIcons.NIGHT_ALT_RAIN or
                WeatherIcons.NIGHT_ALT_RAIN_MIX or
                WeatherIcons.NIGHT_ALT_RAIN_WIND or
                WeatherIcons.NIGHT_ALT_SHOWERS or
                WeatherIcons.NIGHT_ALT_SLEET or
                WeatherIcons.NIGHT_ALT_SLEET_STORM or
                WeatherIcons.NIGHT_ALT_SNOW or
                WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM or
                WeatherIcons.NIGHT_ALT_SNOW_WIND or
                WeatherIcons.NIGHT_ALT_SPRINKLE or
                WeatherIcons.NIGHT_ALT_STORM_SHOWERS or
                WeatherIcons.NIGHT_OVERCAST or
                WeatherIcons.NIGHT_ALT_THUNDERSTORM or
                WeatherIcons.NIGHT_WINDY or
                WeatherIcons.NIGHT_HOT or
                WeatherIcons.NIGHT_ALT_CLOUDY_HIGH or
                WeatherIcons.NIGHT_LIGHT_WIND or
                WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY => true,

                _ => false
            };

            return isNight;
        }

        public virtual Color GetWeatherBackgroundColor(Weather weather)
        {
            byte[] argb = null;
            String icon = weather.condition.icon;

            // Apply background based on weather condition
            switch (icon)
            {
                // Rain
                case WeatherIcons.DAY_RAIN:
                case WeatherIcons.DAY_RAIN_MIX:
                case WeatherIcons.DAY_RAIN_WIND:
                case WeatherIcons.DAY_SHOWERS:
                case WeatherIcons.DAY_SLEET:
                case WeatherIcons.DAY_SPRINKLE:
                    argb = new byte[4] { 0xff, 0x10, 0x20, 0x30 };
                    break;

                case WeatherIcons.NIGHT_ALT_HAIL:
                case WeatherIcons.NIGHT_ALT_RAIN:
                case WeatherIcons.NIGHT_ALT_RAIN_MIX:
                case WeatherIcons.NIGHT_ALT_RAIN_WIND:
                case WeatherIcons.NIGHT_ALT_SHOWERS:
                case WeatherIcons.NIGHT_ALT_SLEET:
                case WeatherIcons.NIGHT_ALT_SPRINKLE:
                case WeatherIcons.RAIN:
                case WeatherIcons.RAIN_MIX:
                case WeatherIcons.RAIN_WIND:
                case WeatherIcons.SHOWERS:
                case WeatherIcons.SLEET:
                case WeatherIcons.SPRINKLE:
                    argb = new byte[4] { 0xff, 0x18, 0x10, 0x10 };
                    break;
                // Tornado / Hurricane / Thunderstorm / Tropical Storm
                case WeatherIcons.DAY_LIGHTNING:
                case WeatherIcons.DAY_THUNDERSTORM:
                    argb = new byte[4] { 0xff, 0x28, 0x38, 0x48 };
                    break;

                case WeatherIcons.NIGHT_ALT_LIGHTNING:
                case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                case WeatherIcons.LIGHTNING:
                case WeatherIcons.THUNDERSTORM:
                    argb = new byte[4] { 0xff, 0x18, 0x18, 0x30 };
                    break;

                case WeatherIcons.DAY_STORM_SHOWERS:
                case WeatherIcons.DAY_SLEET_STORM:
                case WeatherIcons.STORM_SHOWERS:
                case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                case WeatherIcons.HAIL:
                case WeatherIcons.HURRICANE:
                case WeatherIcons.TORNADO:
                    argb = new byte[4] { 0xff, 0x18, 0x28, 0x30 };
                    break;
                // Dust
                case WeatherIcons.DUST:
                case WeatherIcons.SANDSTORM:
                    argb = new byte[4] { 0xff, 0xb0, 0x68, 0x10 };
                    break;
                // Foggy / Haze
                case WeatherIcons.DAY_FOG:
                case WeatherIcons.DAY_HAZE:
                case WeatherIcons.FOG:
                case WeatherIcons.NIGHT_FOG:
                case WeatherIcons.SMOG:
                case WeatherIcons.SMOKE:
                    argb = new byte[4] { 0xff, 0x20, 0x20, 0x20 };
                    break;
                // Snow / Snow Showers/Storm
                case WeatherIcons.DAY_SNOW:
                case WeatherIcons.DAY_SNOW_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_SNOW:
                case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                case WeatherIcons.SNOW:
                    argb = new byte[4] { 0xff, 0xb8, 0xd0, 0xf0 };
                    break;

                case WeatherIcons.SNOW_WIND:
                case WeatherIcons.DAY_SNOW_WIND:
                case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                    argb = new byte[4] { 0xff, 0xe0, 0xe0, 0xe0 };
                    break;
                /* Ambigious weather conditions */
                // (Mostly) Cloudy
                case WeatherIcons.CLOUD:
                case WeatherIcons.CLOUDY:
                case WeatherIcons.CLOUDY_GUSTS:
                case WeatherIcons.CLOUDY_WINDY:
                case WeatherIcons.DAY_CLOUDY:
                case WeatherIcons.DAY_CLOUDY_GUSTS:
                case WeatherIcons.DAY_CLOUDY_HIGH:
                case WeatherIcons.DAY_CLOUDY_WINDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS:
                case WeatherIcons.NIGHT_ALT_CLOUDY_HIGH:
                case WeatherIcons.NIGHT_ALT_CLOUDY_WINDY:
                    if (IsNight(weather))
                        argb = new byte[4] { 0xff, 0x18, 0x20, 0x20 };
                    else
                        argb = new byte[4] { 0xff, 0x50, 0x80, 0xa8 };
                    break;
                // Partly Cloudy
                case WeatherIcons.DAY_SUNNY_OVERCAST:
                case WeatherIcons.DAY_PARTLY_CLOUDY:
                case WeatherIcons.NIGHT_OVERCAST:
                case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                    if (IsNight(weather))
                        argb = new byte[4] { 0xff, 0x18, 0x18, 0x20 };
                    else
                        argb = new byte[4] { 0xff, 0x88, 0xb0, 0xc8 };
                    break;

                case WeatherIcons.DAY_SUNNY:
                case WeatherIcons.NA:
                case WeatherIcons.NIGHT_CLEAR:
                case WeatherIcons.SNOWFLAKE_COLD:
                case WeatherIcons.DAY_HOT:
                case WeatherIcons.WINDY:
                case WeatherIcons.STRONG_WIND:
                default:
                    // Set background based using sunset/rise times
                    if (IsNight(weather))
                        argb = new byte[4] { 0xff, 0x18, 0x10, 0x18 };
                    else
                        argb = new byte[4] { 0xff, 0x20, 0xa8, 0xd8 };
                    break;
            }

            // Just in case
            if (argb == null)
            {
                // Set background based using sunset/rise times
                if (IsNight(weather))
                {
                    // Night background
                    argb = new byte[4] { 0xff, 0x18, 0x10, 0x18 };
                }
                else
                {
                    // set day bg
                    argb = new byte[4] { 0xff, 0x20, 0xa8, 0xd8 };
                }
            }

            return Color.FromArgb(argb[0], argb[1], argb[2], argb[3]);
        }
    }
}